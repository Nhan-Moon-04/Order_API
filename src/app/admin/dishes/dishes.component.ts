import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';
import { Dish, PagedResponse } from '../../model/dish.model';
import { DishGroup } from '../../model/dish-group.model';
import { Kitchen } from '../../model/kitchen.model';
@Component({
  selector: 'app-dishes',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './dishes.component.html',
  styleUrl: './dishes.component.css',
})
export class DishesComponent implements OnInit {
  private http = inject(HttpClient);
  private location = inject(Location);

  dishes = signal<Dish[]>([]);
  totalPages = signal(0);
  pageIndex = signal(1);
  pageSize = 10;
  error = signal<string | null>(null);
  loading = signal(false);
  disgroup = signal<DishGroup[]>([]);

  // Page size dropdown
  dropdownValues: number[] = [5, 10, 15, 20, 25];
  selectedPageSize = signal<number>(10);

  // Sort properties
  sortColumn = signal<string>('');
  sortDirection = signal<'asc' | 'desc'>('asc');

  // Search and filter properties
  searchText = signal<string>('');
  selectedGroup = signal<string>('');
  selectedStatus = signal<string>('');
  dishGroups = signal<any[]>([]);
  kitchens = signal<any[]>([]);
  filteredDishes = signal<Dish[]>([]);

  // Modal properties
  showAddModal = signal(false);
  showEditModal = signal(false);
  newDish = signal<any>({
    dishName: '',
    basePrice: 0,
    groupId: '',
    kitchenId: '',
    isActive: true,
  });
  editDish = signal<any>({});

  ngOnInit() {
    // Load saved page size from localStorage
    const savedPageSize = localStorage.getItem('dishesPageSize');
    if (savedPageSize) {
      this.selectedPageSize.set(+savedPageSize);
      this.pageSize = +savedPageSize;
    }

    this.getAllDishes(1);
    this.loadDishGroups();
    this.loadKitchenGroups();
  }

  loadKitchenGroups() {
    this.http
      .get<any[]>(`${environment.apiUrl}/Kitchens`)
      .pipe(catchError(() => of([])))
      .subscribe((kitchen) => {
        this.kitchens.set(kitchen);
      });
  }
  getAllDishes(page: number) {
    this.loading.set(true);
    this.error.set(null);

    const requestBody = {
      searchString: this.searchText(),
      pageIndex: page,
      pageSize: this.selectedPageSize(),
      isActive: 1,
    };

    this.http
      .post<PagedResponse<Dish>>(`${environment.apiUrl}/Dishes/GetAllDishes`, requestBody)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to load dishes.');
          this.loading.set(false);
          return of({
            items: [],
            totalRecords: 0,
            totalPages: 0,
            pageIndex: 1,
            pageSize: this.selectedPageSize(),
          });
        })
      )
      .subscribe((response) => {
        this.dishes.set(response.items);
        this.totalPages.set(response.totalPages);
        this.pageIndex.set(response.pageIndex);
        this.loading.set(false);
        this.applyFilters();
      });
  }

  loadDishGroups() {
    this.http
      .get<any[]>(`${environment.apiUrl}/DishGroup`)
      .pipe(catchError(() => of([])))
      .subscribe((groups) => {
        this.dishGroups.set(groups);
      });
  }

  // Search functionality
  onSearch() {
    this.getAllDishes(1);
  }

  clearSearch() {
    this.searchText.set('');
    this.getAllDishes(1);
  }

  // Filter functionality
  onFilterChange() {
    this.applyFilters();
  }

  applyFilters() {
    let filtered = this.dishes();

    // Filter by group
    if (this.selectedGroup()) {
      filtered = filtered.filter((dish) => dish.groupId === this.selectedGroup());
    }

    // Filter by status
    if (this.selectedStatus()) {
      const isActive = this.selectedStatus() === 'true';
      filtered = filtered.filter((dish) => dish.isActive === isActive);
    }

    // Apply sorting
    if (this.sortColumn()) {
      filtered = this.sortDishes(filtered);
    }

    this.filteredDishes.set(filtered);
  }

  // Sort functionality
  onSortColumn(column: string) {
    if (this.sortColumn() === column) {
      // Toggle direction if same column
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      // New column, default to ascending
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    this.applyFilters();
  }

  sortDishes(dishes: Dish[]): Dish[] {
    const column = this.sortColumn();
    const direction = this.sortDirection();

    return [...dishes].sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (column) {
        case 'price':
          valueA = a.basePrice;
          valueB = b.basePrice;
          break;
        case 'name':
          valueA = a.dishName.toLowerCase();
          valueB = b.dishName.toLowerCase();
          break;
        case 'group':
          valueA = (a.groupName || '').toLowerCase();
          valueB = (b.groupName || '').toLowerCase();
          break;
        default:
          return 0;
      }

      if (valueA < valueB) {
        return direction === 'asc' ? -1 : 1;
      }
      if (valueA > valueB) {
        return direction === 'asc' ? 1 : -1;
      }
      return 0;
    });
  }

  getSortIcon(column: string): string {
    if (this.sortColumn() !== column) {
      return '↕️'; // No sort
    }
    return this.sortDirection() === 'asc' ? '↑' : '↓';
  }

  clearAllFilters() {
    this.searchText.set('');
    this.selectedGroup.set('');
    this.selectedStatus.set('');
    this.getAllDishes(1);
  }

  // Pagination
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const total = this.totalPages();
    const current = this.pageIndex();

    // Show max 5 pages around current page
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }

  // Page size change handler
  onPageSizeChange(newSize: number) {
    this.selectedPageSize.set(newSize);
    this.pageSize = newSize;
    localStorage.setItem('dishesPageSize', newSize.toString());
    this.getAllDishes(1); // Reset to first page when changing page size
  }

  // Modal functions
  openAddModal() {
    this.newDish.set({
      dishName: '',
      basePrice: 0,
      groupId: '',
      kitchenId: '',
      description: '',
      isActive: true,
    });
    this.showAddModal.set(true);
  }

  closeAddModal() {
    this.showAddModal.set(false);
  }

  openEditModal(dish: Dish) {
    this.editDish.set({ ...dish });
    this.showEditModal.set(true);
  }

  closeEditModal() {
    this.showEditModal.set(false);
  }

  // CRUD operations
  addDish() {
    this.loading.set(true);

    const dishData = {
      dishName: this.newDish().dishName,
      basePrice: this.newDish().basePrice,
      kitchenId: this.newDish().kitchenId,
      groupId: this.newDish().groupId,
      description: this.newDish().description,
      isActive: this.newDish().isActive,
    };

    this.http
      .post(`${environment.apiUrl}/Dishes/AddDish`, dishData)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to add dish.');
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe((response) => {
        if (response) {
          this.getAllDishes(this.pageIndex());
          this.closeAddModal();
        }
        this.loading.set(false);
      });
  }

  updateDish() {
    this.loading.set(true);

    const payload = {
      dishId: this.editDish().dishId,
      dishName: this.editDish().dishName,
      basePrice: this.editDish().basePrice,
      kitchenId: this.editDish().kitchenId,
      groupId: this.editDish().groupId,
      description: this.editDish().description,
      isActive: this.editDish().isActive,
    };

    this.http
      .put(`${environment.apiUrl}/Dishes/UpdateDish`, payload)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to update dish.');
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe((response: any) => {
        if (response && response.isSuccess !== false) {
          // If API returns wrapper { statusCode, isSuccess, message, data }
          if (response.isSuccess === undefined && response.statusCode !== undefined) {
            if (response.isSuccess) {
              this.getAllDishes(this.pageIndex());
              this.closeEditModal();
            } else {
              this.error.set(response.message || 'Failed to update dish.');
            }
          } else {
            // Plain response
            this.getAllDishes(this.pageIndex());
            this.closeEditModal();
          }
        }
        this.loading.set(false);
      });
  }

  toggleDishStatus(dish: Dish) {
    const updatedDish = { ...dish, isActive: !dish.isActive };

    this.http
      .put(`${environment.apiUrl}/Dishes/${dish.dishId}`, updatedDish)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to update dish status.');
          return of(null);
        })
      )
      .subscribe((response) => {
        if (response) {
          this.getAllDishes(this.pageIndex());
        }
      });
  }

  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }
}

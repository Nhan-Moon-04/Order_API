import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Observable, catchError, of, forkJoin, map } from 'rxjs';
import { Area } from '../../model/area.model';
import {
  AreaDishPrice,
  AreaDishPriceDisplay,
  AreaDishPriceSearchRequest,
  PagedResponse,
} from '../../model/area-dish-price.model';
import { Dish } from '../../model/dish.model';
import {
  ChangeQuantityRequest,
  RemoveFoodRequest,
  ChangeQuantityResponse,
  RemoveFoodResponse,
} from '../../model/order-quantity-request.model';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';

@Component({
  selector: 'app-area-prices',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './area-prices.component.html',
  styleUrl: './area-prices.component.css',
})
export class AreaPricesComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private location = inject(Location);

  areas = signal<Area[]>([]);
  selectedArea = signal<Area | null>(null);
  dishPrices = signal<AreaDishPriceDisplay[]>([]);
  loading = signal(false);
  loadingDishes = signal(false);
  error = signal<string | null>(null);
  dishError = signal<string | null>(null);

  // Pagination properties
  totalPages = signal(0);
  pageIndex = signal(1);
  pageSize = 10;
  dropdownValues: number[] = [5, 10, 15, 20, 25];
  selectedPageSize = signal<number>(10);

  // Search and filter properties
  searchText = signal<string>('');
  selectedAreaFilter = signal<string>('');
  selectedStatus = signal<string>('');
  dishNameFilter = signal<string>('');
  kitchenNameFilter = signal<string>('');
  groupNameFilter = signal<string>('');

  // Sort properties
  sortColumn = signal<string>('');
  sortDirection = signal<'asc' | 'desc'>('asc');
  filteredDishPrices = signal<AreaDishPriceDisplay[]>([]);

  // Edit Price Modal
  showEditModal = signal(false);
  editingDish = signal<AreaDishPriceDisplay | null>(null);
  newPrice = signal<number>(0);
  updatingPrice = signal(false);

  // Quantity Management Modal
  showQuantityModal = signal(false);
  editingQuantity = signal<AreaDishPriceDisplay | null>(null);
  currentOrderId = signal<string>('');
  newQuantity = signal<number>(1);
  updatingQuantity = signal(false);

  // Remove Food Modal
  showRemoveModal = signal(false);
  removingDish = signal<AreaDishPriceDisplay | null>(null);
  removingFood = signal(false);

  ngOnInit() {
    // Load saved page size from localStorage
    const savedPageSize = localStorage.getItem('areaPricesPageSize');
    if (savedPageSize) {
      this.selectedPageSize.set(+savedPageSize);
      this.pageSize = +savedPageSize;
    }

    this.loadAreas();
    this.getAllAreaDishPrices(1);
  }

  // Get all area dish prices with pagination
  getAllAreaDishPrices(page: number) {
    this.loadingDishes.set(true);
    this.dishError.set(null);

    const requestBody: AreaDishPriceSearchRequest = {
      searchString: this.searchText(),
      pageIndex: page,
      pageSize: this.selectedPageSize(),
      isActive: 1,
      areaId: this.selectedAreaFilter(),
      dishName: this.dishNameFilter(),
      kitchenName: this.kitchenNameFilter(),
      groupName: this.groupNameFilter(),
    };

    this.http
      .post<PagedResponse<AreaDishPrice>>(
        `${environment.apiUrl}/AreaDishPrices/Search`,
        requestBody
      )
      .pipe(
        catchError((err) => {
          this.dishError.set('Failed to load area dish prices.');
          this.loadingDishes.set(false);
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
        this.dishPrices.set(response.items);
        this.totalPages.set(response.totalPages);
        this.pageIndex.set(response.pageIndex);
        this.loadingDishes.set(false);
        this.applyFilters();
      });
  }

  // Search functionality
  onSearch() {
    this.getAllAreaDishPrices(1);
  }

  clearSearch() {
    this.searchText.set('');
    this.getAllAreaDishPrices(1);
  }

  // Filter functionality
  onFilterChange() {
    this.getAllAreaDishPrices(1);
  }

  applyFilters() {
    let filtered = this.dishPrices();

    // Apply sorting if needed
    if (this.sortColumn()) {
      filtered = this.sortAreaDishPrices(filtered);
    }

    this.filteredDishPrices.set(filtered);
  }

  // Sort functionality
  onSortColumn(column: string) {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    this.applyFilters();
  }

  sortAreaDishPrices(items: AreaDishPriceDisplay[]): AreaDishPriceDisplay[] {
    const column = this.sortColumn();
    const direction = this.sortDirection();

    return [...items].sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (column) {
        case 'price':
          valueA = a.customPrice;
          valueB = b.customPrice;
          break;
        case 'dishName':
          valueA = (a.dishName || '').toLowerCase();
          valueB = (b.dishName || '').toLowerCase();
          break;
        case 'areaName':
          valueA = (a.areaName || '').toLowerCase();
          valueB = (b.areaName || '').toLowerCase();
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
      return '↕️';
    }
    return this.sortDirection() === 'asc' ? '↑' : '↓';
  }

  clearAllFilters() {
    this.searchText.set('');
    this.selectedAreaFilter.set('');
    this.selectedStatus.set('');
    this.dishNameFilter.set('');
    this.kitchenNameFilter.set('');
    this.groupNameFilter.set('');
    this.getAllAreaDishPrices(1);
  }

  // Pagination
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const total = this.totalPages();
    const current = this.pageIndex();

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
    localStorage.setItem('areaPricesPageSize', newSize.toString());
    this.getAllAreaDishPrices(1);
  }

  private loadAreas() {
    this.loading.set(true);
    this.error.set(null);

    this.getAreas().subscribe({
      next: (areas) => {
        this.areas.set(areas);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load areas');
        this.loading.set(false);
        console.error('Error loading areas:', err);
      },
    });
  }

  private getAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(`${environment.apiUrl}/Areas`).pipe(
      catchError((err) => {
        console.error('HTTP Error:', err);
        return of([]);
      })
    );
  }

  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }

  selectArea(area: Area) {
    console.log('Selected area:', area.areaName);
    this.selectedArea.set(area);
    this.selectedAreaFilter.set(area.areaId);
    this.getAllAreaDishPrices(1);
  }

  openEditModal(dish: AreaDishPriceDisplay) {
    this.editingDish.set(dish);
    this.newPrice.set(dish.customPrice);
    this.showEditModal.set(true);
  }

  closeEditModal() {
    this.showEditModal.set(false);
    this.editingDish.set(null);
    this.newPrice.set(0);
  }

  updatePrice() {
    const dish = this.editingDish();
    if (!dish) return;

    this.updatingPrice.set(true);

    const updatePayload = {
      id: dish.id,
      customPrice: this.newPrice(),
    };

    this.http
      .post(`${environment.apiUrl}/AreaDishPrices/update-price`, updatePayload)
      .pipe(
        catchError((err) => {
          console.error('Error updating price:', err);
          alert('Lỗi cập nhật giá: ' + (err.error?.message || err.message));
          return of(null);
        })
      )
      .subscribe({
        next: (response) => {
          if (response) {
            console.log('Price updated successfully:', response);
            this.closeEditModal();
            // Reload the dish prices to get updated data
            this.getAllAreaDishPrices(this.pageIndex());
          }
          this.updatingPrice.set(false);
        },
        error: (err) => {
          console.error('Error updating price:', err);
          alert('Lỗi cập nhật giá!');
          this.updatingPrice.set(false);
        },
      });
  }

  // Quantity Management Methods
  openQuantityModal(dish: AreaDishPriceDisplay, orderId: string) {
    this.editingQuantity.set(dish);
    this.currentOrderId.set(orderId);
    this.newQuantity.set(1); // Default quantity
    this.showQuantityModal.set(true);
  }

  closeQuantityModal() {
    this.showQuantityModal.set(false);
    this.editingQuantity.set(null);
    this.currentOrderId.set('');
    this.newQuantity.set(1);
  }

  changeQuantity() {
    const dish = this.editingQuantity();
    const orderId = this.currentOrderId();
    if (!dish || !orderId) return;

    this.updatingQuantity.set(true);

    const request: ChangeQuantityRequest = {
      orderId: orderId,
      dishId: dish.dishId,
      newQuantity: this.newQuantity(),
    };

    this.http
      .post<ChangeQuantityResponse>(`${environment.apiUrl}/OrderDetails/change-quantity`, request)
      .pipe(
        catchError((err) => {
          console.error('Error changing quantity:', err);
          alert('Lỗi thay đổi số lượng: ' + (err.error?.message || err.message));
          return of(null);
        })
      )
      .subscribe({
        next: (response) => {
          if (response) {
            console.log('Quantity changed successfully:', response);
            alert(`Đã cập nhật số lượng ${dish.dishName} thành ${response.quantity}`);
            this.closeQuantityModal();
          }
          this.updatingQuantity.set(false);
        },
        error: (err) => {
          console.error('Error changing quantity:', err);
          alert('Lỗi thay đổi số lượng!');
          this.updatingQuantity.set(false);
        },
      });
  }

  // Remove Food Methods
  openRemoveModal(dish: AreaDishPriceDisplay, orderId: string) {
    this.removingDish.set(dish);
    this.currentOrderId.set(orderId);
    this.showRemoveModal.set(true);
  }

  closeRemoveModal() {
    this.showRemoveModal.set(false);
    this.removingDish.set(null);
    this.currentOrderId.set('');
  }

  removeFood() {
    const dish = this.removingDish();
    const orderId = this.currentOrderId();
    if (!dish || !orderId) return;

    this.removingFood.set(true);

    const request: RemoveFoodRequest = {
      orderId: orderId,
      dishId: dish.dishId,
    };

    this.http
      .post<RemoveFoodResponse>(`${environment.apiUrl}/OrderDetails/RemoveFood`, request)
      .pipe(
        catchError((err) => {
          console.error('Error removing food:', err);
          alert('Lỗi xóa món ăn: ' + (err.error?.message || err.message));
          return of(null);
        })
      )
      .subscribe({
        next: (response) => {
          if (response && response.success) {
            console.log('Food removed successfully:', response);
            alert(response.message);
            this.closeRemoveModal();
          }
          this.removingFood.set(false);
        },
        error: (err) => {
          console.error('Error removing food:', err);
          alert('Lỗi xóa món ăn!');
          this.removingFood.set(false);
        },
      });
  }
}

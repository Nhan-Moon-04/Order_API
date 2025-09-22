import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  Observable,
  catchError,
  of,
  forkJoin,
  map,
  firstValueFrom,
  debounceTime,
  distinctUntilChanged,
  Subject,
} from 'rxjs';
import { Area } from '../../model/area.model';
import {
  AreaDishPrice,
  AreaDishPriceDisplay,
  AreaDishPriceSearchRequest,
  PagedResponse,
  AddDishesToAreaRequest,
  GetAvailableDishesRequest,
  AvailableDishesResponse,
  AvailableDish,
  DeleteAreaDishPriceRequest,
  DeleteAreaDishPriceResponse,
} from '../../model/area-dish-price.model';
import { Dish } from '../../model/dish.model';
import {
  ChangeQuantityRequest,
  ChangeQuantityResponse,
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
  totalRecords = signal(0); // Add this to track total records
  pageIndex = signal(1);
  pageSize = 10;
  dropdownValues: number[] = [5, 10, 15, 20, 25, 50, 100, 200, 500];
  selectedPageSize = signal<number>(10);

  // Search and filter properties
  searchText = signal<string>('');
  selectedAreaFilter = signal<string>('');
  selectedStatus = signal<string>('');
  dishNameFilter = signal<string>('');
  kitchenNameFilter = signal<string>('');
  groupNameFilter = signal<string>('');

  // Dish names dropdown for area filter
  dishNames = signal<string[]>([]);
  selectedDishNameFilter = signal<string>('');
  loadingDishNames = signal(false);
  // Suggestions debounce for dish name search (when typing)
  private dishNameInputSubject = new Subject<string>();

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

  // Add Dishes Modal
  showAddModal = signal(false);
  availableDishes = signal<AvailableDish[]>([]);
  selectedDish = signal<string>('');
  customPrice = signal<number>(0);
  loadingAvailableDishes = signal(false);
  addingDishes = signal(false);
  availableDishesSearchText = signal<string>('');

  // Search debounce
  private searchSubject = new Subject<string>();
  private dishNamesSearchSubject = new Subject<string>();

  ngOnInit() {
    // Load saved page size from localStorage
    const savedPageSize = localStorage.getItem('areaPricesPageSize');
    if (savedPageSize) {
      this.selectedPageSize.set(+savedPageSize);
      this.pageSize = +savedPageSize;
    }

    // Set up debounced search for available dishes
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe((searchText) => {
      this.availableDishesSearchText.set(searchText);
      if (this.selectedArea()) {
        this.loadAvailableDishes(this.selectedArea()!.areaId);
      }
    });

    // Set up debounced search for dish names by area
    this.dishNamesSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((areaId) => {
        if (areaId) {
          this.loadDishNamesByArea(areaId);
        } else {
          this.dishNames.set([]);
        }
      });

    // Debounced suggestions for dish name input
    this.dishNameInputSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => {
      // If there's a selected area, search names by area+term; otherwise call API with term
      const areaId = this.selectedAreaFilter();
      this.loadDishNameSuggestions(areaId, term);
    });

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
      areaId: this.selectedAreaFilter() || undefined, // Ensure empty string becomes undefined
      dishName: this.selectedDishNameFilter() || this.dishNameFilter() || undefined,
      kitchenName: this.kitchenNameFilter() || undefined,
      groupName: this.groupNameFilter() || undefined,
    };

    console.log('getAllAreaDishPrices - Request:', requestBody);
    console.log('getAllAreaDishPrices - Current pageIndex signal:', this.pageIndex());
    console.log('getAllAreaDishPrices - Current selectedAreaFilter:', this.selectedAreaFilter());

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
        console.log('Area dish prices response:', response);
        console.log('Total pages from response:', response.totalPages);
        console.log('Current page from response:', response.pageIndex);
        console.log('Total records from response:', response.totalRecords);

        // Enrich with dish base prices
        this.enrichAreaDishPricesWithDishInfo(response.items).then((enrichedItems) => {
          this.dishPrices.set(enrichedItems);
          this.totalPages.set(response.totalPages);
          this.totalRecords.set(response.totalRecords); // Store total records

          // Ensure pageIndex doesn't exceed totalPages
          const validPageIndex = Math.min(response.pageIndex, response.totalPages || 1);
          this.pageIndex.set(validPageIndex);

          console.log(
            'Updated signals - totalPages:',
            this.totalPages(),
            'totalRecords:',
            this.totalRecords(),
            'pageIndex:',
            this.pageIndex()
          );

          this.loadingDishes.set(false);
          this.applyFilters();
        });
      });
  }

  // Search functionality
  onSearch() {
    this.pageIndex.set(1); // Reset to first page when searching
    this.getAllAreaDishPrices(1);
  }

  clearSearch() {
    this.searchText.set('');
    this.pageIndex.set(1); // Reset to first page when clearing search
    this.getAllAreaDishPrices(1);
  }

  // Filter functionality
  onFilterChange() {
    this.pageIndex.set(1); // Reset to first page when filtering
    this.getAllAreaDishPrices(1);
  }

  onAreaFilterChange(areaId: string) {
    console.log('Area filter changed to:', areaId);
    this.selectedAreaFilter.set(areaId);
    this.pageIndex.set(1); // Reset to first page when changing area
    console.log('Reset pageIndex to 1, current pageSize:', this.selectedPageSize());

    // Load dish names for the selected area
    this.dishNamesSearchSubject.next(areaId);

    // Clear dish name filter when area changes
    this.selectedDishNameFilter.set('');

    this.getAllAreaDishPrices(1);
  }

  onDishNameFilterChange(dishName: string) {
    this.dishNameFilter.set(dishName);
    this.pageIndex.set(1); // Reset to first page when filtering
    this.getAllAreaDishPrices(1);
  }

  // Called on input event in the dish name text box to trigger suggestions
  onDishNameInput(value: string) {
    // keep the filter in sync
    this.dishNameFilter.set(value);
    // push into subject for debounced suggestions
    this.dishNameInputSubject.next(value);
  }

  // Called when user presses Enter in the dish name input
  onDishNameEnter(value: string) {
    // Apply the typed value as the selected dish name filter and trigger search
    this.dishNameFilter.set(value);
    this.selectedDishNameFilter.set(value);
    this.dishNames.set([]);
    this.pageIndex.set(1);
    this.getAllAreaDishPrices(1);
  }

  // Select suggestion from the dropdown
  selectDishNameSuggestion(name: string) {
    this.dishNameFilter.set(name);
    this.selectedDishNameFilter.set(name);
    // clear suggestions
    this.dishNames.set([]);
    this.pageIndex.set(1);
    this.getAllAreaDishPrices(1);
  }

  // Load suggestions for dish names (optionally filtered by areaId)
  private loadDishNameSuggestions(areaId: string | undefined, term: string) {
    if (!term || term.length < 1) {
      this.dishNames.set([]);
      return;
    }

    this.loadingDishNames.set(true);

    const request = {
      searchString: areaId || '',
      searchName: term,
    };

    // If areaId provided, include it in the request body (API expects searchString only; adjust if API supports area)
    // For now we call same GetDishNames endpoint with the typed term.

    this.http
      .post<{ statusCode: number; isSuccess: boolean; message: string; data: string[] }>(
        `${environment.apiUrl}/AreaDishPrices/GetDishNames`,
        request
      )
      .pipe(
        catchError((err) => {
          console.error('Error loading dish name suggestions:', err);
          this.loadingDishNames.set(false);
          return of({ statusCode: 500, isSuccess: false, message: 'Error', data: [] });
        })
      )
      .subscribe((response) => {
        if (response.isSuccess && response.data) {
          // optionally filter by area-based dishNames if we have area-specific list already
          this.dishNames.set(response.data || []);
        } else {
          this.dishNames.set([]);
        }
        this.loadingDishNames.set(false);
      });
  }

  onKitchenNameFilterChange(kitchenName: string) {
    this.kitchenNameFilter.set(kitchenName);
    this.pageIndex.set(1); // Reset to first page when filtering
    this.getAllAreaDishPrices(1);
  }

  onGroupNameFilterChange(groupName: string) {
    this.groupNameFilter.set(groupName);
    this.pageIndex.set(1); // Reset to first page when filtering
    this.getAllAreaDishPrices(1);
  }

  onDishNameDropdownChange(dishName: string) {
    this.selectedDishNameFilter.set(dishName);
    this.pageIndex.set(1); // Reset to first page when filtering
    this.getAllAreaDishPrices(1);
  }

  onStatusFilterChange(status: string) {
    this.selectedStatus.set(status);
    this.pageIndex.set(1); // Reset to first page when filtering
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

    console.log(`Sorting by column: ${column}, direction: ${direction}`);

    return [...items].sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (column) {
        case 'price':
          valueA = a.customPrice;
          valueB = b.customPrice;
          break;
        case 'dish':
        case 'basePrice':
          valueA = a.basePrice || 0;
          valueB = b.basePrice || 0;
          console.log(`Comparing basePrice: ${a.dishName}(${valueA}) vs ${b.dishName}(${valueB})`);
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
    this.selectedDishNameFilter.set('');
    this.dishNames.set([]);
    this.pageIndex.set(1); // Reset to first page when clearing all filters
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

  // Enrich area dish prices with base price from Dishes table
  private async enrichAreaDishPricesWithDishInfo(
    areaDishPrices: AreaDishPrice[]
  ): Promise<AreaDishPriceDisplay[]> {
    if (areaDishPrices.length === 0) {
      return [];
    }

    console.log('Original area dish prices:', areaDishPrices);

    // Get unique dish IDs
    const dishIds = [...new Set(areaDishPrices.map((adp) => adp.dishId))];
    console.log('Unique dish IDs to fetch:', dishIds);

    try {
      // Get all dishes info in one call
      const dishes = await firstValueFrom(this.getAllDishes());
      console.log('All dishes from API:', dishes);

      // Create a map for quick lookup
      const dishMap = new Map(dishes?.map((dish) => [dish.dishId, dish]) || []);
      console.log('Dish map created:', dishMap);

      // Enrich area dish prices with dish info
      const enrichedItems: AreaDishPriceDisplay[] = areaDishPrices.map((adp) => {
        const dish = dishMap.get(adp.dishId);
        const enriched = {
          ...adp,
          basePrice: dish?.basePrice || 0,
          kitchenId: dish?.kitchenId,
          groupId: dish?.groupId,
        };
        console.log(
          `Enriching ${adp.dishId}: basePrice=${dish?.basePrice} -> ${enriched.basePrice}`
        );
        return enriched;
      });

      console.log('Final enriched area dish prices:', enrichedItems);
      return enrichedItems;
    } catch (error) {
      console.error('Error enriching area dish prices:', error);
      // Return original data if enrichment fails
      return areaDishPrices;
    }
  }

  private getAllDishes(): Observable<Dish[]> {
    return this.http.get<Dish[]>(`${environment.apiUrl}/Dishes`).pipe(
      catchError((err) => {
        console.error('Error loading dishes:', err);
        return of([]);
      })
    );
  }

  private getAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(`${environment.apiUrl}/Areas`).pipe(
      catchError((err) => {
        console.error('HTTP Error:', err);
        return of([]);
      })
    );
  }

  private loadDishNamesByArea(areaId: string) {
    this.loadingDishNames.set(true);

    const request = {
      searchString: areaId || '',
      searchName: '',
    };

    this.http
      .post<{
        statusCode: number;
        isSuccess: boolean;
        message: string;
        data: string[];
      }>(`${environment.apiUrl}/AreaDishPrices/GetDishNames`, request)
      .pipe(
        catchError((err) => {
          console.error('Error loading dish names:', err);
          this.loadingDishNames.set(false);
          return of({
            statusCode: 500,
            isSuccess: false,
            message: 'Error loading dish names',
            data: [],
          });
        })
      )
      .subscribe((response) => {
        console.log('Dish names response:', response);
        if (response.isSuccess && response.data) {
          this.dishNames.set(response.data);
        } else {
          console.error('Failed to load dish names:', response.message);
          this.dishNames.set([]);
        }
        this.loadingDishNames.set(false);
      });
  }

  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }

  getSelectedAreaName(): string {
    if (!this.selectedAreaFilter()) return '';
    const area = this.areas().find((a) => a.areaId === this.selectedAreaFilter());
    return area ? area.areaName : '';
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

  // Remove Area Dish Price Methods
  openRemoveModal(dish: AreaDishPriceDisplay) {
    this.removingDish.set(dish);
    this.showRemoveModal.set(true);
  }

  closeRemoveModal() {
    this.showRemoveModal.set(false);
    this.removingDish.set(null);
  }

  removeAreaDishPrice() {
    const dish = this.removingDish();
    if (!dish) return;

    this.removingFood.set(true);

    const request: DeleteAreaDishPriceRequest = {
      id: dish.id,
    };

    this.http
      .post<DeleteAreaDishPriceResponse>(
        `${environment.apiUrl}/AreaDishPrices/DeleteAreaDishPrice`,
        request
      )
      .pipe(
        catchError((err) => {
          console.error('Error deleting area dish price:', err);
          alert('Lỗi xóa món khỏi khu vực: ' + (err.error?.message || err.message));
          return of(null);
        })
      )
      .subscribe({
        next: (response) => {
          if (response) {
            console.log('Area dish price deleted successfully:', response);
            alert(response.message || 'Đã xóa món khỏi khu vực thành công!');
            this.closeRemoveModal();
            // Reload the data
            this.getAllAreaDishPrices(this.pageIndex());
          }
          this.removingFood.set(false);
        },
        error: (err) => {
          console.error('Error deleting area dish price:', err);
          alert('Lỗi xóa món khỏi khu vực!');
          this.removingFood.set(false);
        },
      });
  }

  // Add Dishes Modal Methods
  openAddModal(areaId?: string) {
    if (areaId) {
      // Find and set the area
      const area = this.areas().find((a) => a.areaId === areaId);
      if (area) {
        this.selectedArea.set(area);
        this.selectedAreaFilter.set(areaId);
      }
    }

    if (!this.selectedArea()) {
      alert('Vui lòng chọn khu vực trước');
      return;
    }

    this.resetAddModalData();
    this.loadAvailableDishes(this.selectedArea()!.areaId);
    this.showAddModal.set(true);
  }

  closeAddModal() {
    this.showAddModal.set(false);
    this.resetAddModalData();
  }

  resetAddModalData() {
    this.availableDishes.set([]);
    this.selectedDish.set('');
    this.customPrice.set(0);
    this.availableDishesSearchText.set('');
    // Reset the search subject as well
    this.searchSubject.next('');
  }

  loadAvailableDishes(areaId: string) {
    this.loadingAvailableDishes.set(true);

    const request: GetAvailableDishesRequest = {
      areaId: areaId,
      searchString: this.availableDishesSearchText() || undefined,
    };

    console.log('Loading available dishes with request:', request);

    this.http
      .post<AvailableDishesResponse>(`${environment.apiUrl}/Dishes/GetAvailableDishes`, request)
      .pipe(
        catchError((err) => {
          console.error('Error loading available dishes:', err);
          alert('Lỗi tải danh sách món ăn: ' + (err.error?.message || err.message));
          return of({
            value: {
              statusCode: 500,
              isSuccess: false,
              message: 'Error',
              data: [],
            },
          } as AvailableDishesResponse);
        })
      )
      .subscribe((response) => {
        console.log('Available dishes response:', response);

        // Check if the response is successful
        if (!response.value?.isSuccess) {
          console.error('API returned error:', response.value?.message);
          alert('Lỗi từ server: ' + (response.value?.message || 'Unknown error'));
          this.availableDishes.set([]);
          this.loadingAvailableDishes.set(false);
          return;
        }

        // Extract dishes from the wrapped response
        const dishes = response.value?.data || [];
        console.log('Extracted dishes from response:', dishes);

        // Filter for active dishes only
        const filteredDishes = dishes.filter((dish) => dish.isActive === true);

        console.log('Filtered available dishes:', filteredDishes);

        // Remember current selection
        const currentSelection = this.selectedDish();

        this.availableDishes.set(filteredDishes);

        // Restore selection if the dish is still in the filtered list
        if (currentSelection && filteredDishes.some((dish) => dish.dishId === currentSelection)) {
          this.selectedDish.set(currentSelection);
        } else if (
          currentSelection &&
          !filteredDishes.some((dish) => dish.dishId === currentSelection)
        ) {
          // Clear selection if the previously selected dish is no longer available
          this.selectedDish.set('');
        }

        this.loadingAvailableDishes.set(false);
      });
  }

  selectDish(dishId: string) {
    this.selectedDish.set(dishId);
  }

  getSelectedDishName(): string {
    const dishId = this.selectedDish();
    if (!dishId) return '';

    const dish = this.availableDishes().find((d) => d.dishId === dishId);
    return dish ? dish.dishName : '';
  }

  // Available Dishes Search Methods
  onAvailableDishesSearch() {
    if (!this.selectedArea()) return;
    console.log('Searching available dishes with text:', this.availableDishesSearchText());
    // Use the subject for debounced search
    this.searchSubject.next(this.availableDishesSearchText());
  }

  clearAvailableDishesSearch() {
    this.availableDishesSearchText.set('');
    this.searchSubject.next('');
  }

  addDishesToArea() {
    if (!this.selectedDish()) {
      alert('Vui lòng chọn món ăn');
      return;
    }

    if (this.customPrice() <= 0) {
      alert('Vui lòng nhập giá hợp lệ');
      return;
    }

    if (!this.selectedArea()) {
      alert('Vui lòng chọn khu vực');
      return;
    }

    this.addingDishes.set(true);

    const request: AddDishesToAreaRequest = {
      areaId: this.selectedArea()!.areaId,
      dishIds: [this.selectedDish()], // Convert single dish to array
      customPrice: this.customPrice(),
    };

    this.http
      .post(`${environment.apiUrl}/AreaDishPrices/Add`, request)
      .pipe(
        catchError((err) => {
          console.error('Error adding dishes to area:', err);
          alert('Lỗi thêm món vào khu vực: ' + (err.error?.message || err.message));
          return of(null);
        })
      )
      .subscribe((response) => {
        if (response) {
          console.log('Dishes added successfully:', response);
          alert('Đã thêm món vào khu vực thành công!');
          this.closeAddModal();
          // Reload the data
          this.getAllAreaDishPrices(this.pageIndex());
        }
        this.addingDishes.set(false);
      });
  }
}

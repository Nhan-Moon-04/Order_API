import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Observable, catchError, of, forkJoin, map } from 'rxjs';
import { Area } from '../../model/area.model';
import { AreaDishPrice, AreaDishPriceDisplay } from '../../model/area-dish-price.model';
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

  areas = signal<Area[]>([]);
  selectedArea = signal<Area | null>(null);
  dishPrices = signal<AreaDishPriceDisplay[]>([]);
  loading = signal(false);
  loadingDishes = signal(false);
  error = signal<string | null>(null);
  dishError = signal<string | null>(null);

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
    this.loadAreas();
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

  private loadDishPrices(areaId: string) {
    this.loadingDishes.set(true);
    this.dishError.set(null);

    const payload = { area: areaId };

    // Load area dish prices
    const dishPrices$ = this.http
      .post<AreaDishPrice[]>('https://localhost:7136/api/AreaDishPrices/Prices', payload)
      .pipe(
        catchError((err) => {
          console.error('Error loading dish prices:', err);
          return of([]);
        })
      );

    // Load all dishes to get dish names
    const dishes$ = this.http.get<Dish[]>(`${environment.apiUrl}/Dishes`).pipe(
      catchError((err) => {
        console.error('Error loading dishes:', err);
        return of([]);
      })
    );

    // Combine both data streams
    forkJoin({
      dishPrices: dishPrices$,
      dishes: dishes$,
    }).subscribe({
      next: ({ dishPrices, dishes }) => {
        // Create a map of dishId to dish info for quick lookup
        const dishMap = new Map(dishes.map((dish) => [dish.dishId, dish]));

        // Merge dish prices with dish information
        const enrichedDishPrices: AreaDishPriceDisplay[] = dishPrices.map((price) => {
          const dishInfo = dishMap.get(price.dishId);
          return {
            ...price,
            dishName: dishInfo?.dishName || `Món ăn #${price.dishId}`,
            basePrice: dishInfo?.basePrice,
            kitchenId: dishInfo?.kitchenId,
            groupId: dishInfo?.groupId,
          };
        });

        this.dishPrices.set(enrichedDishPrices);
        this.loadingDishes.set(false);
      },
      error: (err) => {
        this.dishError.set('Failed to load dish prices');
        this.loadingDishes.set(false);
        console.error('Error loading dish prices:', err);
      },
    });
  }

  private location = inject(Location);
  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }

  selectArea(area: Area) {
    console.log('Selected area:', area.areaName);
    this.selectedArea.set(area);
    this.loadDishPrices(area.areaId);
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
      .post('https://localhost:7136/api/AreaDishPrices/update-price', updatePayload)
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
            if (this.selectedArea()) {
              this.loadDishPrices(this.selectedArea()!.areaId);
            }
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
      .post<ChangeQuantityResponse>(
        'https://localhost:7136/api/OrderDetails/change-quantity',
        request
      )
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
      .post<RemoveFoodResponse>('https://localhost:7136/api/OrderDetails/RemoveFood', request)
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

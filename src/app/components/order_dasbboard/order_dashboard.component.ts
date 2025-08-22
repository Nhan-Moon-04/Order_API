import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';

interface OrderDetail {
    orderDetailId: string;
    orderId: string;
    dishId: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    dishName: string;
    kitchenName?: string;
}

interface Order {
    orderId: string;
    orderDate: string;
    isPaid: boolean;
    tableCode: string;
    createdAt: string;
    closedAt?: string;
    tableName: string;
    areaName: string;
    totalAmount: number;
    orderDetails: OrderDetail[];
}

interface Dish {
    dishId: string;
    dishName: string;
    basePrice: number;
    kitchenId: string;
    groupId: string;
    isActive: boolean;
    createdAt: string;
    kitchenName: string;
    groupName: string;
}

interface DishGroup {
    groupName: string;
    dishes: Dish[];
}

@Component({
    selector: 'app-order-dashboard',
    standalone: true,
    imports: [CommonModule, HttpClientModule],
    templateUrl: './order_dashboard.component.html',
    styleUrl: './order_dashboard.component.css'
})
export class OrderDashboardComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private http = inject(HttpClient);

    orderId = signal<string>('');
    order = signal<Order | null>(null);
    dishes = signal<Dish[]>([]);
    dishGroups = signal<DishGroup[]>([]);
    loading = signal(false);
    error = signal<string | null>(null);
    selectedGroup = signal<string>('');

    ngOnInit() {
        this.route.params.subscribe(params => {
            const id = params['orderId'];
            if (id) {
                this.orderId.set(id);
                this.loadOrder(id);
                this.loadDishes();
            }
        });
    }

    private loadOrder(orderId: string) {
        this.loading.set(true);
        this.error.set(null);

        this.http.get<Order>(`https://localhost:7136/api/Orders/${orderId}`).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                this.error.set('Failed to load order');
                return of(null);
            })
        ).subscribe(order => {
            this.order.set(order);
            this.loading.set(false);
        });
    }

    private loadDishes() {
        this.http.get<Dish[]>('https://localhost:7136/api/Dishes').pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of([]);
            })
        ).subscribe(dishes => {
            this.dishes.set(dishes);
            this.groupDishes(dishes);
        });
    }

    private groupDishes(dishes: Dish[]) {
        const groups = dishes.reduce((acc, dish) => {
            const groupName = dish.groupName || 'Khác';
            if (!acc[groupName]) {
                acc[groupName] = [];
            }
            acc[groupName].push(dish);
            return acc;
        }, {} as Record<string, Dish[]>);

        const dishGroups = Object.entries(groups).map(([groupName, dishes]) => ({
            groupName,
            dishes
        }));

        this.dishGroups.set(dishGroups);
    }

    formatCurrency(amount: number): string {
        return new Intl.NumberFormat('vi-VN').format(amount);
    }

    selectGroup(groupName: string) {
        this.selectedGroup.set(groupName);
    }

    addDishToOrder(dish: Dish) {
        // TODO: Implement add dish to order
        console.log('Adding dish to order:', dish);
    }

    goBack() {
        this.router.navigate(['/']);
    }
}
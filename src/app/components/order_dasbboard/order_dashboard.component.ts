import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
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
    tableSessionId: string | null; // Updated to match actual API response
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

interface TableSession {
    id: string;
    sessionId: string;
    tableId: string;
    openAt: string;
    closeAt?: string;
    openedBy?: string;
    closedBy?: string;
    status: number;
    table: {
        tableCode: string;
        tableName: string;
        capacity: number;
        isActive: boolean;
        areaId: string;
        status: string;
        areaName?: string;
    };
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
    private location = inject(Location);
    private http = inject(HttpClient);

    tableId = signal<string>('');
    order = signal<Order | null>(null);
    dishes = signal<Dish[]>([]);
    dishGroups = signal<DishGroup[]>([]);
    loading = signal(false);
    error = signal<string | null>(null);
    selectedGroup = signal<string>('');

    ngOnInit() {
        this.route.params.subscribe(params => {
            const tableId = params['tableId'];
            if (tableId) {
                this.tableId.set(tableId);
                this.loadOrder(tableId);
                this.loadDishes();
            }
        });
    }

    private loadOrder(tableId: string) {
        this.loading.set(true);
        this.error.set(null);

        // API endpoint expects tableId to get the latest order for that table
        this.http.get<Order>(`https://localhost:7136/api/Orders/${tableId}/latest-order`).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                this.error.set('Không thể tải thông tin đơn hàng');
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
        const currentOrder = this.order();
        if (!currentOrder) {
            this.error.set('Không tìm thấy thông tin đơn hàng');
            return;
        }

        const orderDetailRequest = {
            orderId: currentOrder.orderId,
            dishId: dish.dishId,
            quantity: 1
        };

        this.loading.set(true);
        this.error.set(null);

        this.http.post<OrderDetail>('https://localhost:7136/api/OrderDetails', orderDetailRequest, {
            headers: {
                'Content-Type': 'application/json',
                'accept': 'text/plain'
            }
        }).pipe(
            catchError(err => {
                console.error('Error adding dish to order:', err);
                this.error.set('Không thể thêm món ăn vào đơn hàng');
                this.loading.set(false);
                return of(null);
            })
        ).subscribe(orderDetail => {
            if (orderDetail) {
               
                
                // Cập nhật order details trong UI
                const updatedOrder = { ...currentOrder };
                const existingDetailIndex = updatedOrder.orderDetails.findIndex(
                    detail => detail.dishId === dish.dishId
                );

                if (existingDetailIndex >= 0) {
                    // Cập nhật quantity và totalPrice nếu món đã tồn tại
                    updatedOrder.orderDetails[existingDetailIndex] = orderDetail;
                } else {
                    // Thêm món mới vào danh sách
                    updatedOrder.orderDetails.push(orderDetail);
                }

                // Cập nhật tổng tiền
                updatedOrder.totalAmount = updatedOrder.orderDetails.reduce(
                    (total, detail) => total + detail.totalPrice, 0
                );

                this.order.set(updatedOrder);
                this.loading.set(false);

                // Thông báo đã được tắt để tránh spam
                console.log(`Đã thêm ${dish.dishName} vào đơn hàng!`);
            }
        });
    }

    updateDishQuantity(dishId: string, newQuantity: number) {
        const currentOrder = this.order();
        if (!currentOrder) {
            this.error.set('Không tìm thấy thông tin đơn hàng');
            return;
        }

        if (newQuantity <= 0) {
            // Nếu quantity <= 0, xóa món khỏi order
            this.removeDishFromOrder(dishId);
            return;
        }

        const orderDetailRequest = {
            orderId: currentOrder.orderId,
            dishId: dishId,
            quantity: newQuantity
        };

        this.loading.set(true);
        this.error.set(null);

        this.http.post<OrderDetail>('https://localhost:7136/api/OrderDetails', orderDetailRequest, {
            headers: {
                'Content-Type': 'application/json',
                'accept': 'text/plain'
            }
        }).pipe(
            catchError(err => {
                console.error('Error updating dish quantity:', err);
                this.error.set('Không thể cập nhật số lượng món ăn');
                this.loading.set(false);
                return of(null);
            })
        ).subscribe(orderDetail => {
            if (orderDetail) {
             
                
                // Cập nhật order details trong UI
                const updatedOrder = { ...currentOrder };
                const existingDetailIndex = updatedOrder.orderDetails.findIndex(
                    detail => detail.dishId === dishId
                );

                if (existingDetailIndex >= 0) {
                    updatedOrder.orderDetails[existingDetailIndex] = orderDetail;
                }

                // Cập nhật tổng tiền
                updatedOrder.totalAmount = updatedOrder.orderDetails.reduce(
                    (total, detail) => total + detail.totalPrice, 0
                );

                this.order.set(updatedOrder);
                this.loading.set(false);
            }
        });
    }

    removeDishFromOrder(dishId: string) {
        // TODO: Implement remove dish from order if API is available
        // For now, just update UI
        const currentOrder = this.order();
        if (currentOrder) {
            const updatedOrder = { ...currentOrder };
            updatedOrder.orderDetails = updatedOrder.orderDetails.filter(
                detail => detail.dishId !== dishId
            );
            
            // Cập nhật tổng tiền
            updatedOrder.totalAmount = updatedOrder.orderDetails.reduce(
                (total, detail) => total + detail.totalPrice, 0
            );

            this.order.set(updatedOrder);
        }
    }

    closeTable(sessionId: string): Observable<TableSession> {
        const closedBy = "Nhan";
        const url = `https://localhost:7136/api/TableSessions/session/${sessionId}/close`;
        
        return this.http.put<TableSession>(url, JSON.stringify(closedBy), {
            headers: {
                'Content-Type': 'application/json',
                'accept': 'text/plain'
            }
        }).pipe(
            catchError(err => {
                console.error('Error closing table:', err);
                this.error.set('Không thể đóng bàn. Vui lòng thử lại.');
                throw err;
            })
        );
    }

    private getActiveTableSession(tableCode: string): Observable<TableSession | null> {
        // API để lấy active session của table
        return this.http.get<TableSession>(`https://localhost:7136/api/TableSessions/table/${tableCode}/active`).pipe(
            catchError(err => {
                console.error('Error getting active table session:', err);
                return of(null);
            })
        );
    }

    private generateCurrentSessionId(): string {
        // Tạo sessionId theo pattern TS + yyyyMMddHHmmss
        // Giống với pattern trong backend: $"TS{DateTime.Now:yyyyMMddHHmmss}"
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');
        
        return `TS${year}${month}${day}${hours}${minutes}${seconds}`;
    }

    onCloseTable() {
        const currentOrder = this.order();
        if (!currentOrder) {
            this.error.set('Không tìm thấy thông tin đơn hàng');
            return;
        }

       

        this.loading.set(true);
        this.error.set(null);

        // Nếu order có tableSessionId, sử dụng trực tiếp
        if (currentOrder.tableSessionId) {
           
            this.closeTable(currentOrder.tableSessionId).subscribe({
                next: (response) => {
                    this.loading.set(false);
                    alert(`Đã đóng bàn ${response.table.tableName} thành công!`);
                    this.goBack();
                },
                error: (error) => {
                    console.error('Failed to close table:', error);
                    this.loading.set(false);
                }
            });
        } else {
            // Thử lấy active session từ API
            this.getActiveTableSession(currentOrder.tableCode).subscribe({
                next: (session) => {
                    if (session && session.sessionId) {
                       
                        
                        this.closeTable(session.sessionId).subscribe({
                            next: (response) => {
                              
                                this.loading.set(false);
                                alert(`Đã đóng bàn ${response.table.tableName} thành công!`);
                                this.goBack();
                            },
                            error: (error) => {
                                console.error('Failed to close table:', error);
                                this.loading.set(false);
                            }
                        });
                    } else {
                        // Fallback: thử với sessionId hiện tại theo pattern
                        const currentSessionId = this.generateCurrentSessionId();
                      

                        
                        this.closeTable(currentSessionId).subscribe({
                            next: (response) => {
                               
                                this.loading.set(false);
                                alert(`Đã đóng bàn ${response.table.tableName} thành công!`);
                                this.goBack();
                            },
                            error: (error) => {
                                console.error('Failed to close table with generated sessionId:', error);
                                this.loading.set(false);
                                this.error.set('Không thể đóng bàn. SessionId không hợp lệ.');
                            }
                        });
                    }
                },
                error: (error) => {
                    console.error('Failed to get active table session:', error);
                    // Fallback: thử với sessionId hiện tại theo pattern
                    const currentSessionId = this.generateCurrentSessionId();
                  
                    
                    this.closeTable(currentSessionId).subscribe({
                        next: (response) => {
                           
                            this.loading.set(false);
                            alert(`Đã đóng bàn ${response.table.tableName} thành công!`);
                            this.goBack();
                        },
                        error: (error) => {
                            console.error('Failed to close table with generated sessionId:', error);
                            this.loading.set(false);
                            this.error.set('Không thể đóng bàn. Vui lòng kiểm tra lại.');
                        }
                    });
                }
            });
        }
    }

    goBack() {
        this.location.back();
    }
}
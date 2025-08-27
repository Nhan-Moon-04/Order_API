import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { catchError, of } from 'rxjs';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, HttpClientModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
})
export class AdminDashboardComponent implements OnInit {
  private http = inject(HttpClient);

  // Statistics signals
  totalAreas = signal<number>(0);
  totalTables = signal<number>(0);
  totalDishes = signal<number>(0);
  todayOrders = signal<number>(0);

  // Loading states
  loadingStats = signal<boolean>(true);

  ngOnInit() {
    this.loadStatistics();
  }

  private loadStatistics() {
    this.loadingStats.set(true);

    // Load areas count
    this.loadAreasCount();

    // TODO: Add other statistics APIs when available
    // this.loadTablesCount();
    // this.loadDishesCount();
    // this.loadTodayOrdersCount();

    this.loadingStats.set(false);

    this.loadTablesCount();
  }

  private loadAreasCount() {
    this.http
      .get<number>('https://localhost:7136/api/Areas/count')
      .pipe(
        catchError((err) => {
          console.error('Error loading areas count:', err);
          return of(0);
        })
      )
      .subscribe((count) => {
        this.totalAreas.set(count);
      });
  }

  private loadTablesCount() {
    this.http
      .get<number>('https://localhost:7136/api/Tables/count')
      .pipe(
        catchError((err) => {
          console.error('Error loading tables count:', err);
          return of(0);
        })
      )
      .subscribe((count) => {
        this.totalTables.set(count);
      });
  }

  menuItems = [
    {
      title: 'Quản lý Khu vực',
      description: 'Thêm, sửa, xóa khu vực',
      icon: 'fas fa-map-marked-alt',
      route: '/admin/areas',
      color: 'bg-blue-500',
    },
    {
      title: 'Quản lý Bàn',
      description: 'Thêm, sửa, xóa bàn ăn',
      icon: 'fas fa-table',
      route: '/admin/tables',
      color: 'bg-green-500',
    },
    {
      title: 'Quản lý Món ăn',
      description: 'Thêm, sửa, xóa món ăn',
      icon: 'fas fa-utensils',
      route: '/admin/dishes',
      color: 'bg-orange-500',
    },
    {
      title: 'Quản lý Đơn hàng',
      description: 'Xem lịch sử đơn hàng',
      icon: 'fas fa-receipt',
      route: '/admin/orders',
      color: 'bg-purple-500',
    },

    {
      title: 'Quản lý Nhân viên',
      description: 'Thêm, sửa, xóa nhân viên',
      icon: 'fas fa-users-cog',
      route: '/admin/staff',
      color: 'bg-red-500',
    },
    {
      title: 'Báo cáo Thống kê',
      description: 'Xem báo cáo doanh thu',
      icon: 'fas fa-chart-line',
      route: '/admin/reports',
      color: 'bg-teal-500',
    },
    {
      title: 'Quản lý giá tiền khu vực',
      description: 'Thêm, sửa, xóa giá tiền khu vực',
      icon: 'fas fa-tags',
      route: '/admin/area-prices',
      color: 'bg-yellow-500',
    },
  ];
}

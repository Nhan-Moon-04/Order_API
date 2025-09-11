import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { Table } from '../../model/table.model';
import { Area } from '../../model/area.model';
import { TableFilter } from '../../model/table-filter.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-tables',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './tables.component.html',
  styleUrl: './tables.component.css',
})
export class TablesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);
  private http = inject(HttpClient);

  areaId = signal<string>('');
  area = signal<Area | null>(null);
  tables$ = signal<Table[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  selectedTable = signal<any | null>(null);
  showConfirmModal = signal(false);

  ngOnInit() {
    this.route.params.subscribe((params) => {
      const id = params['areaId'];
      if (id) {
        this.areaId.set(id);
        this.loadTables(id);
      }
    });
  }

  private loadTables(areaId: string) {
    this.loading.set(true);
    this.error.set(null);

    this.getTablesByArea(areaId).subscribe({
      next: (tables) => {
        this.tables$.set(tables);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load tables');
        this.loading.set(false);
        console.error('Error loading tables:', err);
      },
    });
  }

  private getTablesByArea(areaId: string): Observable<Table[]> {
    const ViewTablePayload: TableFilter = {
      areaId: areaId,
      isActive: true,
    };

    return this.http.post<Table[]>(`${environment.apiUrl}/Tables/ViewTable`, ViewTablePayload).pipe(
      catchError((err) => {
        console.error('HTTP Error:', err);
        return of([]);
      })
    );
  }

  private getAreaById(areaId: string): Observable<Area | null> {
    return this.http.get<Area>(`${environment.apiUrl}/Areas/${areaId}`).pipe(
      catchError((err) => {
        console.error('HTTP Error:', err);
        return of(null);
      })
    );
  }

  goBack() {
    this.location.back();
  }

  goToOrderDashboard(table: any) {
    // Kiểm tra xem có thể xem details không
    if (!this.canViewDetails(table.status)) {
      alert('Không thể xem chi tiết vì bàn chưa có đơn hàng!');
      return;
    }

    this.router.navigate(['/order-dashboard', table.tableCode]);
  }

  TableStatus = {
    Available: 0,
    Occupied: 1,
    Reserved: 2,
    Closed: 3,
    Cleaning: 4,
  };

  getStatusNumber(status: string | number): number {
    if (typeof status === 'number') return status;
    switch (status) {
      case 'Available':
        return 0;
      case 'Occupied':
        return 1;
      case 'Reserved':
        return 2;
      case 'Closed':
        return 3;
      case 'Cleaning':
        return 4;
      default:
        return -1;
    }
  }

  getStatusString(status: string | number): string {
    if (typeof status === 'string') return status;
    switch (status) {
      case 0:
        return 'Available';
      case 1:
        return 'Occupied';
      case 2:
        return 'Reserved';
      case 3:
        return 'Closed';
      case 4:
        return 'Cleaning';
      default:
        return 'Unknown';
    }
  }

  getStatusText(status: string | number): string {
    return this.getStatusString(status);
  }

  getStatusBadgeClass(status: string | number): string {
    const statusNum = this.getStatusNumber(status);
    switch (statusNum) {
      case this.TableStatus.Available:
        return 'available';
      case this.TableStatus.Occupied:
        return 'occupied';
      default:
        return 'available';
    }
  }

  getTableCardClass(status: string | number): string {
    const statusNum = this.getStatusNumber(status);
    switch (statusNum) {
      case this.TableStatus.Available:
        return 'available';
      case this.TableStatus.Occupied:
        return 'occupied';
      default:
        return 'available';
    }
  }

  canBookTable(status: string | number): boolean {
    const statusNum = this.getStatusNumber(status);
    return statusNum === this.TableStatus.Available;
  }

  canViewDetails(status: string | number): boolean {
    const statusNum = this.getStatusNumber(status);
    // Chỉ có thể xem details khi bàn đang có khách (Occupied) hoặc Reserved
    // Không thể xem details khi Available vì chưa có đơn hàng
    return statusNum === this.TableStatus.Occupied || statusNum === this.TableStatus.Reserved;
  }

  getBookButtonClass(status: string | number): string {
    const base =
      'flex-1 px-3 py-2 rounded-lg text-sm font-medium transition-all duration-300 relative z-10';
    if (this.canBookTable(status)) {
      return `${base} bg-orange-600 transition-colors`;
    } else {
      return `${base} bg-gray-400 text-gray-200 cursor-not-allowed`;
    }
  }

  getDetailsButtonClass(status: string | number): string {
    const base =
      'flex-1 border-2 px-3 py-2 rounded-lg text-sm font-medium transition-all duration-300';
    if (this.canViewDetails(status)) {
      return `${base} border-orange-500 text-orange-500 hover:bg-orange-500 hover:text-white`;
    } else {
      return `${base} border-gray-300 text-gray-400 cursor-not-allowed`;
    }
  }

  confirmBooking(table: any): void {
    if (this.canBookTable(table.status)) {
      this.selectedTable.set(table);
      this.showConfirmModal.set(true);
    }
  }

  confirmBookingAction() {
    const table = this.selectedTable();
    if (!table) return;

    const url = `${environment.apiUrl}/TableSessions/table/${table.tableCode}/open`;

    console.log('Opening table with URL:', url);

    // Payload cần stringify để đúng format JSON string
    const payload = JSON.stringify('NHAN');

    // Thêm headers để đảm bảo Content-Type đúng
    const headers = {
      'Content-Type': 'application/json',
    };

    console.log('Payload being sent:', payload);

    this.http.post<any>(url, payload, { headers }).subscribe({
      next: (res) => {
        console.log(`Đã mở bàn ${table.tableName} thành công!`, res);
        this.router.navigate(['/order-dashboard', table.tableCode]);
        this.closeModal();
        // Reload lại danh sách bàn để cập nhật trạng thái
        this.loadTables(this.areaId());
      },
      error: (err) => {
        console.error('Lỗi mở bàn:', err);
        alert(`Lỗi mở bàn ${table.tableName}: ${err.error?.message || err.message}`);
        this.closeModal();
      },
    });
  }

  closeModal() {
    this.showConfirmModal.set(false);
    this.selectedTable.set(null);
  }

  getTableStatusClass(isActive: boolean): string {
    return isActive ? 'active' : 'inactive';
  }
}

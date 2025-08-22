import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
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
    styleUrl: './tables.component.css'
})
export class TablesComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private http = inject(HttpClient);

    areaId = signal<string>('');
    area = signal<Area | null>(null);
    tables$ = signal<Table[]>([]);
    loading = signal(false);
    error = signal<string | null>(null);

    selectedTable = signal<any | null>(null);
    showConfirmModal = signal(false);

    ngOnInit() {
        this.route.params.subscribe(params => {
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
            }
        });
    }

    private getTablesByArea(areaId: string): Observable<Table[]> {
        const ViewTablePayload: TableFilter = {
            areaId: areaId,
            isActive: true
        };


        return this.http.post<Table[]>('https://localhost:7136/api/Tables/ViewTable', ViewTablePayload).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of([]);
            })
        );
    }

    private getAreaById(areaId: string): Observable<Area | null> {
        return this.http.get<Area>(`${environment.apiUrl}/Areas/${areaId}`).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of(null);
            })
        );
    }


    goBack() {
        this.router.navigate(['/']);
    }

    TableStatus = {
        Available: 0,
        Occupied: 1,
        Reserved: 2,
        Closed: 3,
        Cleaning: 4
    };

    getStatusNumber(status: string | number): number {
        if (typeof status === 'number') return status;
        switch (status) {
            case "Available": return 0;
            case "Occupied": return 1;
            case "Reserved": return 2;
            case "Closed": return 3;
            case "Cleaning": return 4;
            default: return -1;
        }
    }

    getStatusString(status: string | number): string {
        if (typeof status === 'string') return status;
        switch (status) {
            case 0: return "Available";
            case 1: return "Occupied";
            case 2: return "Reserved";
            case 3: return "Closed";
            case 4: return "Cleaning";
            default: return "Unknown";
        }
    }

    getStatusText(status: string | number): string {
        return this.getStatusString(status);
    }

    getStatusBadgeClass(status: string | number): string {
        const statusNum = this.getStatusNumber(status);
        const base = 'text-xs px-2 py-1 rounded-full';
        switch (statusNum) {
            case this.TableStatus.Available: return `${base} bg-green-100 text-green-800`;
            case this.TableStatus.Occupied: return `${base} bg-red-100 text-red-800`;
            case this.TableStatus.Reserved: return `${base} bg-blue-100 text-blue-800`;
            case this.TableStatus.Closed: return `${base} bg-gray-100 text-gray-800`;
            case this.TableStatus.Cleaning: return `${base} bg-yellow-100 text-yellow-800`;
            default: return `${base} bg-gray-100 text-gray-800`;
        }
    }

    getTableCardClass(status: string | number): string {
        const statusNum = this.getStatusNumber(status);
        let base = 'bg-white rounded-xl shadow-md hover:shadow-lg transition-all duration-300 transform hover:-translate-y-1 p-4';
        switch (statusNum) {
            case this.TableStatus.Available:
                return `${base} border-l-4 border-green-500`;
            case this.TableStatus.Occupied:
                return `${base} border-l-4 border-red-500`;
            case this.TableStatus.Reserved:
                return `${base} border-l-4 border-blue-500`;
            case this.TableStatus.Closed:
                return `${base} border-l-4 border-gray-500 opacity-75`;
            case this.TableStatus.Cleaning:
                return `${base} border-l-4 border-yellow-500`;
            default:
                return `${base} border-l-4 border-orange-500`;
        }
    }

    canBookTable(status: string | number): boolean {
        const statusNum = this.getStatusNumber(status);
        return statusNum === this.TableStatus.Available;
    }

    getBookButtonClass(status: string | number): string {
        const base = 'flex-1 px-3 py-2 rounded-lg text-sm font-medium transition-all duration-300 relative z-10';
        if (this.canBookTable(status)) {
            return `${base} bg-orange-600 transition-colors`;
        } else {
            return `${base} bg-gray-400 text-gray-200 cursor-not-allowed`;
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

        const url = `https://localhost:7136/api/Tables/${table.tableCode}/open?areaId=${this.areaId()}&openedBy=nhan`;

        console.log('Opening table with URL:', url);

        this.http.post<any>(url, {}).subscribe({
            next: (res) => {
                console.log(`Đã mở bàn ${table.tableName} thành công!`, res);
                alert(`Đã mở bàn ${table.tableName} thành công!`);
                this.closeModal();
                // Reload lại danh sách bàn để cập nhật trạng thái
                this.loadTables(this.areaId());
            },
            error: (err) => {
                console.error("Lỗi mở bàn:", err);
                alert(`Lỗi mở bàn ${table.tableName}: ${err.error?.message || err.message}`);
                this.closeModal();
            }
        });
    }


    closeModal() {
        this.showConfirmModal.set(false);
        this.selectedTable.set(null);
    }

    getTableStatusClass(isActive: boolean): string {
        return isActive ? 'text-green-600 font-semibold' : 'text-red-500 font-semibold';
    }
}

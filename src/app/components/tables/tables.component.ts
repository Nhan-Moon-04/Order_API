import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpClientModule, HttpParams } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { Table } from '../../model/table.model';
import { Area } from '../../model/area.model';
import { TableFilter, TableQueryParams } from '../../model/table-filter.model';
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

    ngOnInit() {
        this.route.params.subscribe(params => {
            const id = params['areaId'];
            console.log('Route areaId:', id); // Debug log
            if (id) {
                this.areaId.set(id);
                this.loadTables(id);
                // this.loadAreaInfo(id); // Remove this line if you don't need area info
            }
        });
    }

    private loadTables(areaId: string) {
        this.loading.set(true);
        this.error.set(null);

        console.log('Loading tables for areaId:', areaId); // Debug log

        this.getTablesByArea(areaId).subscribe({
            next: (tables) => {
                console.log('Tables received:', tables); // Debug log
                this.tables$.set(tables);
                this.loading.set(false);
            },
            error: (err) => {
                console.log('Error loading tables:', err); // Debug log
                this.error.set('Failed to load tables');
                this.loading.set(false);
                console.error('Error loading tables:', err);
            }
        });
    }

    private loadAreaInfo(areaId: string) {
        this.getAreaById(areaId).subscribe({
            next: (area) => {
                this.area.set(area);
            },
            error: (err) => {
                console.error('Error loading area info:', err);
            }
        });
    }

    private getTablesByArea(areaId: string): Observable<Table[]> {
        const ViewTablePayload: TableFilter = {
            areaId: areaId,
            isActive: true
        };

        console.log('POST payload:', ViewTablePayload);
        console.log('API URL:', `${environment.apiUrl}/Tables/ViewTable`);

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

    getTableStatusClass(isActive: boolean): string {
        return isActive ? 'text-green-600 font-semibold' : 'text-red-500 font-semibold';
    }

    getTableCardClass(): string {
        return 'bg-white rounded-xl shadow-md hover:shadow-lg transition-all duration-300 transform hover:-translate-y-1 p-4 border-l-4 border-orange-500';
    }
}
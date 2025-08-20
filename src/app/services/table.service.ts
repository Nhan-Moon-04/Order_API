import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { Table } from '../model/table.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class TableService {
    constructor(private http: HttpClient) { }

    getTablesByAreaId(areaId: string): Observable<Table[]> {
        const payload = {
            areaId: areaId,
            isActive: true
        };

        return this.http.post<Table[]>(`${environment.apiUrl}/Tables/ViewTable`, payload).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of([]);
            })
        );
    }
}

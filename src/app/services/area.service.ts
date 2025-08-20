import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { Area } from '../model/area.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AreaService {
    constructor(private http: HttpClient) { }

    getAreas(): Observable<Area[]> {
        return this.http.get<Area[]>(`${environment.apiUrl}/Areas`).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of([]);
            })
        );
    }

    getAreaById(areaId: string): Observable<Area | null> {
        return this.http.get<Area>(`${environment.apiUrl}/Areas/${areaId}`).pipe(
            catchError(err => {
                console.error('HTTP Error:', err);
                return of(null);
            })
        );
    }
}

import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Observable, catchError, of, forkJoin, map } from 'rxjs';
import { Area } from '../../model/area.model';
import { AreaDishPrice, AreaDishPriceDisplay } from '../../model/area-dish-price.model';
import { Table } from '../../model/table.model';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';

@Component({
  selector: 'app-tables-areas',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './tables-areas.component.html',
  styleUrl: './tables-areas.component.css',
})
export class TablesAreasComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  areas = signal<Area[]>([]);
  tables = signal<Table[]>([]);
  selectedArea = signal<Area | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  highlightedTable = signal<string | null>(null);

  ngOnInit() {
    this.loadAreas();
    this.loadTables();
  }

  private loadAreas() {
    this.loading.set(true);
    this.error.set(null);
    this.http
      .get<Area[]>(`${environment.apiUrl}/areas`)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to load areas.');
          return of([]);
        })
      )
      .subscribe((areas) => {
        this.areas.set(areas);
        this.loading.set(false);
      });
  }

  private loadTables() {
    this.loading.set(true);
    this.error.set(null);
    this.http
      .get<Table[]>(`${environment.apiUrl}/tables`)
      .pipe(
        catchError((err) => {
          this.error.set('Failed to load tables.');
          return of([]);
        })
      )
      .subscribe((tables) => {
        this.tables.set(tables);
        this.loading.set(false);
      });
  }

  moveTableUp(table: Table) {
    this.moveTable(table.tableCode, 'up');
  }

  moveTableDown(table: Table) {
    this.moveTable(table.tableCode, 'down');
  }

  private moveTable(tableCode: string, direction: 'up' | 'down') {
    this.loading.set(true);
    this.error.set(null);
    this.highlightedTable.set(tableCode); // Highlight table being moved

    // Create payload with tableCode first
    const payload: any = {};
    payload.tableCode = tableCode;
    payload.direction = direction;

    this.http
      .post<Table[]>(`${environment.apiUrl}/Tables/Move`, payload)
      .pipe(
        catchError((err) => {
          this.error.set(`Failed to move table ${direction}.`);
          return of([]);
        })
      )
      .subscribe((updatedTables) => {
        if (updatedTables && updatedTables.length > 0) {
          // Reload tables to get updated sort order
          this.loadTables();
        }
        this.loading.set(false);
        this.highlightedTable.set(null); // Remove highlight after move
      });
  }

  selectArea(area: Area) {
    this.selectedArea.set(area);
  }

  getTablesForArea(areaId: string): Table[] {
    return this.tables().filter((table) => table.areaId === areaId);
  }

  private location = inject(Location);
  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }
}

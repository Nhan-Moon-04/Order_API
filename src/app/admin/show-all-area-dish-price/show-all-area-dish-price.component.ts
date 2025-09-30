import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { Area } from '../../model/area.model';
import {
  AreaDishPrice,
  AreaDishPriceSearchRequest,
  PagedResponse,
} from '../../model/area-dish-price.model';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';

@Component({
  selector: 'app-show-all-area-dish-price',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './show-all-area-dish-price.component.html',
  styleUrl: './show-all-area-dish-price.component.css',
})
export class ShowAllAreaDishPriceComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private location = inject(Location);

  // Areas data
  areas = signal<Area[]>([]);
  selectedArea = signal<Area | null>(null);
  loadingAreas = signal(false);

  // Area dish prices data
  areaDishPrices = signal<AreaDishPrice[]>([]);
  loadingDishes = signal(false);

  // Pagination
  pageIndex = signal(1);
  pageSize = signal(20);
  totalRecords = signal(0);
  totalPages = signal(0);
  dropdownValues: number[] = [5, 10, 15, 20, 25, 50, 100, 200, 500];
  selectedPageSize = signal<number>(20);

  // Error handling
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadAreas();
  }

  private loadAreas() {
    this.loadingAreas.set(true);
    this.error.set(null);

    this.getAreas().subscribe({
      next: (areas) => {
        this.areas.set(areas);
        this.loadingAreas.set(false);

        // Auto-select first area if available
        if (areas.length > 0) {
          this.selectArea(areas[0]);
        }
      },
      error: (err) => {
        this.error.set('Không thể tải danh sách khu vực');
        this.loadingAreas.set(false);
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

  selectArea(area: Area) {
    this.selectedArea.set(area);
    this.pageIndex.set(1); // Reset to first page
    this.loadAreaDishPrices();
  }

  private loadAreaDishPrices() {
    const area = this.selectedArea();
    if (!area) return;

    this.loadingDishes.set(true);
    this.error.set(null);

    const searchRequest: AreaDishPriceSearchRequest = {
      searchString: '',
      pageIndex: this.pageIndex(),
      pageSize: this.selectedPageSize(),
      isActive: 1, // 1 for active dishes
      areaId: area.areaId,
    };

    this.http
      .post<PagedResponse<AreaDishPrice>>(
        `${environment.apiUrl}/AreaDishPrices/Search`,
        searchRequest
      )
      .pipe(
        catchError((err) => {
          console.error('Error loading area dish prices:', err);
          this.error.set('Không thể tải danh sách món ăn');
          this.loadingDishes.set(false);
          return of({
            items: [],
            totalRecords: 0,
            pageIndex: 1,
            pageSize: this.selectedPageSize(),
            totalPages: 0,
          } as PagedResponse<AreaDishPrice>);
        })
      )
      .subscribe((response) => {
        this.areaDishPrices.set(response.items || []);
        this.totalRecords.set(response.totalRecords || 0);
        this.calculateTotalPages();
        this.loadingDishes.set(false);
      });
  }

  private calculateTotalPages() {
    const total = Math.ceil(this.totalRecords() / this.selectedPageSize());
    // Ensure at least 1 page if there are records, 0 if no records
    this.totalPages.set(this.totalRecords() > 0 ? Math.max(total, 1) : 0);
  }

  // Get page numbers for pagination display
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const total = this.totalPages();
    const current = this.pageIndex();

    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }

  // Page size change handler
  onPageSizeChange(newSize: number) {
    this.selectedPageSize.set(newSize);
    this.pageIndex.set(1); // Reset to first page
    this.loadAreaDishPrices();
  }

  onPageChange(newPage: number) {
    if (newPage >= 1 && newPage <= this.totalPages()) {
      this.pageIndex.set(newPage);
      this.loadAreaDishPrices();
    }
  }

  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    } else {
      this.router.navigate(['/admin']);
    }
  }
}

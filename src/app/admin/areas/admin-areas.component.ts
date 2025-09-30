import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { Area } from '../../model/area.model';
import { environment } from '../../../environments/environment';
import { Location } from '@angular/common';

@Component({
  selector: 'app-admin-areas',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './admin-areas.component.html',
  styleUrl: './admin-areas.component.css',
})
export class AdminAreasComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);

  areas = signal<Area[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showAddModal = signal(false);
  showEditModal = signal(false);
  selectedArea = signal<Area | null>(null);

  // Form data
  newArea = {
    areaId: '',
    areaName: '',
    description: '',
    isActive: true,
  };

  editArea = {
    areaId: '',
    areaName: '',
    description: '',
    isActive: true,
  };

  ngOnInit() {
    this.loadAreas();
  }

  private loadAreas() {
    this.loading.set(true);
    this.error.set(null);

    this.getAreas().subscribe({
      next: (areas) => {
        this.areas.set(areas);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Không thể tải danh sách khu vực');
        this.loading.set(false);
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

  openAddModal() {
    this.newArea = {
      areaId: '',
      areaName: '',
      description: '',
      isActive: true,
    };
    this.showAddModal.set(true);
  }

  closeAddModal() {
    this.showAddModal.set(false);
  }

  openEditModal(area: Area) {
    this.editArea = {
      areaId: area.areaId,
      areaName: area.areaName,
      description: area.description || '',
      isActive: area.isActive,
    };
    this.selectedArea.set(area);
    this.showEditModal.set(true);
  }

  closeEditModal() {
    this.showEditModal.set(false);
    this.selectedArea.set(null);
  }

  addArea() {
    if (!this.newArea.areaName.trim()) {
      this.error.set('Tên khu vực không được để trống');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    // Create payload matching the API specification
    const createAreaPayload = {
      areaId: this.newArea.areaId.trim() || '', // Use custom areaId or let backend generate
      areaName: this.newArea.areaName,
      description: this.newArea.description || '',
      isActive: this.newArea.isActive,
      createdAt: new Date().toISOString(),
    };

    this.http
      .post<Area>(`${environment.apiUrl}/Areas/CreateArea`, createAreaPayload)
      .pipe(
        catchError((err) => {
          console.error('Error adding area:', err);
          this.error.set('Không thể thêm khu vực mới');
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe((area) => {
        if (area) {
          this.loadAreas(); // Reload list
          this.closeAddModal();
          alert('Thêm khu vực thành công!');
        }
        this.loading.set(false);
      });
  }

  updateArea() {
    if (!this.editArea.areaName.trim()) {
      this.error.set('Tên khu vực không được để trống');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    // Create payload matching the API specification
    const updateAreaPayload = {
      areaId: this.editArea.areaId,
      name: this.editArea.areaName, // API expects 'name' instead of 'areaName'
      description: this.editArea.description || '',
      isActive: this.editArea.isActive,
    };

    this.http
      .post<Area>(`${environment.apiUrl}/Areas/UpdateArea`, updateAreaPayload)
      .pipe(
        catchError((err) => {
          console.error('Error updating area:', err);
          this.error.set('Không thể cập nhật khu vực');
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe((area) => {
        if (area) {
          this.loadAreas(); // Reload list
          this.closeEditModal();
          alert('Cập nhật khu vực thành công!');
        }
        this.loading.set(false);
      });
  }

  deleteArea(area: Area) {
    if (!confirm(`Bạn có chắc muốn xóa khu vực "${area.areaName}"?`)) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    // Send areaId as JSON string in request body with proper headers
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    };

    this.http
      .post(`${environment.apiUrl}/Areas/DeleteArea`, JSON.stringify(area.areaId), httpOptions)
      .pipe(
        catchError((err) => {
          console.error('Error deleting area:', err);
          this.error.set('Không thể xóa khu vực');
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe(() => {
        this.loadAreas(); // Reload list
        this.loading.set(false);
        alert('Xóa khu vực thành công!');
      });
  }

  getStatusClass(isActive: boolean): string {
    return isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Hoạt động' : 'Không hoạt động';
  }

  updateActive(areaId: string, isActive: boolean) {
    this.loading.set(true);
    this.error.set(null);

    const payload = {
      id: areaId,
      isActive: isActive,
    };

    this.http
      .post<Area>(`${environment.apiUrl}/Areas/update-active`, payload)
      .pipe(
        catchError((err) => {
          this.error.set(`Không thể cập nhật trạng thái khu vực.`);
          this.loading.set(false);
          return of(null);
        })
      )
      .subscribe((updatedArea) => {
        if (updatedArea) {
          // Reload areas to get updated data
          this.loadAreas();
        }
        this.loading.set(false);
      });
  }
  private location = inject(Location);
  goBack() {
    if (window.history.length > 1) {
      this.location.back();
    }
  }

  editDishes(area: Area) {
    this.router.navigate(['/admin/area-prices', area.areaId]);
  }
}

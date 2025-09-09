import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
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

  areas = signal<Area[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showAddModal = signal(false);
  showEditModal = signal(false);
  selectedArea = signal<Area | null>(null);

  // Form data
  newArea = {
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

    this.http
      .post<Area>(`${environment.apiUrl}/Areas`, this.newArea)
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

    this.http
      .put<Area>(`${environment.apiUrl}/Areas/${this.editArea.areaId}`, this.editArea)
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

    this.http
      .delete(`${environment.apiUrl}/Areas/${area.areaId}`)
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
      .post<Area>(`https://localhost:7136/api/Areas/update-active`, payload)
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
}

import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { Observable, catchError, of } from 'rxjs';
import { Area } from './model/area.model';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('restaurant-app');

  private http = inject(HttpClient);
  area$ = signal<Area[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadAreas();
  }

  private loadAreas() {
    this.loading.set(true);
    this.error.set(null);

    this.getAreas().subscribe({
      next: (areas) => {
        this.area$.set(areas);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load areas');
        this.loading.set(false);
        console.error('Error loading areas:', err);
      }
    });
  }

  private getAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(`${environment.apiUrl}/Areas`).pipe(
      catchError(err => {
        console.error('HTTP Error:', err);
        return of([]);
      })
    );
  }

  // Helper methods for template
  getStatusClass(isActive: boolean): string {
    return isActive ? 'text-green-600 font-semibold' : 'text-red-500 font-semibold';
  }

  getCardClass(): string {
    return 'bg-white rounded-2xl shadow-lg hover:shadow-2xl transition-all duration-300 transform hover:-translate-y-2 overflow-hidden h-full';
  }

  getButtonClass(isPrimary: boolean, isDisabled: boolean): string {
    const baseClass = 'flex-1 rounded-full font-semibold px-4 py-2 text-sm transition-all duration-300';

    if (isPrimary) {
      return isDisabled
        ? `${baseClass} bg-gradient-to-r from-orange-300 to-red-300 text-white opacity-60 cursor-not-allowed`
        : `${baseClass} bg-gradient-to-r from-orange-500 to-red-500 text-white hover:from-red-500 hover:to-orange-500 hover:transform hover:-translate-y-1 hover:shadow-lg`;
    } else {
      return `${baseClass} border-2 border-orange-500 text-orange-500 bg-transparent hover:bg-orange-500 hover:text-white hover:transform hover:-translate-y-1`;
    }
  }
}

import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Restaurant {
    id: number;
    name: string;
    cuisine: string;
    rating: number;
    image: string;
    description: string;
    address: string;
    phone: string;
    price: string;
}

@Component({
    selector: 'app-restaurant-details',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './restaurant-details.component.html',
    styleUrl: './restaurant-details.component.css'
})
export class RestaurantDetailsComponent {
    restaurants = signal<Restaurant[]>([
        {
            id: 1,
            name: 'The Golden Spoon',
            cuisine: 'Italian',
            rating: 4.8,
            image: 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?ixlib=rb-4.0.3&w=500',
            description: 'Authentic Italian cuisine with a modern twist. Experience the finest pasta and wood-fired pizza.',
            address: '123 Main Street, Downtown',
            phone: '+1 (555) 123-4567',
            price: '$$$'
        },
        {
            id: 2,
            name: 'Sakura Sushi',
            cuisine: 'Japanese',
            rating: 4.9,
            image: 'https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?ixlib=rb-4.0.3&w=500',
            description: 'Fresh sushi and traditional Japanese dishes prepared by master chefs.',
            address: '456 Cherry Blossom Ave',
            phone: '+1 (555) 987-6543',
            price: '$$$$'
        },
        {
            id: 3,
            name: 'Farm to Table Bistro',
            cuisine: 'American',
            rating: 4.6,
            image: 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?ixlib=rb-4.0.3&w=500',
            description: 'Local ingredients, seasonal menu, and sustainable dining experience.',
            address: '789 Green Valley Road',
            phone: '+1 (555) 456-7890',
            price: '$$'
        }
    ]);

    getStarArray(rating: number): number[] {
        return Array(Math.floor(rating)).fill(0);
    }

    hasHalfStar(rating: number): boolean {
        return rating % 1 !== 0;
    }
}

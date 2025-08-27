import { Routes } from '@angular/router';
import { TablesComponent } from './components/tables/tables.component';
import { HomeComponent } from './components/home/home.component';
import { OrderDashboardComponent } from './components/order_dasbboard/order_dashboard.component';
import { AdminDashboardComponent } from './admin/dashboard/admin-dashboard.component';
import { AdminAreasComponent } from './admin/areas/admin-areas.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'areas/:areaId/tables', component: TablesComponent },
    { path: 'order-dashboard/:tableId', component: OrderDashboardComponent },
    
    // Admin routes
    { path: 'admin', component: AdminDashboardComponent },
    { path: 'admin/areas', component: AdminAreasComponent },
    
    { path: '**', redirectTo: '' }
];

import { Routes } from '@angular/router';
import { TablesComponent } from './components/tables/tables.component';
import { HomeComponent } from './components/home/home.component';
import { OrderDashboardComponent } from './components/order_dasbboard/order_dashboard.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'areas/:areaId/tables', component: TablesComponent },
    { path: 'order-dashboard/:orderId', component: OrderDashboardComponent },
    { path: '**', redirectTo: '' }
];

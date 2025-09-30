import { Routes } from '@angular/router';
import { TablesComponent } from './components/tables/tables.component';
import { HomeComponent } from './components/home/home.component';
import { OrderDashboardComponent } from './components/order_dasbboard/order_dashboard.component';
import { AdminDashboardComponent } from './admin/dashboard/admin-dashboard.component';
import { AdminAreasComponent } from './admin/areas/admin-areas.component';
import { AreaPricesComponent } from './admin/area-prices/area-prices.component';
import { TablesAreasComponent } from './admin/tables-areas/tables-areas.compontent';
import { DishesComponent } from './admin/dishes/dishes.component';
import { ShowAllAreaDishPriceComponent } from './admin/show-all-area-dish-price/show-all-area-dish-price.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'areas/:areaId/tables', component: TablesComponent },
  { path: 'order-dashboard/:tableId', component: OrderDashboardComponent },

  // Admin routes
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'admin/areas', component: AdminAreasComponent },
  { path: 'admin/area-prices', component: AreaPricesComponent },
  { path: 'admin/area-prices/:areaId', component: AreaPricesComponent },
  { path: 'admin/show-all-area-dish-price', component: ShowAllAreaDishPriceComponent },
  { path: 'admin/tables-areas', component: TablesAreasComponent },
  { path: 'admin/dishes', component: DishesComponent },
  { path: '**', redirectTo: '' },
];

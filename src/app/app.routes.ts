import { Routes } from '@angular/router';
import { TablesComponent } from './components/tables/tables.component';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'areas/:areaId/tables', component: TablesComponent },
    { path: '**', redirectTo: '' }
];

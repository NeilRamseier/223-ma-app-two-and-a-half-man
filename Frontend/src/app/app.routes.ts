import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LedgerComponent } from './components/ledger/ledger.component';
import { AuthGuard } from './guards/auth.guard';
import { BookingComponent } from './components/booking/booking.component';
import { CreateComponent } from './components/ledger/create/create.component';

export const routes: Routes = [
    {
        path: 'ledgers',
        component: LedgerComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'login',
        component: LoginComponent,
    },
    {
        path: 'create',
        component: CreateComponent,
    },
    {
        path: '',
        redirectTo: '/login',
        pathMatch: 'full',
    },
    {
        path: 'booking',
        component: BookingComponent,
        canActivate: [AuthGuard]
    }
];

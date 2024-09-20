import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { authGuard } from './guards/auth.guard';
import { ListPdfComponent } from './pages/pdf/list-pdf/list-pdf.component';

export const routes: Routes = [
   { path: '', redirectTo: 'login', pathMatch: 'full' },

   { path: 'login', component: LoginComponent },
   
   { path: 'register', component: RegisterComponent },

   { path: 'documents', component: ListPdfComponent, canActivate: [authGuard] },

   { path: '**', redirectTo: 'login' }
];

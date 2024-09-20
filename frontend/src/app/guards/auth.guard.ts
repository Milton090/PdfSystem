import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AlertService } from '../services/alert.service';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const alert = inject(AlertService);
  const token = localStorage.getItem('token');

  if (!token) {
    router.navigate(['/login']);
    alert.warning('Sesion caducada', 'Su sesion ha caducado, por favor inicie sesion nuevamente');
    return false;
  } else {
    return true;
  }
};

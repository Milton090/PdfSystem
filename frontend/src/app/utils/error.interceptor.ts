import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AlertService } from '../services/alert.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const alertService = inject(AlertService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ha ocurrido un error inesperado!';

      if (error.status === 401) {
        alertService.error('Unauthorized', 'You are not authorized to access this resource. Redirecting to login...');
        router.navigate(['/login']);
      } else {
        if (!error.error.msg) {
          errorMessage = `Error: ${error.error.message}`;
        } else {
          errorMessage = error.error.msg;
        }
        alertService.error('Error', errorMessage);
      }
      return throwError(() => errorMessage);
    })
  );
};

import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs';

import { ToastService } from '../services/toast-service';
import { NavigationExtras, Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch(error.status) {

          case 400:
            if (error.error.errors) {
              const modelStateErrors : string[] = []
              for (const key in error.error.errors) {
                modelStateErrors.push(error.error.errors[key]);
              }
              throw modelStateErrors.flat();
            } else {
              toastService.error("Bad Request")
            }
            break;

          case 401:
            toastService.error("Unauthorized");
            break;

          case 404:
            router.navigateByUrl('/not-found');
            break;

          case 500:
            // use NavigationExtras pass a state (data) to a router and can be accessed
            // this data in the component's constructor (ServerError in this case)
            const navigationExtras: NavigationExtras = { state: { error: error.error }}
            router.navigateByUrl('/server-error', navigationExtras);
            break;

          default:
            toastService.error("Something went wrong");
            break;
        }
      }

      throw error;
    })
  );
};

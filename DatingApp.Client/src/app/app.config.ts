import { lastValueFrom } from 'rxjs';
import { routes } from './app.routes';
import { provideRouter, withViewTransitions } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';


import { InitService } from '../core/services/init-service';
import { jwtInterceptor } from '../core/interceptors/jwt-interceptor';
import { errorInterceptor } from '../core/interceptors/error-interceptor';
import { loadingInterceptor } from '../core/interceptors/loading-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withViewTransitions() /* add transition animation when routing */),
    provideHttpClient(withInterceptors([ errorInterceptor, jwtInterceptor, loadingInterceptor ])),
    provideAppInitializer(async () => { /* execute code before loading components */
      const initService = inject(InitService);
      return new Promise<void>((resolve) => {
        setTimeout(async () => {        /* add delay for better ui experience */
          try {
            return lastValueFrom(initService.init());
          } finally {
            const splash = document.getElementById('initial-splash');
            if (splash) {
              splash.remove();
            }
            resolve();
          }
        }, 500);
      });
    })
  ]
};

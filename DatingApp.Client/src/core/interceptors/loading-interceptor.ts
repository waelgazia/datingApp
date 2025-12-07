import { inject } from '@angular/core';
import { delay, finalize, of, tap } from 'rxjs';
import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';

import { BusyService } from '../services/busy-service';

const cache = new Map<string, HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams) : string => {
    const queryString = params.keys().map(key => `${key}=${params.get(key)}`).join('$');
    return queryString ? `${url}?${queryString}` : url;
  }

  const invalidateCache = (urlPattern: string) : void => {
    for (const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key);
      }
    }
  }

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method.includes('POST') && req.url.includes('/likes')) {
    invalidateCache('/likes')
  }
  if (req.method.includes('POST') && req.url.includes('/messages')) {
    invalidateCache('/messages')
  }
  if (req.method.includes('DELETE') && req.url.includes('/messages')) {
    invalidateCache('/messages')
  }
  if (req.method.includes('POST') && req.url.includes('/logout')) {
    cache.clear(); // clear the cache when the user logout.
  }

  if (req.method === 'GET') {
    const cachedResponse = cache.get(cacheKey);
    if (cachedResponse) {
      return of(cachedResponse);
    }
  }

  busyService.busy();

  return next(req).pipe(
    delay(500),
    tap(response => {
      cache.set(cacheKey, response)
    }),
    finalize(() => busyService.idle())
  );
};

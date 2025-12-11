import { inject } from '@angular/core';
import { delay, finalize, identity, of, tap } from 'rxjs';
import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';

import { BusyService } from '../services/busy-service';
import { environment } from '../../environments/environment';


type CacheEntry = {
  response: HttpEvent<unknown>;
  timestamp: number;
}

const cache = new Map<string, CacheEntry>();
const CACHE_DURATION_MS = 5 * 60 * 1000; /* 5 minutes */

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
  else if (req.method.includes('POST') && req.url.includes('/messages')) {
    invalidateCache('/messages')
  }
  else if (req.method.includes('DELETE') && req.url.includes('/messages')) {
    invalidateCache('/messages')
  }
  else if (req.method.includes('POST') && req.url.includes('admins')) {
    invalidateCache('/admins')
  }
  else if (req.method.includes('POST') && req.url.includes('/logout')) {
    cache.clear(); // clear the cache when the user logout.
  }

  if (req.method === 'GET') {
    const cachedResponse = cache.get(cacheKey);

    if (cachedResponse) {
      const isExpired = (Date.now() - cachedResponse.timestamp) > CACHE_DURATION_MS;
      if (!isExpired) {
        return of(cachedResponse.response);
      } else {
        cache.delete(cacheKey);
      }

    }
  }

  busyService.busy();

  return next(req).pipe(
    (environment.production ? identity : delay(500)), // no need to use delay in production
    tap(response => {
      cache.set(cacheKey, {
        response,
        timestamp: Date.now()
      })
    }),
    finalize(() => busyService.idle())
  );
};

import { HttpResponse } from "@angular/common/http";

import { HEADERS } from "../../constants/headers";
import { PaginatedResult, PaginationMetadata } from "../../interfaces/base/PaginatedResult";

export class HttpUtils {
  public static GetPaginatedResult<T>(response: HttpResponse<T[]>): PaginatedResult<T> {
    const items = response.body ?? [];

    const paginationHeader = response.headers.get(HEADERS.PAGINATION_HEADER);
    const paginationMetadata = paginationHeader
      ? JSON.parse(paginationHeader) as PaginationMetadata
      : {} as PaginationMetadata;

    return { items, paginationMetadata } as PaginatedResult<T>
  }
}
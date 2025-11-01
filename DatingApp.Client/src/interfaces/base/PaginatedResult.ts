export interface PaginatedResult<T> {
  items: T[],
  paginationMetadata: PaginationMetadata
}

export interface PaginationMetadata {
  pageNumber: number,
  totalPages: number,
  pageSize: number,
  totalCount: number,
  hasNext: boolean,
  hasPrevious: boolean
}
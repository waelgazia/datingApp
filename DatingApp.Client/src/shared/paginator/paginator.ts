import { Component, computed, input, model, output } from '@angular/core';
import { PaginationMetadata } from '../../interfaces/base/PaginatedResult';

@Component({
  selector: 'app-paginator',
  imports: [],
  templateUrl: './paginator.html',
  styleUrl: './paginator.css'
})
export class Paginator {
  // model allows the input property to be editable (two-way binding)
  paginationData = model.required<PaginationMetadata>();
  pageSizeOptions = input([5, 10, 15, 20]);
  lastItemIndex = computed(() => Math.min(
    this.paginationData().pageSize * (this.paginationData().pageNumber),
    this.paginationData().totalCount
  ));

  pageChanged = output<{ pageNumber: number, pageSize: number }>();

  onPageChanged(newPageNumber?: number, pageSize?: EventTarget | null) {
    if (newPageNumber) {
      this.paginationData.update(data => ({ ...data, pageNumber: newPageNumber }));
      console.log(this.paginationData);
    }
    if (pageSize) {
      const newPageSize = Number((pageSize as HTMLSelectElement).value);
      this.paginationData.update(data => ({ ...data, pageSize: newPageSize }));
    }

    this.pageChanged.emit({
      pageNumber: this.paginationData().pageNumber,
      pageSize: this.paginationData().pageSize,
    })
  }
}

import { FormsModule } from '@angular/forms';
import { Component, ElementRef, model, output, ViewChild } from '@angular/core';

import { STORAGE_KEY } from '../../../constants/storage-keys';
import { MembersParameters } from '../../../interfaces/ResourceParameters/MembersParameters';

@Component({
  selector: 'app-filter-modal',
  imports: [FormsModule],
  templateUrl: './filter-modal.html',
  styleUrl: './filter-modal.css'
})
export class FilterModal {
  @ViewChild('filterModal') modalRef!: ElementRef<HTMLDialogElement>

  modalClosed = output();
  dataSubmitted = output<MembersParameters>();
  membersParameters = model(new MembersParameters());

  constructor() {
    const filters = localStorage.getItem(STORAGE_KEY.FILTERS);
    if (filters) {
      this.membersParameters.set(JSON.parse(filters));
    }
  }

  open() {
    this.modalRef.nativeElement.showModal();
  }

  close() {
    this.modalRef.nativeElement.close();
    this.modalClosed.emit();
  }

  submit() {
    this.dataSubmitted.emit(this.membersParameters());
    this.close();
  }

  onMinAgeChange() {
    if (this.membersParameters().minAge < 18) {
      this.membersParameters().minAge = 18;
    }
  }

  onMaxAgeChange() {
    if (this.membersParameters().maxAge < this.membersParameters().minAge) {
      this.membersParameters().maxAge = this.membersParameters().minAge;
    }

    if (this.membersParameters().maxAge > 100) {
      this.membersParameters().maxAge = 100;
    }
  }
}

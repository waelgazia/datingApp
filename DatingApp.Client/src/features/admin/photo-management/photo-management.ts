import { Component, inject, OnInit, signal } from '@angular/core';

import { PhotoDto } from '../../../interfaces/models/PhotoDto';
import { AdminService } from '../../../core/services/admin-service';

@Component({
  selector: 'app-photo-management',
  imports: [],
  templateUrl: './photo-management.html',
  styleUrl: './photo-management.css'
})
export class PhotoManagement implements OnInit {
  private _adminService = inject(AdminService);

  protected unapprovedPhotos = signal<PhotoDto[]>([]);

  ngOnInit(): void {
    this.loadUnapprovedPhotos();
  }

  loadUnapprovedPhotos() {
    this._adminService.getUnapprovedPhotos().subscribe({
      next: photos => this.unapprovedPhotos.set(photos)
    })
  }

  approvePhoto(photoId: number) {
    this._adminService.approvePhoto(photoId).subscribe({
      next: () => this.unapprovedPhotos.update(photos => photos.filter(p => p.id !== photoId))
    })
  }

  rejectPhoto(photoId: number) {
    this._adminService.rejectPhoto(photoId).subscribe({
      next: () => this.unapprovedPhotos.update(photos => photos.filter(p => p.id !== photoId))
    })
  }
}

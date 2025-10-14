import { ActivatedRoute } from '@angular/router';
import { Component, inject, OnInit, signal } from '@angular/core';

import { Photo } from '../../../interfaces/models/Photo';
import { Member } from '../../../interfaces/models/Member';
import { MemberService } from '../../../core/services/member-service';
import { ImageUpload } from "../../../shared/image-upload/image-upload";
import { AccountService } from '../../../core/services/account-service';
import { StarButton } from "../../../shared/star-button/star-button";
import { DeleteButton } from "../../../shared/delete-button/delete-button";

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload, StarButton, DeleteButton],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  private _route = inject(ActivatedRoute);
  protected accountService = inject(AccountService);
  protected memberService = inject(MemberService);
  protected photos = signal<Photo[]>([]);
  protected loading = signal<boolean>(false)

  ngOnInit(): void {
    const memberId = this._route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: photos => this.photos.set(photos)
      })
    }
  }

  // mock a photo list (getter)
  get photoMocks() {
    // The parentheses () around {} are for telling JS you’re returning an object literal,
    // not starting a block of code.
    return Array.from({ length: 20 }, (_, i) => ({
      url: './user.png'
    }));
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: photo => {
        this.memberService.editMode.set(false);
        this.loading.set(false);
        this.photos.update(photos => [...photos, photo])
      },
      error: error => {
        console.log('Error uploading image', error);
        this.loading.set(false);
      }
    })
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if (currentUser)
        {
          currentUser.imageUrl = photo.url;
          this.accountService.setCurrentUser(currentUser);
        }

        this.memberService.member.update(member => ({
          ...member,
          imageUrl: photo.url
        }) as Member)
      }
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos => photos.filter(p => p.id !== photoId))
      }
    })
  }
}

import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css'
})

export class ImageUpload {
  protected imageSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected isDragging = false;

  private fileToUpload: File | null = null;
  uploadFile = output<File>();
  loading = input<boolean>(false);

  onDragOver(event: DragEvent) {
    event.preventDefault();     // stop default drag leave behavior
    event.stopPropagation();    // prevent event bubbling
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {    /* on letting og the mouse */
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }

  private previewImage(file: File) {
    const reader = new FileReader();
    reader.onload = (e) => this.imageSrc?.set(e.target?.result);
    reader.readAsDataURL(file);
  }

  onCancel() {
    this.fileToUpload = null;
    this.imageSrc.set(null);
  }

  onUploadFile() {
    if (this.fileToUpload) {
      this.uploadFile.emit(this.fileToUpload);
    }
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement)?.files?.[0];
    if (!file) return;

    this.previewImage(file);
    this.fileToUpload = file;
  }
}

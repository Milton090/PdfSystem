import { Component, ElementRef, inject, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import SignaturePad from 'signature_pad';
import { PdfService } from '../../../services/pdf.service';
import { AlertService } from '../../../services/alert.service';


@Component({
	selector: 'app-signer',
	standalone: true,
	imports: [
		NgxExtendedPdfViewerModule,
	],
	templateUrl: './signer.component.html',
	styleUrl: './signer.component.css'
})
export class SignerComponent {


	private pdfSvc = inject(PdfService);
	private alert = inject(AlertService);

	@ViewChild('signatureCanvas') signatureCanvas!: ElementRef<HTMLCanvasElement>;
	private signaturePad!: SignaturePad;

	pdfUrl: string;

	constructor(
		public dialogRef: MatDialogRef<SignerComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any
	) {
		this.pdfUrl = data.pdfUrl;
	}

	ngAfterViewInit() {
		if (this.signatureCanvas) {
			this.signaturePad = new SignaturePad(this.signatureCanvas.nativeElement);
		}
	}

	saveSignature() {
		if (this.signaturePad && !this.signaturePad.isEmpty()) {
			const signatureData = this.signaturePad.toDataURL();
			this.pdfSvc.signPdf(this.data.pdfId, signatureData).subscribe(
				(response: any) => {
					const contentType = response.type;
					if (contentType === 'application/pdf') {
						this.alert.success('Exito', 'PDF firmado correctamente');
						const pdfUrl = window.URL.createObjectURL(response);
						window.open(pdfUrl);
						this.dialogRef.close(true);
					} else {
						this.alert.error('Error', response.msg);
					}
				},
			);
		} else {
			this.alert.error('Error', 'No se ha firmado el PDF');
		}
	}


	clearSignature() {
		if (this.signaturePad) {
			this.signaturePad.clear();
		}
	}

	closeModal() {
		this.dialogRef.close(false);
	}
}

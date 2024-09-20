import { Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { DatePipe, NgStyle } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MenuComponent } from '../../../components/menu/menu.component';
import { MatButtonModule } from '@angular/material/button';
import { AlertService } from '../../../services/alert.service';
import { ResponseI } from '../../../interfaces/response.interface';
import { PdfService } from '../../../services/pdf.service';
import { PdfI } from '../../../interfaces/pdf.interface';
import { SignerComponent } from '../signer/signer.component';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';


@Component({
	selector: 'app-list-pdf',
	standalone: true,
	imports: [
		MenuComponent,
		MatPaginatorModule,
		NgStyle,
		MatMenuModule,
		MatIconModule,
		MatTableModule,
		MatLabel,
		MatFormFieldModule,
		MatInputModule,
		MatButtonModule,
		FormsModule,
		ReactiveFormsModule,
		DatePipe,
		MatOptionModule,
		MatSelectModule,
	],
	templateUrl: './list-pdf.component.html',
	styleUrl: './list-pdf.component.css'
})

export class ListPdfComponent {

	pdfs: PdfI[] = [];
	dataSource = new MatTableDataSource(this.pdfs);

	statusFilter = [
		{ value: 1, label: 'Todos' },
		{ value: 2, label: 'Firmados' },
		{ value: 3, label: 'No firmados' },
	];

	filter = new FormGroup({
		selectedStatus: new FormControl(1)
	});

	@ViewChild(MatPaginator) paginator!: MatPaginator;


	constructor(
		private pdfSvc: PdfService,
		private alert: AlertService,
		private dialog: MatDialog
	) { }

	ngOnInit(): void {
		this.getAll();
		this.filter.get('selectedStatus')?.valueChanges.subscribe((value) => {
			switch (value) {
				case 1:
					this.dataSource.data = this.pdfs;
					break;
				case 2:
					this.dataSource.data = this.pdfs;
					this.dataSource.data = this.dataSource.data.filter(s => s.isSigned);
					break;
				case 3:
					this.dataSource.data = this.pdfs;
					this.dataSource.data = this.dataSource.data.filter(s => !s.isSigned);
					break;
			}
		});
	}

	ngAfterViewInit() {
		this.dataSource.paginator = this.paginator;
	}

	openPdf(pdf: PdfI) {
		this.pdfSvc.downloadPdf(pdf.id!).subscribe(blob => {
			const url = URL.createObjectURL(blob);
			this.dialog.open(SignerComponent, {
				disableClose: true,
				width: '80%',
				height: '90%',
				data: { pdfUrl: url, pdfId: pdf.id }
			}).afterClosed().subscribe((res: boolean) => {
				if (res) {
					this.getAll();
				}
			}, error => {
				console.error('Error al descargar el archivo:', error);
			});
		});
	}

	downloadFile(file: any): void {
		this.pdfSvc.downloadPdf(file.id).subscribe(
			blob => {
				const url = window.URL.createObjectURL(blob);
				const a = document.createElement('a');
				a.href = url;
				a.download = file.fileName;
				document.body.appendChild(a);
				a.click();
				document.body.removeChild(a);
				window.URL.revokeObjectURL(url);
			},
			error => {
				console.error('Error al descargar el archivo:', error);
			}
		);
	}


	onFileSelected(event: Event): void {
		const input = event.target as HTMLInputElement;
		if (input.files && input.files.length > 0) {
			const file = input.files[0];
			if (file.type === 'application/pdf') {
				this.uploadPdf(file);
			} else {
				this.alert.error('Error', 'El archivo seleccionado no es un PDF.');
			}
		}
	}


	uploadPdf(file: File) {
		this.pdfSvc.uploadPdf(file).subscribe({
			next: (res: ResponseI) => {
				if (res.success) {
					this.getAll();
					this.alert.success('Operacion exitosa', 'El archivo se ha subido correctamente.');
				} else {
					this.alert.error('Error', res.msg);
				}
			},
			error: (error) => {
				const errorMsg = error.message || 'Ocurrió un error al subir el archivo. Por favor, inténtalo nuevamente.';
				this.alert.error('Error', errorMsg);
			}
		});
	}

	deletePdf(id: string) {
		this.pdfSvc.deletePdf(id).subscribe({
			next: (res: ResponseI) => {
				if (res.success) {
					this.getAll();
					this.alert.success('Operacion exitosa', 'El archivo se ha eliminado correctamente.');
				} else {
					this.alert.error('Error', res.msg);
				}
			},
			error: (error) => {
				const errorMsg = error.message || 'Ocurrió un error al eliminar el archivo. Por favor, inténtalo nuevamente.';
				this.alert.error('Error', errorMsg);
			}
		});
	}


	applyFilter(event: Event) {
		const filterValue = (event.target as HTMLInputElement).value;
		this.dataSource.filter = filterValue.trim().toLowerCase();
	}

	getAll() {
		this.pdfSvc.getAll().subscribe((res: ResponseI) => {
			this.pdfs = res.data;
			this.dataSource.data = this.pdfs;
		});
	}
}

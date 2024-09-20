import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment/environment';
import { ResponseI } from '../interfaces/response.interface';
import { PdfI } from '../interfaces/pdf.interface';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})

export class PdfService {

  private readonly url: string = `${environment.apiUrl}${environment.pdfController}`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<ResponseI> {
    return this.http.get<ResponseI>(`${this.url}`);
  }


  signPdf(pdfId: string, signature: string): Observable<Blob | ResponseI> {
    const formData = new FormData();
    const blob = this.convertBase64ToBlob(signature, 'image/png');
    formData.append('signatureImage', blob, 'signature.png');

    return this.http.post<Blob | ResponseI>(`${this.url}/sign/${pdfId}`, formData, { responseType: 'blob' as 'json'})
  }

  uploadPdf(file: File): Observable<ResponseI> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<ResponseI>(`${this.url}/upload`, formData);
  }


  downloadPdf(id: string): Observable<Blob> {
    return this.http.get(`${this.url}/download/${id}`, { responseType: 'blob' });
  }


  deletePdf(id: string): Observable<ResponseI> {
    return this.http.delete<ResponseI>(`${this.url}/${id}`);
  }


  private convertBase64ToBlob(base64: string, contentType: string): Blob {
    const [header, data] = base64.split(',');
    const base64Data = header.includes('base64') ? data : base64;

    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    return new Blob([byteArray], { type: contentType });
  }


}

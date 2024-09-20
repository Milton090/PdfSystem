import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { AlertService } from '../../services/alert.service';
import { SweetAlertResult } from 'sweetalert2';


@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, RouterModule, MatSidenavModule, MatListModule, MatToolbarModule, CommonModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss'
})

export class MenuComponent {

  router = inject(Router);
  alert = inject(AlertService);

  modules: any[] = [];

  constructor(
  ) {

    this.modules = [
      { name: 'Documentos', route: '/documents' },
    ];

  }


  logout() {
    this.alert.question('Cerrar sesión', '¿Estás seguro de cerrar sesión?').then((result: SweetAlertResult) => {
      if (result.isConfirmed) {
        localStorage.removeItem('token');
        this.alert.success('Sesión cerrada', 'Sesión cerrada correctamente');
        this.router.navigate(['/login']);
      }
    });
  }
}

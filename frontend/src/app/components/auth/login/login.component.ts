import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthI } from '../../../interfaces/auth.interface';
import { ResponseI } from '../../../interfaces/response.interface';
import { Router, RouterModule } from '@angular/router';
import { AlertService } from '../../../services/alert.service';

@Component({
	selector: 'app-login',
	standalone: true,
	imports: [ReactiveFormsModule, RouterModule],
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss']
})
export class LoginComponent {

	authService = inject(AuthService);
	alert = inject(AlertService);
	router = inject(Router);

	userData = new FormGroup({
		email: new FormControl('', Validators.required),
		password: new FormControl('', Validators.required)
	});

	login() {
		if (this.userData.invalid) {
			this.alert.error('Error', 'Por favor complete todos los campos');
			return;
		}

		const loginData: AuthI = {
			email: this.userData.value.email ?? '',
			password: this.userData.value.password ?? ''
		};

		this.authService.login(loginData).subscribe({
			next: (res: ResponseI) => {
				if (res.success) {
					localStorage.setItem('token', res.data.token);
					this.alert.success('Login correcto', 'Bienvenido a la aplicación');
					this.router.navigate(['/documents']);
				} else {
					this.alert.error('Error', res.msg);
				}
			},
			error: (error) => {
				const errorMsg = error || 'Ocurrió un error al registrar el usuario';
				this.alert.error('Error', errorMsg);
			}
		});
	}
}
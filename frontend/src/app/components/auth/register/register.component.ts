import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthI } from '../../../interfaces/auth.interface';
import { ResponseI } from '../../../interfaces/response.interface';
import { Router, RouterModule } from '@angular/router';
import { AlertService } from '../../../services/alert.service';

@Component({
	selector: 'app-register',
	standalone: true,
	imports: [ReactiveFormsModule, RouterModule],
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
	authService = inject(AuthService);
	router = inject(Router);
	alert = inject(AlertService);

	userData = new FormGroup({
		email: new FormControl('', Validators.required),
		password: new FormControl('', Validators.required)
	});

	register() {
		if (this.userData.invalid) {
			this.alert.error('Error', 'Por favor complete todos los campos');
			return;
		}

		const registerData: AuthI = {
			email: this.userData.value.email ?? '',
			password: this.userData.value.password ?? ''
		};

		this.authService.register(registerData).subscribe({
			next: (data: ResponseI) => {
				if (data.success) {
					this.alert.success('Usuario creado', 'Usuario creado correctamente, por favor inicie sesion');
					this.router.navigate(['/login']);
				} else {
					this.alert.error('Error', data.msg);
				}
			},
		});
	}

}

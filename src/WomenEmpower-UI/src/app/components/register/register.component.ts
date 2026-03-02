import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm = new FormGroup({
    fullName: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', Validators.required)
  });

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  private apiUrl = 'http://localhost:5032/api/account/register';

  constructor(private http: HttpClient, private router: Router) { }

  get passwordMismatch() {
    const pw = this.registerForm.get('password')?.value;
    const cpw = this.registerForm.get('confirmPassword')?.value;
    return pw && cpw && pw !== cpw;
  }

  onRegister() {
    if (this.registerForm.valid && !this.passwordMismatch) {
      this.isLoading = true;
      this.errorMessage = '';

      const payload = {
        fullName: this.registerForm.value.fullName,
        email: this.registerForm.value.email,
        password: this.registerForm.value.password
      };

      this.http.post(this.apiUrl, payload).subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage = 'Account created successfully! Redirecting to login...';
          setTimeout(() => this.router.navigate(['/login']), 2000);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
        }
      });
    }
  }
}

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required)
  });

  isLoading = false;
  errorMessage = '';

  private apiUrl = 'http://localhost:5032/api/account/login';

  constructor(private http: HttpClient, private router: Router) { }

  onLogin() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      this.http.post(this.apiUrl, this.loginForm.value).subscribe({
        next: (response: any) => {
          this.isLoading = false;
          localStorage.setItem('authToken', response.token);
          localStorage.setItem('userEmail', response.email || '');
          localStorage.setItem('userRole', response.role || 'User');

          // Use window.location for guaranteed navigation after login
          if (response.role === 'Admin') {
            window.location.href = '/admin';
          } else {
            window.location.href = '/home';
          }
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.status === 401
            ? 'Invalid email or password.'
            : 'Something went wrong. Please try again.';
        }
      });
    }
  }
}

import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'account',
  templateUrl: './account.component.html'
})
export class AccountComponent {
  public response: AuthResponse;
  public loginForm: FormGroup;
  public errorMessage: string = '';
  public showError: boolean;
  private _returnUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private _router: Router, private _route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      username: new FormControl("", [Validators.required]),
      password: new FormControl("", [Validators.required])
    })
    this._returnUrl = this._route.snapshot.queryParams['returnUrl'] || '/';
  }

  public validateControl = (controlName: string) => {
    return this.loginForm.controls[controlName].invalid && this.loginForm.controls[controlName].touched
  }
  public hasError = (controlName: string, errorName: string) => {
    return this.loginForm.controls[controlName].hasError(errorName)
  }
  public loginUser = (loginFormValue) => {
    this.showError = false;
    const login = { ...loginFormValue };
    const body: AuthRequest = {
      login: login.username,
      password: login.password
    }

    this.http.post<AuthResponse>(this.baseUrl + 'accounts/login', body).subscribe(result => {
      localStorage.setItem("token", result.token);
      this._router.navigate([this._returnUrl]);
    }, error => {
        this.errorMessage = error;
        this.showError = true;
    });   
  }

  
}

interface AuthRequest {
  login: string;
  password: string;
}

interface AuthResponse {
  token: string;
  username: string;
}

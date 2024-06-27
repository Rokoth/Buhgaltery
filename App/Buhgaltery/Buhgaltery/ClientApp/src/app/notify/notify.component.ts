import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'notify',
  templateUrl: './notify.component.html'
})
export class NotifyComponent {
  public notifyForm: FormGroup;
  public errorMessage: string = '';
  public showError: boolean;
  private _returnUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private _router: Router, private _route: ActivatedRoute) { }

  ngOnInit(): void {
    this.notifyForm = new FormGroup({
      message: new FormControl("", [Validators.required])
    })
    this._returnUrl = this._route.snapshot.queryParams['returnUrl'] || '/';
  }

  public validateControl = (controlName: string) => {
    return this.notifyForm.controls[controlName].invalid && this.notifyForm.controls[controlName].touched
  }
  public hasError = (controlName: string, errorName: string) => {
    return this.notifyForm.controls[controlName].hasError(errorName)
  }
  public sendMessage = (notifyFormValue) => {
    this.showError = false;
    const notify = { ...notifyFormValue };
    const body: NotifyRequest = {
      message: notify.message
    }

    this.http.post(this.baseUrl + 'api/v1/common/notify', body).subscribe(result => {
      this._router.navigate([this._returnUrl]);
    }, error => {
        this.errorMessage = error;
        this.showError = true;
    });   
  }
}

interface NotifyRequest {
  message: string;
}

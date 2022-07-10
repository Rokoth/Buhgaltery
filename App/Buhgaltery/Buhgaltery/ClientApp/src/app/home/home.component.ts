import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public data: AllDataModel;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private _router: Router) {
    var _token = localStorage.getItem("token");
    if (_token === null || _token === "") {
      this._router.navigate(["account"]);
    }
    var httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "Authorization": "Bearer " + _token
      })
    };
    http.get<AllDataModel>(baseUrl + 'home/current-data', httpOptions ).subscribe(result => {
      this.data = result;      
    }, error => {
        console.error(error);
        if (error.status == 401) {
          this._router.navigate(["account"]);
        }
    });
  }
}

interface AllDataModel {
  incomings: number;
  outgoings: number;
  reserves: number;
  free: number;
  isError: boolean;
  errorMessage: string;
}

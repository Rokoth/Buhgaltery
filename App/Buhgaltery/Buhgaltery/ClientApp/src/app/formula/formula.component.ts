import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class FormulaComponent {
  public data: FormulaModel[];
  public currentDetailed: FormulaModel;

  private _http: HttpClient;
  private _baseUrl: string;
  private _router: Router;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private router: Router) {
    this._http = http;
    this._baseUrl = baseUrl;
    this._router = router;
    var _token = localStorage.getItem("token");
    if (_token === null || _token === "") {
      this.router.navigate(["account"]);
    }
    var httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "Authorization": "Bearer " + _token
      })
    };
    http.get<FormulaModel[]>(baseUrl + 'formula/getlist', httpOptions ).subscribe(result => {
      this.data = result;      
    }, error => {
        console.error(error);
        if (error.status == 401) {
          this.router.navigate(["account"]);
        }
    });
  }

  getDetails(id: string) {
    var _token = localStorage.getItem("token");
    if (_token === null || _token === "") {
      this.router.navigate(["account/?_returnUrl=formula"]);
    }
    var httpOptions = {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
        "Authorization": "Bearer " + _token
      })
    };
    this._http.get<FormulaModel>(this._baseUrl + 'formula/getitem/' + id, httpOptions).subscribe(result => {
      this.currentDetailed = result;
    }, error => {
      console.error(error);
      if (error.status == 401) {
        this.router.navigate(["account/?_returnUrl=formula"]);
      }
      return null;
    });    
  }

}

interface FormulaModel {
  id: string;
  name: string;
  text: string;
  isDefault: boolean;
}

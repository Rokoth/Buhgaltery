import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public data: AllDataModel;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<AllDataModel>(baseUrl + 'home/current-data').subscribe(result => {
      this.data = result;
      alert(result.errorMessage);
      if (result.isError === true) {
        alert(result.errorMessage);
      }
    }, error => console.error(error));
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

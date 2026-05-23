import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'UserRegistrationApp';
  editdata: any;
  isRefreshList: boolean | undefined;
  onEditData(data: any){
    this.editdata = data;
  }   
  onDataUpdated(data: any){
    this.isRefreshList = data;
  }
}

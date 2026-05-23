import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit, OnChanges {
  constructor(private userService: UserService) { }
@Input() refreshList: boolean | undefined ;
  ngOnChanges(changes: SimpleChanges): void {
if(changes['refreshList'] && changes['refreshList'].currentValue) {
      this.getListData();
    }
  }
  listData: any;
  ngOnInit(): void {
    this.getListData();
  }

  getListData() {
    this.userService.GetUserList().subscribe((res) => {
      this.listData = res;
    });
  }
  @Output() editData = new EventEmitter<any>();
  editUser(user: any) {
    this.userService.GetUserDetails(user.id).subscribe((res) => {
      this.editData.emit(res);
    });
  }

  deleteUser(id: number) {
    let cancel = confirm("Are you sure to delete this user?");
    if (cancel == true) {
      this.userService.deleteUser(id).subscribe((res) => {
        this.getListData();
      });
    }
  }
}

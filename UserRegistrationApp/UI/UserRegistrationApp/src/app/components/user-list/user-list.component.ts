import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserDetail, UserListItem } from 'src/app/models/user.model';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit, OnDestroy {
  @Output() editData = new EventEmitter<UserDetail>();

  listData: UserListItem[] = [];
  errorMessage = '';
  private refreshSubscription?: Subscription;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.getListData();
    this.refreshSubscription = this.userService.listRefresh$.subscribe(() => {
      this.getListData();
    });
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  getListData(): void {
    this.errorMessage = '';
    this.userService.getUserList().subscribe({
      next: (res) => {
        this.listData = res ?? [];
      },
      error: () => {
        this.listData = [];
        this.errorMessage = 'Failed to load users.';
      }
    });
  }

  editUser(user: UserListItem): void {
    this.userService.getUserDetails(user.id).subscribe({
      next: (res) => {
        this.editData.emit(res);
      },
      error: () => {
        this.errorMessage = 'Failed to load user details.';
      }
    });
  }

  deleteUser(id: number): void {
    const confirmed = confirm('Are you sure you want to delete this user?');
    if (!confirmed) {
      return;
    }

    this.userService.deleteUser(id).subscribe({
      next: () => {
        this.userService.requestListRefresh();
      },
      error: () => {
        this.errorMessage = 'Failed to delete user.';
      }
    });
  }
}

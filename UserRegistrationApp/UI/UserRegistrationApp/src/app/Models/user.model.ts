export interface UserListItem {
  id: number;
  name: string;
  email: string;
  dateOfBirth: string;
  gender: string;
  hobby: string;
  status: boolean;
  createdDate?: string;
}

export interface UserDetail extends UserListItem {
  updatedDate?: string;
}

export interface UserFormValue {
  id: number;
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
  dateOfBirth: string;
  gender: string;
  hobby: string;
  status: boolean;
}

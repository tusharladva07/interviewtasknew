import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserDetail, UserFormValue } from 'src/app/models/user.model';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit, OnChanges {
  @Output() isDataUpdated = new EventEmitter<void>();
  @Output() editCleared = new EventEmitter<void>();
  @Input() editInputData: UserDetail | null = null;

  userForm: FormGroup = new FormGroup({});
  errorMessage = '';
  isEditMode = false;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.formsetup();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['editInputData']) {
      this.isEditMode = !!this.editInputData;

      if (this.editInputData) {
        this.userForm.patchValue({
          ...this.editInputData,
          password: '',
          confirmPassword: ''
        });
        this.showPassword = false;
        this.showConfirmPassword = false;
      }
    }
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword = !this.showPassword;
      return;
    }
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  passwordMatchValidator(form: AbstractControl) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }

  clearForm(): void {
    this.userForm.reset();
    this.isEditMode = false;
    this.errorMessage = '';
    this.showPassword = false;
    this.showConfirmPassword = false;
    this.formsetup();
    this.editCleared.emit();
  }

  private notifyListUpdated(): void {
    this.userService.requestListRefresh();
    this.isDataUpdated.emit();
  }

  saveForm(): void {
    this.userForm.markAllAsTouched();
    this.userForm.updateValueAndValidity();
    this.errorMessage = '';

    if (!this.userForm.valid) {
      return;
    }

    const user = this.userForm.value as UserFormValue;
    this.userService.addUser(user).subscribe({
      next: () => {
        this.notifyListUpdated();
        this.clearForm();
      },
      error: (err) => {
        this.errorMessage = err?.error?.message ?? 'Failed to create user.';
      }
    });
  }

  updateForm(): void {
    this.userForm.markAllAsTouched();
    this.userForm.updateValueAndValidity();
    this.errorMessage = '';

    if (!this.userForm.valid || !this.editInputData) {
      return;
    }

    const user = this.userForm.value as UserFormValue;
    this.userService.updateUser(user.id, user).subscribe({
      next: () => {
        this.notifyListUpdated();
        this.clearForm();
      },
      error: (err) => {
        this.errorMessage = err?.error?.message ?? 'Failed to update user.';
      }
    });
  }

  formsetup(): void {
    this.userForm = this.fb.group({
      id: [0],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      gender: ['Male', Validators.required],
      dateOfBirth: ['', Validators.required],
      hobby: ['Cricket', Validators.required],
      status: [false],
      password: ['', [Validators.required, Validators.minLength(4)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(4)]]
    }, {
      validators: this.passwordMatchValidator
    });
  }
}

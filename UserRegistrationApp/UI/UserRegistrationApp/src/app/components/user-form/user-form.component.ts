import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {  User } from 'src/app/Models/User';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit,OnChanges {
  @Output() isDataUpdated = new EventEmitter<any>();
  @Input() editInputData: any;
  constructor(private fb: FormBuilder,private userServices: UserService) { }
  ngOnChanges(changes: SimpleChanges): void {
    if (this.editInputData) {
      this.userForm.patchValue(this.editInputData);
    }
  }
  userForm: FormGroup = new FormGroup({});

  ngOnInit(): void {
    this.formsetup();
  }
  passwordMatchValidator(form: AbstractControl) {
    const password =
      form.get('password')?.value;

    const confirmPassword =
      form.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return {
        passwordMismatch: true
      };
    }

    return null;
  }
  clearForm() {
    this.userForm.reset();
    this.editInputData = null;
    this.formsetup();
  }
  saveForm() {
    this.userForm.markAllAsTouched();
    this.userForm.updateValueAndValidity();

    if (this.userForm.valid) {
      const user: User = this.userForm.value;
      this.userServices.AddUser(user).subscribe((res) => {  
        this.clearForm();
        this.isDataUpdated.emit(true);
      });
    }
  }
  updateForm() {
    this.userForm.markAllAsTouched();
    this.userForm.updateValueAndValidity();

    if (this.userForm.valid) {
      const user: User = this.userForm.value;
      this.userServices.updateUser(user.id!, user).subscribe((res) => {  
        this.clearForm();
        this.isDataUpdated.emit(true);
      });
    }
  }
  formsetup() { 
   this.userForm = this.fb.group({
      id: [0],
      name: ['', Validators.required],
      email: ['', Validators.required],
      gender: ['Male', Validators.required],
      dateOfBirth: ['', Validators.required],
      hobby: ['Cricket', Validators.required],
      status: [false],
      password: ['', [
        Validators.required,
        Validators.minLength(4)
      ]],

      confirmPassword: ['', [
        Validators.required,
        Validators.minLength(4)
      ]]
    }, {
      validators: this.passwordMatchValidator
    });
  }
}


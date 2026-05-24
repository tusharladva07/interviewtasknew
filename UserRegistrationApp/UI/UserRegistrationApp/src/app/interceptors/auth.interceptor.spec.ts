import { TestBed } from '@angular/core/testing';
import { AuthService } from '../services/auth.service';
import { AuthInterceptor } from './auth.interceptor';

describe('AuthInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      AuthInterceptor,
      {
        provide: AuthService,
        useValue: { getToken: () => null }
      }
    ]
  }));

  it('should be created', () => {
    const interceptor: AuthInterceptor = TestBed.inject(AuthInterceptor);
    expect(interceptor).toBeTruthy();
  });
});

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';


import { SharedModule } from '../shared/shared.module';

import { RoutingGuard } from '../shared/routing-guard.service';

import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { ChangePasswordComponent } from './change-password/change-password.component';


@NgModule({
  declarations: [LoginComponent, SignupComponent, ChangePasswordComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([
      {path:'login', component:LoginComponent, canActivate:[RoutingGuard] },
      {path:'signup', component:SignupComponent,canActivate:[RoutingGuard]},
      {path:'changePassword',component:ChangePasswordComponent,canActivate:[RoutingGuard]}
    ]),
  ],
  providers:[]
})
export class UserModule { }

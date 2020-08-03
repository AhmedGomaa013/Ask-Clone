import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatDialogModule } from '@angular/material/dialog';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import {TimeAgoPipe} from 'time-ago-pipe';

import { UserService } from './user-service';
import { ConfirmEqualDirective } from './confirm-equal-directive.directive';
import { AuthService } from './auth.service';
import { AuthInterceptor } from './auth-interceptor';
import { DataService } from './data.service';
import { RoutingGuard } from './routing-guard.service';
import { OpenDialogComponent } from './open-dialog/open-dialog.component';



@NgModule({
  declarations: [ConfirmEqualDirective, OpenDialogComponent,TimeAgoPipe],
  imports: [
    CommonModule,
    FormsModule,
    AngularFontAwesomeModule,
    MatDialogModule,
    RouterModule.forChild([
      {path:'confirm/dialog/open',component:OpenDialogComponent}
    ])
  ],
  
  exports:[CommonModule,
  FormsModule,
HttpClientModule, 
AngularFontAwesomeModule,
ConfirmEqualDirective,
TimeAgoPipe,
OpenDialogComponent],

providers:[UserService,
AuthService,DataService,RoutingGuard,
{
  provide:HTTP_INTERCEPTORS,
  useClass: AuthInterceptor,
  multi: true
}]
})
export class SharedModule { }

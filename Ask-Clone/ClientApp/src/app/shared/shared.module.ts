import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatDialogModule } from '@angular/material/dialog';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import {TimeAgoPipe} from 'time-ago-pipe';

import { ConfirmEqualDirective } from './confirm-equal-directive.directive';

import { OpenDialogComponent } from './open-dialog/open-dialog.component';
import { FollowComponent } from './follow/follow.component';

import { AuthService } from './auth.service';
import { UserService } from './user-service';
import { DataService } from './data.service';
import { RoutingGuard } from './routing-guard.service';
import { ErrorInterceptor } from './error-interceptor';




@NgModule({
  declarations: [ConfirmEqualDirective, OpenDialogComponent,TimeAgoPipe, FollowComponent],
  imports: [
    CommonModule,
    FormsModule,
    AngularFontAwesomeModule,
    MatDialogModule,
    RouterModule.forChild([
      {path:'confirm/dialog/open',component:OpenDialogComponent},
      {path:'follow/dialog/open',component:FollowComponent}
    ])
  ],
  
  exports:[CommonModule,
  FormsModule,
HttpClientModule, 
AngularFontAwesomeModule,
ConfirmEqualDirective,
TimeAgoPipe,
OpenDialogComponent,
FollowComponent],

providers:[
  AuthService,
  UserService,
  DataService,
  RoutingGuard,
  {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
  }
]
})
export class SharedModule { }

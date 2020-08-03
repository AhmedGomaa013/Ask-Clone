import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { SharedModule } from '../shared/shared.module';

import { ProfileComponent } from './profile/profile.component';
import { InboxComponent } from './inbox/inbox.component';
import { RoutingGuard } from '../shared/routing-guard.service';





@NgModule({
  declarations: [ProfileComponent, InboxComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([
      {path:'user/:username',component:ProfileComponent},
      {path:'inbox/:username',canActivate:[RoutingGuard], component:InboxComponent}
    ]),
    MatSlideToggleModule
  ]
})
export class QuestionsModule { }

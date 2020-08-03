import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/shared/auth.service';
import { Router } from '@angular/router';
import { Question } from 'src/app/shared/question';
import { MatDialog } from '@angular/material/dialog';
import { OpenDialogComponent } from 'src/app/shared/open-dialog/open-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { DataService } from 'src/app/shared/data.service';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit {

  constructor(private authService:AuthService,private router:Router,private dialog:MatDialog,
    private toastrService:ToastrService, private dataService:DataService) { }

  username:string='';
  questions:Question[] = [];

onConfirmDeletion(question:Question)
  {
    
    const openedDialog = this.dialog.open(OpenDialogComponent,{
      data:  {title:"Warning",content:'Are you sure you want to permanently delete this question?',delete:true,answer:''}
    });
    openedDialog.afterClosed().subscribe(result => {
      if(result == 1)
      {
        this.onDeleteQuestion(question);
      }
    })

  }

  onDeleteQuestion(question:Question)
  {
    this.dataService.delete(question.questionId).subscribe({
      next: success=>{
        let index:number = this.questions.indexOf(question,0);
        this.questions.splice(index,1);
        this.toastrService.success("Question Deleted Successfully");
      },
      error: err =>{
        this.toastrService.error("Couldn't delete this question","Operation Failed");
      }
    });
  }
  
  onAnswering(question:Question)
  {
    const dialogRef = this.dialog.open(OpenDialogComponent,{
      data:{title:'Question',content:question.question,delete:false,answer:question.answer?question.answer:''}
    });

    dialogRef.afterClosed().subscribe(result => {
      question.answer = result;
      if((result != 0)&&(result != ''))
      {
        this.onEditing(question);
      }
      
    })
  }

  onEditing(question:Question)
  {
    this.dataService.put(question).subscribe({
      next: success => {
        let index:number = this.questions.indexOf(question,0);
        this.questions.splice(index,1);
        this.toastrService.success("Answer sent successfully");
      },
      error: err => {
        this.toastrService.error("Couldn't answer this question","Operation Failed");
      }
    })
  }

  onSortQuestions()
  {
    this.dataService.sort(this.questions);
  }

ngOnInit() {
  this.username = this.router.url.slice(7);
  if (!this.username.length) this.router.navigateByUrl('/');
  if (this.authService.username != this.username) this.router.navigate(['inbox', this.authService.username]);
    this.dataService.authorizedGet().subscribe({
      next: questions =>{
        this.questions = questions;
        this.onSortQuestions();
      },
      error: err=>{}
    });
    
  }

}

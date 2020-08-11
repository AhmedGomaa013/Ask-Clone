import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface DialogData
{
  delete:boolean,
  title:string,
  content:string,
  answer:string
}
@Component({
  selector: 'app-open-dialog',
  templateUrl: './open-dialog.component.html',
  styleUrls: ['./open-dialog.component.css']
})

export class OpenDialogComponent{

  constructor(private dialogRef: MatDialogRef<OpenDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) { }
  
  answer:string=this.data.answer;
  errorMessage='';
  onNoClick()
  {
    this.dialogRef.close();
  }

  onYesClick()
  {
    if(!this.data.delete) 
    {
      if((this.answer != '')&&(this.answer != null)&&(this.answer.length <= 3000)) this.dialogRef.close(this.answer);
      else this.errorMessage = "The answer mustn't exceed 3000 character!"
    }
    else
    {
      this.dialogRef.close(1);
    }
  }

}

export interface Question {
  questionId: number;
  question: string;
  answer: string;
  isAnswered: boolean;
  time: Date;
  questionFrom: string;
  questionTo: string;
}

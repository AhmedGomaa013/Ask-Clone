using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.ViewModels
{
    public class QuestionsViewModel
    {
        public int QuestionId { get; set; }
        
        [Required]
        [MaxLength(300)]
        public string Question { get; set; }
        
        [MaxLength(3000)]
        public string Answer { get; set; }
        public Boolean IsAnswered { get; set; }
        public DateTime Time { get; set; }
        
        [MinLength(5), MaxLength(25),AllowNull]
        public string QuestionFrom { get; set; }
        
        [Required]
        [MinLength(5),MaxLength(25)]
        public string QuestionTo { get; set; }
    }
}

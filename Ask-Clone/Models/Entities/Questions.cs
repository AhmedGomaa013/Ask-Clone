using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models.Entities
{
    public class Questions
    {
        [Key]
        public int QuestionId { get; set; }

        [MaxLength(300)]
        public string Question { get; set; }
        [MaxLength(3000)]
        public string Answer { get; set; }
        public Boolean IsAnswered { get; set; }
        public ApplicationUser QuestionFrom { get; set; }
        public ApplicationUser QuestionTo { get; set; }
        public DateTime Time { get; set; }

    }
}

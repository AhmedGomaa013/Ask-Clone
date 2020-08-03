using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.ViewModels
{
    public class LoginUserViewModel
    {
        [Required]
        [MinLength(5),MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

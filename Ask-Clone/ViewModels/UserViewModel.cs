using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
		public string LastName { get; set; }
		
        [Required]
        [MinLength(5),MaxLength(25)]
        public string UserName { get; set; }
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}

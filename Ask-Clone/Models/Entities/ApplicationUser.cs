using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Following = new HashSet<ApplicationUser>();
            Followers = new HashSet<ApplicationUser>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public HashSet<ApplicationUser> Following { get; set; }
        public HashSet<ApplicationUser> Followers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models.Entities
{
    public class Follow
    {
        [Key]
        public int FollowId { get; set; }
        public ApplicationUser FollowedUser { get; set; }
        public ApplicationUser FollowingUser { get; set; }
    }
}

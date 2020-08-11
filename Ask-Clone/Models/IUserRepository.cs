using Ask_Clone.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public interface IUserRepository
    {

        public List<ApplicationUser> GetAllUsersByName(string user);

        public Follow GetFollowByUsers(ApplicationUser followedUser, ApplicationUser followingUser);

        public void AddFollow(Follow connection);

        public void DeleteFollow(Follow connection);

        public List<ApplicationUser> GetFollowers(ApplicationUser user);

        public List<ApplicationUser> GetFollowing(ApplicationUser user);

        public bool SaveAll();
    }
}

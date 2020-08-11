using Ask_Clone.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class UserRepository: IUserRepository
    {
        private readonly AuthenticationContext _authenticationContext;

        public UserRepository(AuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }

        public List<ApplicationUser> GetAllUsersByName(string user)
        {
            try
            {
                return _authenticationContext.ApplicationUsers
                    .Where(u => u.UserName.Contains(user) || u.FirstName.Contains(user) || u.LastName.Contains(user))
                    .ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Follow GetFollowByUsers(ApplicationUser followedUser, ApplicationUser followingUser)
        {
            try
            {
                return _authenticationContext.Follow
                    .Where(f => f.FollowedUser.UserName == followedUser.UserName && f.FollowingUser.UserName == followingUser.UserName)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddFollow(Follow connection)
        {
            _authenticationContext.Add(connection);
        }

        public void DeleteFollow(Follow connection)
        {
            _authenticationContext.Remove(connection);
        }

        public List<ApplicationUser> GetFollowers(ApplicationUser user)
        {
            try
            {
               return _authenticationContext.Follow
                    .Where(u => u.FollowedUser.UserName == user.UserName)
                    .Select(u => u.FollowingUser).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ApplicationUser> GetFollowing(ApplicationUser user)
        {
            try
            {
                return _authenticationContext.Follow
                    .Where(u => u.FollowingUser.UserName == user.UserName)
                    .Select(u => u.FollowedUser).ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SaveAll()
        {
            return _authenticationContext.SaveChanges() > 0;
        }
    }
}

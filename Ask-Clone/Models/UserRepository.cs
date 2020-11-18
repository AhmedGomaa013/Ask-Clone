using Ask_Clone.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class UserRepository: IUserRepository
    {
        private readonly AuthenticationContext _authenticationContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            AuthenticationContext authenticationContext,
            ILogger<UserRepository> logger
            )
        {
            _authenticationContext = authenticationContext;
            _logger = logger;
        }

        public List<ApplicationUser> GetAllUsersByName(string user)
        {
            try
            {
                return _authenticationContext.ApplicationUsers
                    .Where(u => u.UserName.Contains(user) || u.FirstName.Contains(user) || u.LastName.Contains(user))
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
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
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public void AddFollow(Follow connection)
        {
            try
            {
                _authenticationContext.Add(connection);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
            }
        }

        public void DeleteFollow(Follow connection)
        {
            try
            {
                _authenticationContext.Remove(connection);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
            }
        }

        public List<ApplicationUser> GetFollowers(ApplicationUser user)
        {
            try
            {
               return _authenticationContext.Follow
                    .Where(u => u.FollowedUser.UserName == user.UserName)
                    .Select(u => u.FollowingUser).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
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
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public bool SaveAll()
        {
            try
            {
                return _authenticationContext.SaveChanges() > 0;
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}

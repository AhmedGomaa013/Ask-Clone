using Ask_Clone.Models.Entities;
using Ask_Clone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Controllers
{
    public partial class UserController: ControllerBase
    {

        [HttpGet("Follow/{user}")]
        //Get => api/User/Follow/user
        public async Task<ActionResult> Follow(string user)
        {
            try
            {
                var followedUser = await _userManager.FindByNameAsync(user);
                if (followedUser == null) return StatusCode(StatusCodes.Status404NotFound);

                var followingUsername = User.Claims.First(u => u.Type == "UserName").Value;
                var followingUser = await _userManager.FindByNameAsync(followingUsername);
                if (followingUser == null) return StatusCode(StatusCodes.Status404NotFound);

                if (followedUser.UserName == followingUser.UserName) return StatusCode(StatusCodes.Status400BadRequest);

                Follow connection = _userRepository.GetFollowByUsers(followedUser, followingUser);
                if (connection != null) return StatusCode(StatusCodes.Status400BadRequest);

                connection = new Follow() { FollowedUser = followedUser, FollowingUser = followingUser };
                _userRepository.AddFollow(connection);

                if (_userRepository.SaveAll())
                {
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpGet("Unfollow/{user}")]
        //Get => api/User/Unfollow/user
        public async Task<ActionResult> Unfollow(string user)
        {
            try
            {
                var followedUser = await _userManager.FindByNameAsync(user);
                if (followedUser == null) return StatusCode(StatusCodes.Status404NotFound);

                var followingUsername = User.Claims.First(u => u.Type == "UserName").Value;
                var followingUser = await _userManager.FindByNameAsync(followingUsername);
                if (followingUser == null) return StatusCode(StatusCodes.Status404NotFound);

                if (followedUser.UserName == followingUser.UserName) return StatusCode(StatusCodes.Status400BadRequest);

                Follow connection = _userRepository.GetFollowByUsers(followedUser, followingUser);
                if (connection == null) return StatusCode(StatusCodes.Status404NotFound);

                _userRepository.DeleteFollow(connection);

                if (_userRepository.SaveAll())
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [AllowAnonymous]
        [HttpGet("Followers/{username}")]
        //Get => api/User/Followers/username
        public async Task<ActionResult> GetFollowers(string username)
        {
            try
            {
                var loggedInUser = new ApplicationUser();
                if (User.Identity.IsAuthenticated)
                {
                    var loggedInUsername = User.Claims.First(o => o.Type == "UserName").Value;
                    loggedInUser = await _userManager.FindByNameAsync(loggedInUsername);
                }

                var user = await _userManager.FindByNameAsync(username);
                if (user == null) return StatusCode(StatusCodes.Status404NotFound);

                var users = _userRepository.GetFollowers(user);
                List<UserInfoViewModel> followers = new List<UserInfoViewModel>();
                foreach (var item in users)
                {
                    UserInfoViewModel follower = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName,
                        IsFollowed = false
                    };
                    if (loggedInUser.UserName != null)
                    {
                        if (_userRepository.GetFollowByUsers(item, loggedInUser) != null) follower.IsFollowed = true;
                    }
                    followers.Add(follower);
                }
                followers.OrderBy(o => o.UserName);
                return Ok(followers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [AllowAnonymous]
        [HttpGet("Following/{username}")]
        //Get => api/User/Following
        public async Task<ActionResult> GetFollowing(string username)
        {
            try
            {
                ApplicationUser loggedInUser = new ApplicationUser();
                if (User.Identity.IsAuthenticated)
                {
                    var loggedInUsername = User.Claims.First(o => o.Type == "UserName").Value;
                    loggedInUser = await _userManager.FindByNameAsync(loggedInUsername);
                }

                var user = await _userManager.FindByNameAsync(username);
                if (user == null) return StatusCode(StatusCodes.Status404NotFound);

                var users = _userRepository.GetFollowing(user);
                List<UserInfoViewModel> followingUsers = new List<UserInfoViewModel>();
                foreach (var item in users)
                {
                    UserInfoViewModel followingUser = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName,
                        IsFollowed = false
                    };

                    if (loggedInUser.UserName != null)
                    {
                        if (_userRepository.GetFollowByUsers(item, loggedInUser) != null) followingUser.IsFollowed = true;
                    }

                    followingUsers.Add(followingUser);
                }

                followingUsers.OrderBy(o => o.UserName);
                return Ok(followingUsers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpGet("IsFollowing/{username}")]
        //Get => api/User/IsFollowing/username
        public async Task<ActionResult> IsFollowing(string username)
        {
            try
            {
                var followedUser = await _userManager.FindByNameAsync(username);
                if (followedUser == null) return StatusCode(StatusCodes.Status404NotFound);

                var followingUsername = User.Claims.First(u => u.Type == "UserName").Value;
                var followingUser = await _userManager.FindByNameAsync(followingUsername);
                if (followingUser == null) return StatusCode(StatusCodes.Status404NotFound);

                if (followedUser.UserName == followingUser.UserName) return StatusCode(StatusCodes.Status400BadRequest);

                Follow connection = _userRepository.GetFollowByUsers(followedUser, followingUser);
                if (connection == null)
                {
                    return Ok(false);
                }
                else
                {
                    return Ok(true);
                }

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [AllowAnonymous]
        [HttpGet("FollowingFollowersNumber/{username}")]
        //Get => api/User/FollowingFollowersNumber/username
        public async Task<ActionResult> GetFollowingFollowersNumber(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return StatusCode(StatusCodes.Status404NotFound);

            var followers = _userRepository.GetFollowers(user);
            var following = _userRepository.GetFollowing(user);

            return Ok(new { followers = followers.Count, following = following.Count });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ask_Clone.Models;
using Ask_Clone.Models.Entities;
using Ask_Clone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ask_Clone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IQuestionsRepository _questionsRepository;
        private readonly IUserRepository _userRepository;

        public UserController(UserManager<ApplicationUser> userManager,IQuestionsRepository questionsRepository,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _questionsRepository = questionsRepository;
            _userRepository = userRepository;
        }
        
        [HttpPost]
        [Route("Register")]
        //Post " api/User/Register

        public   async Task<object> Register (UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ApplicationUser = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };

                    var result = await _userManager.CreateAsync(ApplicationUser, model.Password);
                    return Ok(result);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpPost]
        [Route("Login")]
        //Post => api/User/Login

        public async Task<ActionResult> Login(LoginUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    var key = Environment.GetEnvironmentVariable("Token");
                    if ((user != null) && (await _userManager.CheckPasswordAsync(user, model.Password)))
                    {
                        var tokenDecriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                            new Claim("UserName", user.UserName)
                            }),
                            Expires = DateTime.Now.AddHours(2),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                            SecurityAlgorithms.HmacSha256Signature)
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDecriptor);
                        var token = tokenHandler.WriteToken(securityToken);

                        return Ok(new { token });
                    }
                    else
                    {
                        return BadRequest(new { message = "Username or password is incorrect." });
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        //Post => api/User/ChangePassword

        public async Task<ActionResult> ChangePassword(PasswordsViewModel model)
        {
            try
            {
                var username = User.Claims.First(o => o.Type == "UserName").Value;
                var user = await _userManager.FindByNameAsync(username);
                if (user == null) return StatusCode(StatusCodes.Status404NotFound);

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get user data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Search")]
        //Get => api/User/Search?username=

        public async Task<ActionResult> Search([FromQuery]string username)
        {
            try
            {
                var authorizedUsername = User.Claims.First(o => o.Type == "UserName").Value;
                var authorizedUser = await _userManager.FindByNameAsync(authorizedUsername);
                if (authorizedUser == null) return StatusCode(StatusCodes.Status404NotFound);

                var users = _userRepository.GetAllUsersByName(username);
                if (users == null) return StatusCode(StatusCodes.Status404NotFound);

                List<UserInfoViewModel> returnUsers = new List<UserInfoViewModel>();
                foreach (var item in users)
                {
                    if (authorizedUser.UserName == item.UserName) continue;

                    UserInfoViewModel user = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
                    
                    if (_userRepository.GetFollowByUsers(item, authorizedUser) != null) user.IsFollowed = true;
                    else user.IsFollowed = false;

                    returnUsers.Add(user);
                }

                return Ok(returnUsers);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Follow/{user}")]
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
                if(connection != null) return StatusCode(StatusCodes.Status400BadRequest);

                connection = new Follow() { FollowedUser = followedUser, FollowingUser = followingUser };
                _userRepository.AddFollow(connection);

                if(_userRepository.SaveAll())
                {
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Unfollow/{user}")]
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

                if(_userRepository.SaveAll())
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        
        [HttpGet]
        [Route("Followers/{username}")]
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
                foreach(var item in users)
                {
                    UserInfoViewModel follower = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName,
                        IsFollowed = false
                    };
                    if(loggedInUser.UserName != null)
                    {
                        if (_userRepository.GetFollowByUsers(item, loggedInUser) != null) follower.IsFollowed = true;
                    }
                    followers.Add(follower);
                }
                followers.OrderBy(o => o.UserName);
                return Ok(followers);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpGet]
        [Route("Following/{username}")]
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
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("IsFollowing/{username}")]
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
                if(connection == null)
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

        [HttpGet]
        [Route("FollowingFollowersNumber/{username}")]
        //Get => api/User/FollowingFollowersNumber/username
        public async Task<ActionResult> GetFollowingFollowersNumber(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return StatusCode(StatusCodes.Status404NotFound);

            var followers = _userRepository.GetFollowers(user);
            var following = _userRepository.GetFollowing(user);

            return Ok(new { followers = followers.Count, following = following.Count });
        }

        [Authorize]
        [HttpGet]
        [Route("Home/Questions")]
        //Get => api/User/Home/Questions
        public async Task<ActionResult> GetHomeQuestions()
        {
            var username = User.Claims.First(o => o.Type == "UserName").Value;
            var user = await _userManager.FindByNameAsync(username);
            var following = _userRepository.GetFollowing(user);
            following.Add(user);

            List<QuestionsViewModel> returnQuestions = new List<QuestionsViewModel>();
            foreach(var item in following)
            {
                var questions = _questionsRepository.GetAllAnsweredQuestionsByUser(item.UserName);
                foreach(var question in questions)
                {
                    var returnQuestion = new QuestionsViewModel()
                    {
                        QuestionId = question.QuestionId,
                        Answer = question.Answer,
                        Question = question.Question,
                        QuestionTo = question.QuestionTo.UserName,
                        Time = question.Time,
                        IsAnswered = question.IsAnswered
                    };

                    if (question.QuestionFrom != null) returnQuestion.QuestionFrom = question.QuestionFrom.UserName;
                    returnQuestions.Add(returnQuestion);
                }
            }

            return Ok(returnQuestions);
        }
    }
}
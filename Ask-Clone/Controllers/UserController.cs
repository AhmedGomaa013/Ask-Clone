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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IQuestionsRepository _questionsRepository;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IQuestionsRepository questionsRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _questionsRepository = questionsRepository;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to get user data, err:{e}");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Search")]
        //Get => api/User/Search?username=

        public ActionResult Search(string? username)
        {
            try
            {
                var users = _questionsRepository.GetAllUsersByUser(username);
                if (users == null) return StatusCode(StatusCodes.Status404NotFound);

                List<UserInfoViewModel> returnUsers = new List<UserInfoViewModel>();
                foreach (var item in users)
                {
                    UserInfoViewModel user = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
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
                var following = await _userManager.FindByNameAsync(user);
                if (following == null) return NotFound("Couldn't find the User");

                var followerUsername = User.Claims.First(u => u.Type == "UserName").Value;
                var follower = await _userManager.FindByNameAsync(followerUsername);
                if (follower == null) return NotFound("Couldn't find the User");

                if (follower.UserName == following.UserName) return BadRequest("Can't follow yourself!");

                following.Followers.Add(follower);
                follower.Following.Add(following);

                if (_questionsRepository.SaveAll())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Operation Failed!");
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
                var following = await _userManager.FindByNameAsync(user);
                if (following == null) return NotFound("Couldn't find the User");

                var followerUsername = User.Claims.First(u => u.Type == "UserName").Value;
                var follower = await _userManager.FindByNameAsync(followerUsername);
                if (follower == null) return NotFound("Couldn't find the User");

                if (follower.UserName == following.UserName) return BadRequest("Can't unfollow yourself!");

                if (following.Followers.Contains(follower))
                {
                    following.Followers.Remove(follower);
                }
                else
                {
                    return BadRequest("Operation Failed!");
                }

                if (follower.Following.Contains(following))
                {
                    follower.Following.Remove(following);
                }
                else
                {
                    return BadRequest("Operation Failed!");
                }

                if (_questionsRepository.SaveAll())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Operation Failed!");
                }
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Followers")]
        //Get => api/User/Followers
        public ActionResult Followers()
        {
            try
            {
                var username = User.Claims.First(u => u.Type == "UserName").Value;
                var user = _questionsRepository.GetFollowersByUser(username);
                List<UserInfoViewModel> followersInfo = new List<UserInfoViewModel>();

                foreach (var item in user.Followers)
                {
                    var userdata = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
                    followersInfo.Add(userdata);
                }

                return Ok(followersInfo);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Following")]
        //Get => api/User/Following
        public ActionResult Following()
        {
            try
            {
                var username = User.Claims.First(u => u.Type == "UserName").Value;
                var user = _questionsRepository.GetFollowingByUser(username);
                List<UserInfoViewModel> followingInfo = new List<UserInfoViewModel>();

                foreach (var item in user.Following)
                {
                    var userdata = new UserInfoViewModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    };
                    followingInfo.Add(userdata);
                }

                return Ok(followingInfo);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

    }
}
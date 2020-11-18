using Ask_Clone.Models;
using Ask_Clone.Models.Entities;
using Ask_Clone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController: Controller
    {
        private readonly IQuestionsRepository _questionsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(
            IQuestionsRepository questionsRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<QuestionsController> logger
            )
        {
            _questionsRepository = questionsRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("{user}")]
        //Get => api/Questions/user
        public async Task<ActionResult> GetAnsweredQuestions(string user)
        {
            try
            {
                var applicationUser =await _userManager.FindByNameAsync(user);
                if (applicationUser == null) return NotFound("Couldn't find this User");

                var result = _questionsRepository.GetAllAnsweredQuestionsByUser(user);

                List<QuestionsViewModel> answeredQuestions = new List<QuestionsViewModel>();
                foreach(var item in result)
                {
                    QuestionsViewModel question = new QuestionsViewModel()
                    {
                        QuestionId = item.QuestionId,
                        Answer = item.Answer,
                        Question = item.Question,
                        QuestionTo = item.QuestionTo.UserName,
                        Time = item.Time,
                        IsAnswered = item.IsAnswered
                        
                    };
                    if (item.QuestionFrom != null) question.QuestionFrom = item.QuestionFrom.UserName;
                    answeredQuestions.Add(question);
                }
                
                return Ok(answeredQuestions);
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpGet]
        //Get => api/Questions
        public ActionResult GetUnansweredQuestions()
        {
            try
            {
                string user = User.Claims.First(o => o.Type == "UserName").Value;

                var result = _questionsRepository.GetAllUnasweredQuestionsByUser(user);

                List<QuestionsViewModel> questions = new List<QuestionsViewModel>();
                foreach (var item in result)
                {
                    QuestionsViewModel question = new QuestionsViewModel()
                    {
                        Answer = item.Answer,
                        Question = item.Question,
                        QuestionId = item.QuestionId,
                        Time = item.Time,
                        IsAnswered=item.IsAnswered
                    };
                    question.QuestionTo = item.QuestionTo.UserName;
                    if (item.QuestionFrom != null) question.QuestionFrom = item.QuestionFrom.UserName;
                    questions.Add(question);
                }
                return Ok(questions);
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [AllowAnonymous]
        [HttpPost("{user}")]
        //Post => api/Questions/user
        public async Task<ActionResult> Post(QuestionsViewModel model,string user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var questionTo = await _userManager.FindByNameAsync(user);
                    if (questionTo == null) return NotFound("Couldn't find this User");

                    var question = new Questions()
                    {
                        Question = model.Question,
                        QuestionTo = questionTo,
                        Time = DateTime.Now,
                        IsAnswered = false,
                        Answer = null,
                        
                    };
                    
                    if(model.QuestionFrom != null)
                    {
                        var questionFrom = await _userManager.FindByNameAsync(model.QuestionFrom);
                        question.QuestionFrom = questionFrom;
                    }

                    _questionsRepository.AddQuestion(question);

                    if(_questionsRepository.SaveAll())
                    {
                        return Created("api/questions",new { message = "Question Sent" });
                    }
                    else
                    {
                        _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Couldn't save the question into database");
                        return BadRequest("Failed to save your question");
                    }
                }
                else
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Question Format isn't right from Post");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpPut("{id}")]
        //Put => api/Questions/id
        public ActionResult Put(QuestionsViewModel model,int id)
        {
            try
            {
                if ((model.Answer == null) || (model.IsAnswered))
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Question is answered before");
                    return BadRequest(ModelState); 
                }

                if (ModelState.IsValid)
                {
                    string userName = User.Claims.First(o => o.Type == "UserName").Value;

                    var question = _questionsRepository.GetQuestionByUserAndId(userName, id);
                    if (question == null) return NotFound("Couldn't find the Question");

                    question.Answer = model.Answer;
                    question.IsAnswered = true;
                    question.Time = DateTime.Now;

                    if (_questionsRepository.SaveAll())
                    {
                        return Ok();
                    }
                    else
                    {
                        _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Couldn't edit the question in the database");
                        return BadRequest("Failed to save a your question");
                    }
                }
                else
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Question Format isn't right from Put");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve data");
            }
        }

        [HttpDelete("{id}")]
        //Delete => api/Questions/id
        public ActionResult Delete(int id)
        {
            try
            {
                string userName = User.Claims.First(u => u.Type == "UserName").Value;

                Questions question = _questionsRepository.GetQuestionByUserAndId(userName, id);
                if (question == null) return NotFound("Couldn't find the Question");

                _questionsRepository.DeleteQuestion(question);

                if(_questionsRepository.SaveAll())
                {
                    return Ok();
                }
                else
                {
                    _logger.LogWarning($"DateTime: {DateTime.Now} -- Error: Couldn't detele the question from the database");
                    return BadRequest("Failed to delete the Question");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get note");
            }
        }
    }
}

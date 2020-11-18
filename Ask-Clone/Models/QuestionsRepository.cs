using Ask_Clone.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class QuestionsRepository : IQuestionsRepository
    {
        private readonly AuthenticationContext _authenticationContext;
        private readonly ILogger<QuestionsRepository> _logger;

        public QuestionsRepository(
            AuthenticationContext authenticationContext,
            ILogger<QuestionsRepository> logger
            )
        {
            _authenticationContext = authenticationContext;
            _logger = logger;
        }

        public List<Questions> GetAllAnsweredQuestionsByUser(string user)
        {
            try
            {
                return _authenticationContext.Questions
                .Where(q => q.QuestionTo.UserName == user && q.IsAnswered == true)
                .Include(q => q.QuestionTo).Include(q => q.QuestionFrom)
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public List<Questions> GetAllUnasweredQuestionsByUser(string user)
        {
            try
            {
                return _authenticationContext.Questions
                .Where(q => q.QuestionTo.UserName == user && q.IsAnswered == false)
                .Include(q => q.QuestionTo).Include(q=>q.QuestionFrom)
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public Questions GetQuestionByUserAndId(string user, int id)
        {
            try
            {
                return _authenticationContext.Questions
                    .Where(q => q.QuestionTo.UserName == user && q.QuestionId == id).FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public void AddQuestion(Questions question)
        {
            try
            {
                _authenticationContext.Add(question);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
            }
        }

        public void DeleteQuestion(Questions question)
        {
            try
            {
                _authenticationContext.Remove(question);
            }
            catch(Exception e)
            {
                _logger.LogError($"DateTime:{DateTime.Now} -- Error:{e.Message}\n{e.StackTrace}");
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

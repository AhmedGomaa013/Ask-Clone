using Ask_Clone.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class QuestionsRepository : IQuestionsRepository
    {
        private readonly AuthenticationContext _authenticationContext;

        public QuestionsRepository(AuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
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
            catch (Exception)
            {

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
            catch (Exception)
            {

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
            catch (Exception)
            {

                return null;
            }
        }

        public void AddQuestion(Questions question)
        {
            _authenticationContext.Add(question);
        }

        public void DeleteQuestion(Questions question)
        {
            _authenticationContext.Remove(question);
        }

        public bool SaveAll()
        {
            return _authenticationContext.SaveChanges() > 0;
        }
    }
}

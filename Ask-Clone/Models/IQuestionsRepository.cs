using Ask_Clone.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public interface IQuestionsRepository
    {
        public List<Questions> GetAllUnasweredQuestionsByUser(string user);
        public List<Questions> GetAllAnsweredQuestionsByUser(string user);
        public Questions GetQuestionByUserAndId(string user, int id);
        public void AddQuestion(Questions question);
        public void DeleteQuestion(Questions question);

        public bool SaveAll();
    }
}

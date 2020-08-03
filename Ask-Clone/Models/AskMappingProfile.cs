using Ask_Clone.Models.Entities;
using Ask_Clone.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class AskMappingProfile: Profile
    {
        public AskMappingProfile()
        {
            CreateMap<Questions, QuestionsViewModel>()
                .ReverseMap();
        }
    }
}

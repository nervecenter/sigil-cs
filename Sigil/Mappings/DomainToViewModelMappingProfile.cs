using AutoMapper;
using Sigil.Models;
using Sigil.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public new string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Comment, CommentViewModel>();
            Mapper.CreateMap<Issue, IssueViewModel>();
            Mapper.CreateMap<Org, OrgViewModel>();
            
            //Mapper.CreateMap<Org, OrgViewModel>();
            //Mapper.CreateMap<>
        }

    }
}
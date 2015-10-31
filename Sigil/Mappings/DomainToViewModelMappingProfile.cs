using AutoMapper;
using Sigil.Models;
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
            Mapper.CreateMap<Org, OrgViewModel>();
            //Mapper.CreateMap<>
        }

    }
}
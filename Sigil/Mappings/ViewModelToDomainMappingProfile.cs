using AutoMapper;
using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public new string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {

        }
    }
}
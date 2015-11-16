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
            //Mapper.CreateMap
            //Mapper.CreateMap<OrgViewModel, Org>()
            //    .ForMember(o => o.orgName, map => map.MapFrom(vm => vm.OrgName))
            //    .ForMember(o => o.orgURL, map => map.MapFrom(vm => vm.OrgUrl))
            //    .ForMember(o => o.)
        }
    }
}
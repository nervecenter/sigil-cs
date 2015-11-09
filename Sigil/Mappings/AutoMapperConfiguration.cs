using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            AutoMapper.Mapper.Initialize(m =>
            {

                m.AddProfile<DomainToViewModelMappingProfile>(); //<------------ Maps Domain models (db) to Viewmodels

                m.AddProfile<ViewModelToDomainMappingProfile>(); //<------------ Maps ViewModels to Domain models(db)
                
            });
        }
    }
}
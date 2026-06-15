using comentapp.authentication.businessLogic.DTOs;
using comentapp.persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.authentication.businessLogic.Mapper
{
    public class EntityMapperProfile : AutoMapper.Profile
    {
        public EntityMapperProfile()
        {
            CreateMap<ConfirmMailDTO, User>()
                .AfterMap((source, destination) =>
                {

                });
        }
    }
}


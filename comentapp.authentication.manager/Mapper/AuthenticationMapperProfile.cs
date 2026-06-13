using comentapp.persistence.Models;
using Comentapp.AuthenticationManager.Endpoint.DTOs;

namespace Comentapp.AuthenticationManager.Endpoint.Mapper
{
    public class AuthenticationMapperProfile : AutoMapper.Profile
    {
        public AuthenticationMapperProfile()
        {
            CreateMap<Register_Req, User>()
                .AfterMap((source, destination) =>
                {
                    destination.CreatedDate = DateTime.UtcNow;
                    destination.LastModifiedDate = DateTime.UtcNow;

                    destination.PasswordHash = source.Password!;
                });
        }
    }
}

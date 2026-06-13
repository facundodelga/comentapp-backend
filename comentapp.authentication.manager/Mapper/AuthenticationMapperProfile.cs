using Comentapp.AuthenticationManager.Endpoint.DTOs;
using Comentapp.AuthenticationManager.Endpoint.Models;

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

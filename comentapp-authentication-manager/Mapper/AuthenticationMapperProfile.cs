using comentapp_authentication_manager.DTOs;

namespace comentapp_authentication_manager.Mapper
{
    public class AuthenticationMapperProfile : AutoMapper.Profile
    {
        public AuthenticationMapperProfile()
        {
            CreateMap<Register_Req, Models.User>()
                .AfterMap((source, destination) =>
                {
                    destination.CreatedDate = DateTime.UtcNow;
                    destination.LastModifiedDate = DateTime.UtcNow;
                });
        }
    }
}

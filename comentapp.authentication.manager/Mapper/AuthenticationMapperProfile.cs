using comentapp.authentication.businessLogic.DTOs;
using comentapp.persistence.Models;
using Comentapp.AuthenticationManager.Endpoint.DTOs;

namespace Comentapp.AuthenticationManager.Endpoint.Mapper
{
    /// <summary>
    /// AutoMapper profile defining the mappings between HTTP-facing DTOs
    /// (<see cref="Comentapp.AuthenticationManager.Endpoint.DTOs"/>) and the
    /// business-logic DTOs/entities used by the authentication services.
    /// </summary>
    public class AuthenticationMapperProfile : AutoMapper.Profile
    {
        /// <summary>
        /// Registers all authentication-related mapping configurations.
        /// </summary>
        public AuthenticationMapperProfile()
        {
            CreateMap<User, Me_Res>()
                .ForMember(dest => dest.IsCreator, opt => opt.MapFrom(src => src.Creator != null));

            CreateMap<User, Register_Res>();

            CreateMap<Register_Req, RegisterDTO>()
                .ForPath(dest => dest.User.Email, opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.User.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.User.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForPath(dest => dest.User.Surname, opt => opt.MapFrom(src => src.Surname))
                .AfterMap((source, destination) =>
                {
                    destination.User.CreatedAt = DateTime.UtcNow;
                    destination.User.UpdatedAt = DateTime.UtcNow;

                    destination.User.PasswordHash = source.Password!;
                });

            CreateMap<Login_Req, LoginDTO>()
                .AfterMap((source, destination) =>
                {
                    destination.User.Email = source.Email;

                    destination.User.PasswordHash = source.Password!;
                });

            CreateMap<ConfirmEmail_Req, ConfirmMailDTO>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .AfterMap((source, destination) =>
                {
                    destination.User.Email = source.Email;
                });
        }
    }
}

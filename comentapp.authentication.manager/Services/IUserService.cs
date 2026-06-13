using comentapp.persistence.Models;
using Comentapp.AuthenticationManager.Endpoint.Core;
using Comentapp.AuthenticationManager.Endpoint.DTOs;

namespace Comentapp.AuthenticationManager.Endpoint.Services
{
    public interface IUserService
    {
        Task<Result<User>> RegisterUser(Register_Req register);
        Task<Result<User>> LoginUser(Login_Req login);
        Task<Result<User>> ConfirmEmailAsync(ConfirmEmail_Req confirmEmail);
    }
}

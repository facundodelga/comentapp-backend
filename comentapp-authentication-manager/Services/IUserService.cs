using comentapp_authentication_manager.Core;
using comentapp_authentication_manager.DTOs;
using comentapp_authentication_manager.Models;

namespace comentapp_authentication_manager.Services
{
    public interface IUserService
    {
        Task<Result<User>> RegisterUser(Register_Req register);
        Task<Result<User>> LoginUser(Login_Req login);
    }
}

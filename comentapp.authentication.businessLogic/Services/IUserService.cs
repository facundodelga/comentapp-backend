using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.persistence.Models;

namespace comentapp.authentication.businessLogic.Services
{
    public interface IUserService
    {
        Task<Result<User>> RegisterUser(RegisterDTO register);
        Task<Result<User>> LoginUser(LoginDTO login);
        Task<Result<User>> ConfirmEmailAsync(ConfirmMailDTO confirmEmail);
    }
}

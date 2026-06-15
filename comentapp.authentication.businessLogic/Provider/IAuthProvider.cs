using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.authentication.businessLogic.Provider
{
    public interface IAuthProvider
    {
        string ProviderName { get; }
        Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request);
    }
}

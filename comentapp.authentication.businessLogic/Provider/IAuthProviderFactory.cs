using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.authentication.businessLogic.Provider
{
    public interface IAuthProviderFactory
    {
        IAuthProvider GetProvider(string providerName);
    }
}

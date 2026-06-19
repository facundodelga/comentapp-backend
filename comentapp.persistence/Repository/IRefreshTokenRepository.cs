using comentapp.persistence.Models;
using comentapp.persistence.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Repository
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
       public Task<RefreshToken?> GetByTokenAsync(string refreshToken);
       public Task RevokeByIdAsync(int id);
    }
}

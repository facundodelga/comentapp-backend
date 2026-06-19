using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Repository.Implementation
{
    public class RefreshTokenRepository(ComentappDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        }

        public async Task RevokeByIdAsync(int id)
        {
            _context.RefreshTokens.Where(rt => rt.Id == id).ToList().ForEach(rt => rt.IsRevoked = true);
            _context.SaveChanges();
        }


    }


}

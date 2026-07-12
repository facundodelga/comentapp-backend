using comentapp.persistence.Models;
using comentapp.persistence.Repository;

namespace comentapp.business.endpoint.Services
{
    public class CreatorService(
        ICreatorRepository creatorRepository,
        ICreatorMercadoPagoAccountRepository accountRepository) : ICreatorService
    {
        public async Task<CreatorResult> RegisterAsync(int userId, string creatorName)
        {
            var existing = await creatorRepository.GetByUserIdAsync(userId);
            if (existing is not null)
            {
                return new CreatorResult { Success = false, Error = CreatorErrorCode.AlreadyCreator };
            }

            var nameTaken = await creatorRepository.ExistsAsync(c => c.CreatorName == creatorName);
            if (nameTaken)
            {
                return new CreatorResult { Success = false, Error = CreatorErrorCode.NameTaken };
            }

            var creator = new Creator
            {
                UserId = userId,
                CreatorName = creatorName,
            };
            await creatorRepository.AddAsync(creator);
            await creatorRepository.SaveChangesAsync();

            return new CreatorResult { Success = true, Creator = creator, IsConnected = false };
        }

        public async Task<CreatorResult?> GetByUserIdAsync(int userId)
        {
            var creator = await creatorRepository.GetByUserIdAsync(userId);
            if (creator is null)
            {
                return null;
            }

            var account = await accountRepository.GetByCreatorIdAsync(creator.Id);
            return new CreatorResult
            {
                Success = true,
                Creator = creator,
                IsConnected = account?.IsConnected ?? false,
            };
        }
    }
}

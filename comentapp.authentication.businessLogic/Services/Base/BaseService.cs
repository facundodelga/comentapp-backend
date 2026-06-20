using AutoMapper;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Services.Base
{
    public abstract class BaseService<T> (IMapper mapper, ILogger<T> logger) where T : class
    {
        protected IMapper _mapper = mapper;
        protected ILogger<T> _logger = logger;
        
        protected void LogError(string message, Exception ex) => _logger.LogError(ex, message);
        protected void LogInformation(string message) => _logger.LogInformation("{Message}", message);
        protected void LogWarning(string message) => _logger.LogWarning("{Message}", message);
        protected void LogDebug(string message) => _logger.LogDebug("{Message}", message);
    }
}

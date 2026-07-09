using AutoMapper;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Services.Base
{
    /// <summary>
    /// Shared base class for authentication services, providing a consistent
    /// AutoMapper instance and scoped structured-logging helpers.
    /// </summary>
    /// <typeparam name="T">The concrete service type, used as the logging category.</typeparam>
    public abstract class BaseService<T>(IMapper mapper, ILogger<T> logger) where T : class
    {
        /// <summary>AutoMapper instance available to derived services.</summary>
        protected readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        /// <summary>Logger scoped to the derived service type <typeparamref name="T"/>.</summary>
        protected readonly ILogger<T> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>Logs an error with an attached exception.</summary>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception to log alongside the message.</param>
        protected void LogError(string message, Exception ex) => _logger.LogError(ex, message);

        /// <summary>Logs an informational message.</summary>
        /// <param name="message">The log message.</param>
        protected void LogInformation(string message) => _logger.LogInformation("{Message}", message);

        /// <summary>Logs a warning message.</summary>
        /// <param name="message">The log message.</param>
        protected void LogWarning(string message) => _logger.LogWarning("{Message}", message);

        /// <summary>Logs a debug message.</summary>
        /// <param name="message">The log message.</param>
        protected void LogDebug(string message) => _logger.LogDebug("{Message}", message);
    }
}

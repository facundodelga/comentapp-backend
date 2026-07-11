using AutoMapper;
using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services.Base;
using comentapp.infrastructure.Service;
using comentapp.persistence.Models;
using comentapp.persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Services.Implementation
{
    /// <summary>
    /// Default <see cref="IUserService"/> implementation backed by <see cref="IUserRepository"/>,
    /// handling registration, login, email confirmation, and current-user hydration.
    /// </summary>
    public class UserService(
        IMapper mapper,
        ILogger<UserService> logger,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        IUserRepository userRepository,
        IEmailConfirmationService emailConfirmationService,
        ISmtpEmailSender emailSender,
        IEmailTemplateRenderer templateRenderer
        ) : BaseService<UserService>(mapper, logger), IUserService
    {
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService ?? throw new ArgumentNullException(nameof(emailConfirmationService));
        private readonly ISmtpEmailSender _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        private readonly IEmailTemplateRenderer _templateRenderer = templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));

        /// <inheritdoc />
        public async Task<Result<User>> ConfirmEmailAsync(ConfirmMailDTO confirmEmail)
        {
            ArgumentNullException.ThrowIfNull(confirmEmail);

            var confirmEmailDTO = confirmEmail.User.Email;

            LogInformation($"Intentando confirmar el email del usuario: {confirmEmailDTO}");
            var user = await _userRepository.GetByEmailAsync(confirmEmailDTO);

            if (user == null)
            {
                LogInformation("Email no encontrado");
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            var validationToken = _emailConfirmationService.ValidateToken(confirmEmail.Token, confirmEmailDTO);

            if(validationToken == null)
            {
                LogInformation("Token inválido o expirado");
                return Result<User>.Failure("Token inválido o expirado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            if(user.IsEmailConfirmed)
            {
                LogInformation("El email ya ha sido confirmado");
                return Result<User>.Failure("El email ya ha sido confirmado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            user.IsEmailConfirmed = true;

            await _userRepository.UpdateUserAsync(user);

            await _userRepository.SaveChangesAsync();

            return Result<User>.Success(user);
        }

        /// <inheritdoc />
        public async Task<Result<User>> LoginUser(LoginDTO login)
        {
            ArgumentNullException.ThrowIfNull(login);

            var loginUser = login.User;
            var user = await _userRepository.GetByEmailAsync(loginUser.Email);

            if (user == null)
            {
                LogInformation("Email no encontrado");
                return Result<User>.Failure("Usuario no encontrado, revisar email y contraseña", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user,
                                                                      user.PasswordHash,
                                                                      loginUser.PasswordHash
                                                                    );

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                LogInformation("Contraseña incorrecta");
                return Result<User>.Failure("Usuario no encontrado, revisar email y contraseña", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            if(!user.IsEmailConfirmed) {
                LogInformation("El usuario debe confirmar su email");
                return Result<User>.Failure("El email no ha sido confirmado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            return Result<User>.Success(user);
        }

        /// <inheritdoc />
        public async Task<Result<User>> RegisterUser(RegisterDTO register)
        {
            ArgumentNullException.ThrowIfNull(register);

            var newUser = register.User;

            LogInformation($"Intentando crear el usuario: {newUser}");


            var resultEmail = await _userRepository.EmailExistsAsync(newUser.Email);
            if (resultEmail)
            {
                LogInformation("El email ya pertenece a un usuario");
                return Result<User>.Failure("El email ya pertenece a un usuario", (int)UserServiceErrorCodes.CU_EmailAlreadyExists);
            }

            var resultUsername = await _userRepository.UsernameExistsAsync(newUser.UserName!);
            if (resultUsername)
            {
                LogInformation("El nombre de usuario ya pertenece a un usuario");
                return Result<User>.Failure("El nombre de usuario ya pertenece a un usuario", (int)UserServiceErrorCodes.CU_UsernameAlreadyExists);
            }
            try
            {
                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, newUser.PasswordHash);
                var response = await _userRepository.CreateUserAsync(newUser);

                var token = _emailConfirmationService.GenerateToken(newUser.Id, newUser.Email);

                var frontendBaseUrl = _configuration["Frontend:BaseUrl"];

                var confirmationUrl =
                    $"{frontendBaseUrl}/confirm-email" +
                    $"?email={Uri.EscapeDataString(newUser.Email)}" +
                    $"&token={Uri.EscapeDataString(token)}";

                var htmlBody = await _templateRenderer.RenderAsync(
                                    "confirm-email.html",
                                    new Dictionary<string, string>
                                    {
                                        ["ConfirmationUrl"] = confirmationUrl,
                                        ["UserName"] = newUser.Name
                                    }
                                );

                await _emailSender.SendEmailAsync(
                    newUser.Email,
                    "Confirmá tu cuenta",
                    htmlBody
                );

                await _userRepository.SaveChangesAsync();

                return Result<User>.Success(response);

            }
            catch (Exception ex)
            {
                LogError($"Ocurrio un error al crear el usuario {ex.Message}", ex);
                return Result<User>.Failure($"Error al crear el usuario: {ex.Message}");
            }
            
        }

        /// <inheritdoc />
        public async Task<Result<User>> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithCreatorAsync(userId);

            if (user == null)
            {
                LogInformation($"No se encontró el usuario con id: {userId}");
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            return Result<User>.Success(user);
        }

        /// <inheritdoc />
        public async Task<Result<User>> FindOrCreateGoogleUserAsync(string email, string? name, string? surname, string? username)
        {
            ArgumentException.ThrowIfNullOrEmpty(email);

            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                // El email ya viene verificado por Google: si el usuario local todavía
                // no había confirmado su email, lo damos por confirmado para no bloquear
                // el login (regla: un mismo email nunca crea un segundo usuario).
                if (!existingUser.IsEmailConfirmed)
                {
                    existingUser.IsEmailConfirmed = true;
                    await _userRepository.UpdateUserAsync(existingUser);
                    await _userRepository.SaveChangesAsync();

                    LogInformation($"Email confirmado automáticamente vía Google: {email}");
                }

                return Result<User>.Success(existingUser);
            }

            var userNameBase = string.IsNullOrWhiteSpace(username) ? name : username;
            userNameBase = string.IsNullOrWhiteSpace(userNameBase) ? email : userNameBase;
            var generatedUserName = $"{userNameBase}{Random.Shared.Next(1, 10000):D4}";

            var newUser = new User
            {
                Email = email,
                Name = string.IsNullOrWhiteSpace(name) ? email : name,
                Surname = surname ?? string.Empty,
                UserName = generatedUserName,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Las cuentas creadas por Google no requieren password propio: se guarda un
            // hash inutilizable (contraseña aleatoria) para que el login local nunca
            // pueda tener éxito con este usuario.
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, Guid.NewGuid().ToString("N"));

            var created = await _userRepository.CreateUserAsync(newUser);
            await _userRepository.SaveChangesAsync();

            LogInformation($"Usuario creado desde Google: {email}");

            return Result<User>.Success(created);
        }
    }
}

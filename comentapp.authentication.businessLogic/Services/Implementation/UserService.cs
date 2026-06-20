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
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;
        private readonly ISmtpEmailSender _emailSender = emailSender;
        private readonly IEmailTemplateRenderer _templateRenderer = templateRenderer;

        public async Task<Result<User>> ConfirmEmailAsync(ConfirmMailDTO confirmEmail)
        {
            var confirmEmailDTO = confirmEmail.User.Email;

            LogInformation($"Intentando confirmar el email del usuario: {confirmEmailDTO}");
            var user = await _userRepository.GetByEmailAsync(confirmEmailDTO);

            if (user == null)
            {
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }
            LogInformation($"TOKEN RECIBIDO: {confirmEmail.Token}");
            LogInformation($"TOKEN RECIBIDO LENGTH: {confirmEmail.Token.Length}");
            var validationToken = _emailConfirmationService.ValidateToken(confirmEmail.Token, confirmEmailDTO);

            if(validationToken == null)
            {
                return Result<User>.Failure("Token inválido o expirado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            if(user.IsEmailConfirmed)
            {
                return Result<User>.Failure("El email ya ha sido confirmado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            user.IsEmailConfirmed = true;

            await _userRepository.UpdateUserAsync(user);

            await _userRepository.SaveChangesAsync();

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> LoginUser(LoginDTO login)
        {
            var loginUser = login.User;
            var user = await _userRepository.GetByEmailAsync(loginUser.Email);

            if (user == null)
            {
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user,
                                                                      user.PasswordHash,
                                                                      loginUser.PasswordHash
                                                                    );

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                return Result<User>.Failure("Contraseña incorrecta", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> RegisterUser(RegisterDTO register)
        {
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
                LogDebug($"Ocurrio un error al crear el usuario {ex.Message}");
                return Result<User>.Failure($"Error al crear el usuario: {ex.Message}");
            }
            
        }
    }
}

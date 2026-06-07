using AutoMapper;
using comentapp_authentication_manager.Core;
using comentapp_authentication_manager.DTOs;
using comentapp_authentication_manager.Models;
using comentapp_authentication_manager.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace comentapp_authentication_manager.Services.Implementation
{
    public class UserService(
        IMapper mapper,
        ILogger<UserService> logger, 
        IPasswordHasher<User> passwordHasher, 
        IConfiguration configuration,
        IUserRepository userRepository,
        IEmailConfirmationService emailConfirmationService,
        IEmailSender emailSender,
        IEmailTemplateRenderer templateRenderer
        ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;
        private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IEmailTemplateRenderer _templateRenderer = templateRenderer;

        public async Task<Result<User>> ConfirmEmailAsync(ConfirmEmail_Req confirmEmail)
        {
            _logger.LogInformation($"Intentando confirmar el email del usuario: {confirmEmail.Email}");
            var user = await _userRepository.GetByEmailAsync(confirmEmail.Email);

            if (user == null)
            {
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }
            _logger.LogInformation("TOKEN RECIBIDO: {Token}", confirmEmail.Token);
            _logger.LogInformation("TOKEN RECIBIDO LENGTH: {Length}", confirmEmail.Token.Length);
            var validationToken = _emailConfirmationService.ValidateToken(confirmEmail.Token, confirmEmail.Email);

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

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> LoginUser(Login_Req login)
        {
            var user = await _userRepository.GetByEmailAsync(login.Email);

            if (user == null)
            {
                return Result<User>.Failure("Usuario no encontrado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user,
                                                                      user.PasswordHash,
                                                                      login.Password
                                                                    );

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                return Result<User>.Failure("Contraseña incorrecta", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> RegisterUser(Register_Req register)
        {
            var newUser = _mapper.Map<User>(register);
            _logger.LogInformation($"Intentando crear el usuario: {newUser}");


            var resultEmail = await _userRepository.EmailExistsAsync(newUser.Email);
            if (resultEmail)
            {
                _logger.LogInformation("El email ya pertenece a un usuario");
                return Result<User>.Failure("El email ya pertenece a un usuario", (int)UserServiceErrorCodes.CU_EmailAlreadyExists);
            }

            var resultUsername = await _userRepository.UsernameExistsAsync(newUser.UserName!);
            if (resultUsername)
            {
                _logger.LogInformation("El nombre de usuario ya pertenece a un usuario");
                return Result<User>.Failure("El nombre de usuario ya pertenece a un usuario", (int)UserServiceErrorCodes.CU_UsernameAlreadyExists);
            }
            try
            {
                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, register.Password!);
                var response = await _userRepository.CreateUserAsync(newUser);

                var token = _emailConfirmationService.GenerateToken(newUser.Id, newUser.Email);
                _logger.LogInformation("TOKEN GENERADO: {Token}", token);
                _logger.LogInformation("TOKEN GENERADO LENGTH: {Length}", token.Length);

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

                return Result<User>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogDebug("Ocurrio un error al crear el usuario {message}", ex.Message);
                return Result<User>.Failure($"Error al crear el usuario: {ex.Message}");
            }

        }
    }
}

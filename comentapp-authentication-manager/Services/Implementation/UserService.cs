using AutoMapper;
using comentapp_authentication_manager.Core;
using comentapp_authentication_manager.DTOs;
using comentapp_authentication_manager.Models;
using comentapp_authentication_manager.Repository;

namespace comentapp_authentication_manager.Services.Implementation
{
    public class UserService(IMapper mapper,ILogger<UserService> logger, IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;
        public Task<Result<User>> LoginUser(Login_Req login)
        {
            throw new NotImplementedException();
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
                var response = await _userRepository.CreateUserAsync(newUser);
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

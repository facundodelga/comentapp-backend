using FluentValidation;

namespace Comentapp.AuthenticationManager.Endpoint.DTOs
{
    public class Login_Req
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Login_Req_Validation : AbstractValidator<Login_Req>
    {
        public Login_Req_Validation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
        }
    }
}

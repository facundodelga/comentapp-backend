using FluentValidation;

namespace comentapp_authentication_manager.DTOs
{
    public class Register_Req
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? UserName { get; set; }

    }

    public class Register_Req_Validation : AbstractValidator<Register_Req>
    {
        public Register_Req_Validation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("La confirmación de la contraseña es obligatoria.")
                .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.");
            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("El apellido es obligatorio.");
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.");
        }
    }
}

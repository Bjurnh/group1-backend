using FluentValidation;
using Register = Group1.BackEnd.Application.DTOs.RegisterDto;


namespace Group1.BackEnd.Validators
{
    public class UserValidator : AbstractValidator<Register>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email format.");
        }   
    }
}

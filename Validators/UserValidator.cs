using FluentValidation;
using Register = WebApiWithRoleAuthentication.Models.Register;


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

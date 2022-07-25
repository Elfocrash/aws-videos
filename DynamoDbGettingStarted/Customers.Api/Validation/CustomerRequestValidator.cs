using Customers.Api.Contracts.Requests;
using FluentValidation;

namespace Customers.Api.Validation;

public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.GitHubUsername).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}

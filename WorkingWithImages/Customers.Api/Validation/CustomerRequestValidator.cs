using System.Text.RegularExpressions;
using Customers.Api.Contracts.Requests;
using FluentValidation;

namespace Customers.Api.Validation;

public partial class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FullName)
            .Matches(FullNameRegex());

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.GitHubUsername)
            .Matches(UsernameRegex());

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now)
            .WithMessage("Your date of birth cannot be in the future");
    }

    [GeneratedRegex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex FullNameRegex();
    
    [GeneratedRegex("^[a-z\\d](?:[a-z\\d]|-(?=[a-z\\d])){0,38}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex UsernameRegex();
}

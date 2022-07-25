using Customers.Api.Contracts.Data;
using Customers.Api.Domain;
using Customers.Api.Domain.Common;

namespace Customers.Api.Mapping;

public static class DtoToDomainMapper
{
    public static Customer ToCustomer(this CustomerDto customerDto)
    {
        return new Customer
        {
            Id = CustomerId.From(Guid.Parse(customerDto.Id)),
            Email = Email.From(customerDto.Email),
            GitHubUsername = GitHubUsername.From(customerDto.GitHubUsername),
            FullName = FullName.From(customerDto.FullName),
            DateOfBirth = DateOfBirth.From(DateOnly.FromDateTime(customerDto.DateOfBirth))
        };
    }
}

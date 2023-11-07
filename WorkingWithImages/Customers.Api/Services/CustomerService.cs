using Customers.Api.Domain;
using Customers.Api.Mapping;
using Customers.Api.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IGitHubService _gitHubService;

    public CustomerService(ICustomerRepository customerRepository, 
        IGitHubService gitHubService)
    {
        _customerRepository = customerRepository;
        _gitHubService = gitHubService;
    }

    public async Task<bool> CreateAsync(Customer customer)
    {
        var existingUser = await _customerRepository.GetAsync(customer.Id);
        if (existingUser is not null)
        {
            var message = $"A user with id {customer.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(nameof(Customer), message));
        }

        var isValidGitHubUser = await _gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        var customerDto = customer.ToCustomerDto();
        return await _customerRepository.CreateAsync(customerDto);
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        var customerDto = await _customerRepository.GetAsync(id);
        return customerDto?.ToCustomer();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customerDtos = await _customerRepository.GetAllAsync();
        return customerDtos.Select(x => x.ToCustomer());
    }

    public async Task<bool> UpdateAsync(Customer customer, DateTime requestStarted)
    {
        var customerDto = customer.ToCustomerDto();
        
        var isValidGitHubUser = await _gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        return await _customerRepository.UpdateAsync(customerDto, requestStarted);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _customerRepository.DeleteAsync(id);
    }

    private static ValidationFailure[] GenerateValidationError(string paramName, string message)
    {
        return new []
        {
            new ValidationFailure(paramName, message)
        };
    }
}

public interface ICustomerService
{
    Task<bool> CreateAsync(Customer customer);

    Task<Customer?> GetAsync(Guid id);

    Task<IEnumerable<Customer>> GetAllAsync();

    Task<bool> UpdateAsync(Customer customer, DateTime requestStarted);

    Task<bool> DeleteAsync(Guid id);
}

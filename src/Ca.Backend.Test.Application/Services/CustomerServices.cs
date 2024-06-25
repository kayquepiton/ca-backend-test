using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;

namespace Ca.Backend.Test.Application.Services;

public class CustomerServices : ICustomerServices
{
    private readonly IGenericRepository<CustomerEntity> _repository;

    public CustomerServices(IGenericRepository<CustomerEntity> repository)
    {
        _repository = repository;
    }
    public async Task<CustomerResponse> CreateAsync(CustomerRequest customerRequest)
    {
        var customerEntity = new CustomerEntity(){
            Name = customerRequest.Name,
            Email = customerRequest.Email,
            Address = customerRequest.Address
        };

        customerEntity = await _repository.CreateAsync(customerEntity);
        
        return new CustomerResponse()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Email = customerEntity.Email,
            Address = customerEntity.Address
        };
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id)
    {
        var customerEntity = await _repository.GetByIdAsync(id);

        if(customerEntity is null)
            throw new ApplicationException($"Customer with ID {id} not found.");

        return new CustomerResponse()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Email = customerEntity.Email,
            Address = customerEntity.Address
        };
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customersEntities = await _repository.GetAllAsync();

        var customerResponses = new List<CustomerResponse>();
        foreach(var customerEntity in customersEntities)
        {
            customerResponses.Add(new CustomerResponse()
            {
                Id = customerEntity.Id,
                Name = customerEntity.Name,
                Email = customerEntity.Email,
                Address = customerEntity.Address
            });
        }

        return customerResponses;

    }

    public async Task<CustomerResponse> UpdateAsync(Guid id, CustomerRequest customerRequest)
    {
        var customerEntity = await _repository.GetByIdAsync(id);

        if(customerEntity is null)
            throw new ApplicationException($"Customer with ID {id} not found.");

        customerEntity.Name = customerRequest.Name;
        customerEntity.Email = customerRequest.Email;
        customerEntity.Address = customerRequest.Address;

        customerEntity = await _repository.UpdateAsync(customerEntity);

        return new CustomerResponse()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Email = customerEntity.Email,
            Address = customerEntity.Address
        };
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var customerEntity = await _repository.GetByIdAsync(id);
        if(customerEntity is null)
            throw new ApplicationException($"Customer with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }
}

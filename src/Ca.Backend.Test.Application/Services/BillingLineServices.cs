using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;

namespace Ca.Backend.Test.Application.Services;
public class BillingLineServices : IBillingLineServices
{
    private readonly IGenericRepository<BillingLineEntity> _repository;

    public BillingLineServices(IGenericRepository<BillingLineEntity> repository)
    {
        _repository = repository;
    }
    public async Task<BillingLineResponse> CreateAsync(BillingLineRequest billingLineRequest)
    {
        var billingLineEntity = new BillingLineEntity
        {
            Description = billingLineRequest.Description,
            Quantity = billingLineRequest.Quantity,
            UnitPrice = billingLineRequest.UnitPrice,
            Subtotal = billingLineRequest.Subtotal
        };

        billingLineEntity = await _repository.CreateAsync(billingLineEntity);

        return new BillingLineResponse()
        {
            Id = billingLineEntity.Id,
            Description = billingLineEntity.Description,
            Quantity = billingLineEntity.Quantity,
            UnitPrice = billingLineEntity.UnitPrice,
            Subtotal = billingLineEntity.Subtotal
        };
    }

    public async Task<BillingLineResponse> GetByIdAsync(Guid id)
    {
        var billingLineEntity = await _repository.GetByIdAsync(id);

        if(billingLineEntity is null)
            throw new ApplicationException($"Billing Line with ID {id} not found.");

        return new BillingLineResponse()
        {
            Id = billingLineEntity.Id,
            Description = billingLineEntity.Description,
            Quantity = billingLineEntity.Quantity,
            UnitPrice = billingLineEntity.UnitPrice,
            Subtotal = billingLineEntity.Subtotal
        };
    }

    public async Task<IEnumerable<BillingLineResponse>> GetAllAsync()
    {
        var billingLineEntities = await _repository.GetAllAsync();

        var billingLineResponses = new List<BillingLineResponse>();

        foreach(var billingLineEntity in billingLineEntities)
        {
            billingLineResponses.Add(new BillingLineResponse()
            {
                Id = billingLineEntity.Id,
                Description = billingLineEntity.Description,
                Quantity = billingLineEntity.Quantity,
                UnitPrice = billingLineEntity.UnitPrice,
                Subtotal = billingLineEntity.Subtotal
            });
        };

        return billingLineResponses;
    }

    public async Task<BillingLineResponse> UpdateAsync(Guid id, BillingLineRequest billingLineRequest)
    {
        var billingLineEntity = new BillingLineEntity
        {
            Id = id,
            Description = billingLineRequest.Description,
            Quantity = billingLineRequest.Quantity,
            UnitPrice = billingLineRequest.UnitPrice,
            Subtotal = billingLineRequest.Subtotal
        };

        billingLineEntity = await _repository.UpdateAsync(billingLineEntity);

        return new BillingLineResponse()
        {
            Id = billingLineEntity.Id,
            Description = billingLineEntity.Description,
            Quantity = billingLineEntity.Quantity,
            UnitPrice = billingLineEntity.UnitPrice,
            Subtotal = billingLineEntity.Subtotal
        };
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var billingLineEntity = await _repository.GetByIdAsync(id);

        if(billingLineEntity is null)
            throw new ApplicationException($"Billing Line with ID {id} not found.");
        
        await _repository.DeleteByIdAsync(id);
    }
}

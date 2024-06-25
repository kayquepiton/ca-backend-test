using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;

namespace Ca.Backend.Test.Application.Services;
public class BillingServices : IBillingServices
{
    private readonly IGenericRepository<BillingEntity> _repository;

    public BillingServices(IGenericRepository<BillingEntity> repository)
    {
        _repository = repository;
    }
    public async Task<BillingResponse> CreateAsync(BillingRequest billingRequest)
    {
        var billingEntity = new BillingEntity
        {
            InvoiceNumber = billingRequest.InvoiceNumber,
            Date = billingRequest.Date,
            DueDate = billingRequest.DueDate,
            TotalAmount = billingRequest.TotalAmount,
            Currency = billingRequest.Currency
        };

        billingEntity = await _repository.CreateAsync(billingEntity);

        return new BillingResponse()
        {
            Id = billingEntity.Id,
            InvoiceNumber = billingEntity.InvoiceNumber,
            Date = billingEntity.Date,
            DueDate = billingEntity.DueDate,
            TotalAmount = billingEntity.TotalAmount,
            Currency = billingEntity.Currency
        };
    }

    public async Task<BillingResponse> GetByIdAsync(Guid id)
    {
        var billingEntity = await _repository.GetByIdAsync(id);

        if(billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        return new BillingResponse()
        {
            Id = billingEntity.Id,
            InvoiceNumber = billingEntity.InvoiceNumber,
            Date = billingEntity.Date,
            DueDate = billingEntity.DueDate,
            TotalAmount = billingEntity.TotalAmount,
            Currency = billingEntity.Currency
        };
    }

    public async Task<IEnumerable<BillingResponse>> GetAllAsync()
    {
        var billingsEntities = await _repository.GetAllAsync();

        var billingResponses = new List<BillingResponse>();

        foreach(var billingEntity in billingsEntities)
        {
            billingResponses.Add(new BillingResponse()
            {
                Id = billingEntity.Id,
                InvoiceNumber = billingEntity.InvoiceNumber,
                Date = billingEntity.Date,
                DueDate = billingEntity.DueDate,
                TotalAmount = billingEntity.TotalAmount,
                Currency = billingEntity.Currency
            });
        }

        return billingResponses;
    }

    public async Task<BillingResponse> UpdateAsync(Guid id, BillingRequest billingRequest)
    {
        var billingEntity = await _repository.GetByIdAsync(id);

        if(billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        billingEntity.InvoiceNumber = billingRequest.InvoiceNumber;
        billingEntity.Date = billingRequest.Date;
        billingEntity.DueDate = billingRequest.DueDate;
        billingEntity.TotalAmount = billingRequest.TotalAmount;
        billingEntity.Currency = billingRequest.Currency;

        billingEntity = await _repository.UpdateAsync(billingEntity);

        return new BillingResponse()
        {
            Id = billingEntity.Id,
            InvoiceNumber = billingEntity.InvoiceNumber,
            Date = billingEntity.Date,
            DueDate = billingEntity.DueDate,
            TotalAmount = billingEntity.TotalAmount,
            Currency = billingEntity.Currency
        };
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var billingEntity = await _repository.GetByIdAsync(id);

        if(billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }
}

using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentValidation;

namespace Ca.Backend.Test.Application.Services;
public class BillingLineServices : IBillingLineServices
{
    private readonly IGenericRepository<BillingLineEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<BillingLineRequest> _billingLineRequestValidator;

    public BillingLineServices(IGenericRepository<BillingLineEntity> repository,
                                IMapper mapper, IValidator<BillingLineRequest> billingLineRequestValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _billingLineRequestValidator = billingLineRequestValidator;
    }

    public async Task<BillingLineResponse> CreateAsync(BillingLineRequest billingLineRequest)
    {
        var validationResult = await _billingLineRequestValidator.ValidateAsync(billingLineRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var billingLineEntity = _mapper.Map<BillingLineEntity>(billingLineRequest);
        billingLineEntity = await _repository.CreateAsync(billingLineEntity);
        return _mapper.Map<BillingLineResponse>(billingLineEntity);
    }

    public async Task<BillingLineResponse> GetByIdAsync(Guid id)
    {
        var billingLineEntity = await _repository.GetByIdAsync(id);

        if (billingLineEntity is null)
            throw new ApplicationException($"Billing Line with ID {id} not found.");

        return _mapper.Map<BillingLineResponse>(billingLineEntity);
    }

    public async Task<IEnumerable<BillingLineResponse>> GetAllAsync()
    {
        var billingLineEntities = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<BillingLineResponse>>(billingLineEntities);
    }

    public async Task<BillingLineResponse> UpdateAsync(Guid id, BillingLineRequest billingLineRequest)
    {
        var validationResult = await _billingLineRequestValidator.ValidateAsync(billingLineRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var billingLineEntity = await _repository.GetByIdAsync(id);
        if (billingLineEntity is null)
            throw new ApplicationException($"Billing Line with ID {id} not found.");

        _mapper.Map(billingLineRequest, billingLineEntity);
        billingLineEntity = await _repository.UpdateAsync(billingLineEntity);
        return _mapper.Map<BillingLineResponse>(billingLineEntity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var billingLineEntity = await _repository.GetByIdAsync(id);

        if (billingLineEntity is null)
            throw new ApplicationException($"Billing Line with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }
}


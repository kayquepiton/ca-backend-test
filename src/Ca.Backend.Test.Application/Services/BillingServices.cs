using System.Text.Json;
using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Models.Responses;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentValidation;

namespace Ca.Backend.Test.Application.Services;
public class BillingServices : IBillingServices
{
    private readonly IGenericRepository<BillingEntity> _repository;
    private readonly IGenericRepository<CustomerEntity> _customerRepository;
    private readonly IGenericRepository<ProductEntity> _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<BillingRequest> _billingRequestValidator;
    private readonly HttpClient _httpClient;

    public BillingServices(IGenericRepository<BillingEntity> repository,
                            IGenericRepository<CustomerEntity> customerRepository,
                            IMapper mapper, IValidator<BillingRequest> billingRequestValidator,
                            IGenericRepository<ProductEntity> productRepository,
                            HttpClient httpClient)
    {
        _repository = repository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _billingRequestValidator = billingRequestValidator;
        _productRepository = productRepository;
        _httpClient = httpClient;
    }

    public async Task ImportBillingFromExternalApiAsync()
    {
        try
        {
            // Fetch billing data from external API
            var response = await _httpClient.GetAsync("https://65c3b12439055e7482c16bca.mockapi.io/api/v1/billing");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var billingData = JsonSerializer.Deserialize<List<BillingApiResponse>>(responseContent);

            if (billingData is null || !billingData.Any())
                throw new ApplicationException("No billing data found in the API.");

            var firstBilling = billingData.First();

            // Validate customer
            var customerEntity = await _customerRepository.GetByIdAsync(Guid.Parse(firstBilling.Customer.Id));
            if (customerEntity is null)
                throw new ApplicationException($"Customer with ID {firstBilling.Customer.Id} not found.");

            // Validate products
            foreach (var line in firstBilling.Lines)
            {
                var productEntity = await _productRepository.GetByIdAsync(Guid.Parse(line.ProductId));
                if (productEntity is null)
                    throw new ApplicationException($"Product with ID {line.ProductId} not found.");
            }

            // Map and save billing data using AutoMapper
            var billingEntity = _mapper.Map<BillingEntity>(firstBilling);
            await _repository.CreateAsync(billingEntity);
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException("Error fetching data from external API.", ex);
        }
        catch (JsonException ex)
        {
            throw new ApplicationException("Error deserializing data from external API.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while importing billing data.", ex);
        }
    }

    public async Task<BillingResponse> CreateAsync(BillingRequest billingRequest)
    {
        var validationResult = await _billingRequestValidator.ValidateAsync(billingRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customerEntity = await _customerRepository.GetByIdAsync(billingRequest.CustomerId);
        if (customerEntity is null)
            throw new ApplicationException($"Customer with ID {billingRequest.CustomerId} not found.");

        foreach(var line in billingRequest.Lines)
        {
            var productEntity = await _productRepository.GetByIdAsync(line.ProductId);
            if(productEntity is null)
                throw new ApplicationException($"Product with ID {line.ProductId} not found.");
        }
                
        var billingEntity = _mapper.Map<BillingEntity>(billingRequest);
        billingEntity = await _repository.CreateAsync(billingEntity);
        return _mapper.Map<BillingResponse>(billingEntity);
    }

    public async Task<BillingResponse> GetByIdAsync(Guid id)
    {
        var billingEntity = await _repository.GetByIdAsync(id);

        if (billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        return _mapper.Map<BillingResponse>(billingEntity);
    }

    public async Task<IEnumerable<BillingResponse>> GetAllAsync()
    {
        var billingEntities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<BillingResponse>>(billingEntities);
    }

    public async Task<BillingResponse> UpdateAsync(Guid id, BillingRequest billingRequest)
    {
        var validationResult = await _billingRequestValidator.ValidateAsync(billingRequest);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var billingEntity = await _repository.GetByIdAsync(id);
        if (billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        _mapper.Map(billingRequest, billingEntity);
        billingEntity = await _repository.UpdateAsync(billingEntity);
        return _mapper.Map<BillingResponse>(billingEntity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var billingEntity = await _repository.GetByIdAsync(id);

        if (billingEntity is null)
            throw new ApplicationException($"Billing with ID {id} not found.");

        await _repository.DeleteByIdAsync(id);
    }

}


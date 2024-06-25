using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;

namespace Ca.Backend.Test.Application.Services;

public class ProductServices : IProductServices
{
    private readonly IGenericRepository<ProductEntity> _repository;

    public ProductServices(IGenericRepository<ProductEntity> repository)
    {
        _repository = repository;
    }
    public async Task<ProductResponse> CreateAsync(ProductRequest productRequest)
    {
        var productEntity = new ProductEntity()
        {
            Description = productRequest.Description
        };

        productEntity = await _repository.CreateAsync(productEntity);

        return new ProductResponse()
        {
            Id = productEntity.Id,
            Description = productEntity.Description
        };
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var productEntity = await _repository.GetByIdAsync(id);

        if(productEntity is null)
            throw new ApplicationException($"Product with ID {id} not found.");

        return new ProductResponse()
        {
            Id = productEntity.Id,
            Description = productEntity.Description 
        };
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var productEntities = await _repository.GetAllAsync();

        var productResponses = new List<ProductResponse>();
        foreach(var productEntity in productEntities)
        {
            productResponses.Add(new ProductResponse(){
                Id = productEntity.Id,
                Description = productEntity.Description
            });
        }

        return productResponses;
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, ProductRequest productRequest)
    {
        var productEntity = await _repository.GetByIdAsync(id);

        if(productEntity is null)
            throw new ApplicationException($"Product with ID {id} not found.");
        
        productEntity.Description = productRequest.Description;

        productEntity = await _repository.UpdateAsync(productEntity);

        return new ProductResponse()
        {
            Id = productEntity.Id,
            Description = productEntity.Description
        };
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var productEntity = await _repository.GetByIdAsync(id);

        if(productEntity is null)
            throw new ApplicationException($"Product with ID {id} not found.");
        
        await _repository.DeleteByIdAsync(id);
    }

}

using AutoMapper;
using Ca.Backend.Test.Application.Mappings;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Services;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;

namespace Ca.Backend.Test.Application.Tests.Services;
[TestFixture]
public class ProductServicesTests
{
    private IProductServices _productService;
    private Mock<IGenericRepository<ProductEntity>> _mockRepository;
    private IMapper _mapper;
    private Mock<IValidator<ProductRequest>> _mockValidator;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IGenericRepository<ProductEntity>>();
        _mockValidator = new Mock<IValidator<ProductRequest>>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        _mapper = mapperConfig.CreateMapper();

        _productService = new ProductServices(_mockRepository.Object, _mapper, _mockValidator.Object);
    }

    [Test]
    public async Task CreateAsync_ValidProductRequest_ReturnsProductResponse()
    {
        // Arrange
        var productRequest = new ProductRequest
        {
            Description = "Product Description"
        };

        _mockValidator.Setup(v => v.ValidateAsync(productRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var createdProductEntity = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Description = productRequest.Description
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<ProductEntity>()))
            .ReturnsAsync(createdProductEntity);

        // Act
        var result = await _productService.CreateAsync(productRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdProductEntity.Id);
        result.Description.Should().Be(productRequest.Description);
    }

    [Test]
    public async Task CreateAsync_InvalidProductRequest_ThrowsValidationException()
    {
        // Arrange
        var productRequest = new ProductRequest(); // Invalid because required fields are not set

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Description", "Description is required"));
        _mockValidator.Setup(v => v.ValidateAsync(productRequest, default))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> action = async () => await _productService.CreateAsync(productRequest);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task GetByIdAsync_ExistingProductId_ReturnsProductResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProductEntity = new ProductEntity
        {
            Id = productId,
            Description = "Existing Product Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProductEntity);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProductEntity.Id);
        result.Description.Should().Be(existingProductEntity.Description);
    }

    [Test]
    public async Task GetByIdAsync_NonExistingProductId_ThrowsApplicationException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((ProductEntity)null);

        // Act
        Func<Task> action = async () => await _productService.GetByIdAsync(productId);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Product with ID {productId} not found.");
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var productsEntities = new List<ProductEntity>
        {
            new ProductEntity { Id = Guid.NewGuid(), Description = "Product 1" },
            new ProductEntity { Id = Guid.NewGuid(), Description = "Product 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(productsEntities);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Description.Should().Be("Product 1");
        result.Last().Description.Should().Be("Product 2");
    }

    [Test]
    public async Task GetAllAsync_NoProducts_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ProductEntity>());

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task UpdateAsync_ValidProductRequest_ReturnsUpdatedProductResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productRequest = new ProductRequest
        {
            Description = "Updated Product Description"
        };

        _mockValidator.Setup(v => v.ValidateAsync(productRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var existingProductEntity = new ProductEntity
        {
            Id = productId,
            Description = "Old Product Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProductEntity);

        var updatedProductEntity = new ProductEntity
        {
            Id = productId,
            Description = productRequest.Description
        };

        _mockRepository.Setup(r => r.UpdateAsync(existingProductEntity))
            .ReturnsAsync(updatedProductEntity);

        // Act
        var result = await _productService.UpdateAsync(productId, productRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Description.Should().Be(productRequest.Description);
    }

    [Test]
    public async Task UpdateAsync_NonExistingProductId_ThrowsApplicationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productRequest = new ProductRequest
        {
            Description = "Updated Product Description"
        };

        _mockValidator.Setup(v => v.ValidateAsync(productRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((ProductEntity)null);

        // Act
        Func<Task> action = async () => await _productService.UpdateAsync(productId, productRequest);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Product with ID {productId} not found.");
    }

    [Test]
    public async Task DeleteByIdAsync_ExistingProductId_CallsRepositoryDelete()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProductEntity = new ProductEntity
        {
            Id = productId,
            Description = "Product Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProductEntity);

        // Act
        await _productService.DeleteByIdAsync(productId);

        // Assert
        _mockRepository.Verify(r => r.DeleteByIdAsync(productId), Times.Once);
    }

    [Test]
    public async Task DeleteByIdAsync_NonExistingProductId_ThrowsApplicationException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((ProductEntity)null);

        // Act
        Func<Task> action = async () => await _productService.DeleteByIdAsync(productId);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Product with ID {productId} not found.");
    }
}

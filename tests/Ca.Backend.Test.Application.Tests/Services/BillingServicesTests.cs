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
public class BillingServicesTests
{
    private IBillingServices _billingService;
    private Mock<IGenericRepository<BillingEntity>> _mockRepository;
    private Mock<IGenericRepository<CustomerEntity>> _mockCustomerRepository;
    private Mock<IGenericRepository<ProductEntity>> _mockProductRepository;
    private IMapper _mapper;
    private Mock<IValidator<BillingRequest>> _mockValidator;
    private Mock<HttpClient> _mockHttpClient;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IGenericRepository<BillingEntity>>();
        _mockCustomerRepository = new Mock<IGenericRepository<CustomerEntity>>();
        _mockProductRepository = new Mock<IGenericRepository<ProductEntity>>();
        _mockValidator = new Mock<IValidator<BillingRequest>>();
        _mockHttpClient = new Mock<HttpClient>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        _mapper = mapperConfig.CreateMapper();

        _billingService = new BillingServices(
            _mockRepository.Object,
            _mockCustomerRepository.Object,
            _mapper,
            _mockValidator.Object,
            _mockProductRepository.Object,
            _mockHttpClient.Object
        );
    }

    [Test]
    public async Task CreateAsync_ValidBillingRequest_ReturnsBillingResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var billingRequest = new BillingRequest
        {
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TotalAmount = 100,
            Currency = "USD",
            Lines = new List<BillingLineRequest>
            {
                new BillingLineRequest
                {
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = 100,
                    Subtotal = 100
                }
            }
        };

        _mockValidator.Setup(v => v.ValidateAsync(billingRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var customerEntity = new CustomerEntity { Id = billingRequest.CustomerId };
        _mockCustomerRepository.Setup(r => r.GetByIdAsync(billingRequest.CustomerId))
            .ReturnsAsync(customerEntity);

        var productEntity = new ProductEntity { Id = productId, Description = "Test Product" };
        _mockProductRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(productEntity);

        var createdBillingEntity = new BillingEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = billingRequest.CustomerId,
            Date = billingRequest.Date,
            DueDate = billingRequest.DueDate,
            TotalAmount = billingRequest.TotalAmount,
            Currency = billingRequest.Currency,
            Lines = new List<BillingLineEntity>
            {
                new BillingLineEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Description = productEntity.Description,
                    Quantity = billingRequest.Lines[0].Quantity,
                    UnitPrice = billingRequest.Lines[0].UnitPrice,
                    Subtotal = billingRequest.Lines[0].Subtotal
                }
            }
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<BillingEntity>()))
            .ReturnsAsync(createdBillingEntity);

        // Act
        var result = await _billingService.CreateAsync(billingRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdBillingEntity.Id);
        result.TotalAmount.Should().Be(billingRequest.TotalAmount);
        result.Currency.Should().Be(billingRequest.Currency);
        result.Lines.Should().HaveCount(1);
        result.Lines[0].ProductId.Should().Be(productId);
        result.Lines[0].Quantity.Should().Be(1); 
        result.Lines[0].UnitPrice.Should().Be(100); 
        result.Lines[0].Subtotal.Should().Be(100); 
        result.Lines[0].Description.Should().Be(productEntity.Description); 
    }

    [Test]
    public async Task CreateAsync_InvalidBillingRequest_ThrowsValidationException()
    {
        // Arrange
        var billingRequest = new BillingRequest(); 

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("CustomerId", "Customer ID is required"));
        _mockValidator.Setup(v => v.ValidateAsync(billingRequest, default))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> action = async () => await _billingService.CreateAsync(billingRequest);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task GetByIdAsync_ExistingBillingId_ReturnsBillingResponse()
    {
        // Arrange
        var billingId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productRequest = new ProductRequest
        {
            Description = "Test Product"
        };

        var existingBillingEntity = new BillingEntity
        {
            Id = billingId,
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TotalAmount = 100,
            Currency = "USD",
            Lines = new List<BillingLineEntity>
            {
                new BillingLineEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Description = productRequest.Description,
                    Quantity = 1,
                    UnitPrice = 100,
                    Subtotal = 100
                }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync(existingBillingEntity);

        // Act
        var result = await _billingService.GetByIdAsync(billingId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingBillingEntity.Id);
        result.TotalAmount.Should().Be(existingBillingEntity.TotalAmount);
        result.Currency.Should().Be(existingBillingEntity.Currency);
        result.Lines.Should().HaveCount(1);
        result.Lines[0].Description.Should().Be(productRequest.Description);
    }

    [Test]
    public async Task GetByIdAsync_NonExistingBillingId_ThrowsApplicationException()
    {
        // Arrange
        var billingId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync((BillingEntity)null);

        // Act
        Func<Task> action = async () => await _billingService.GetByIdAsync(billingId);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Billing with ID {billingId} not found.");
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllBillings()
    {
        // Arrange
        var billingEntities = new List<BillingEntity>
        {
            new BillingEntity { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = 100, Currency = "USD" },
            new BillingEntity { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = 200, Currency = "USD" }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(billingEntities);

        // Act
        var result = await _billingService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().TotalAmount.Should().Be(100);
        result.Last().TotalAmount.Should().Be(200);
    }

    [Test]
    public async Task GetAllAsync_NoBillings_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<BillingEntity>());

        // Act
        var result = await _billingService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task UpdateAsync_ValidBillingRequest_ReturnsUpdatedBillingResponse()
    {
        // Arrange
        var billingId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productRequest = new ProductRequest
        {
            Description = "Updated Product"
        };

        var billingRequest = new BillingRequest
        {
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TotalAmount = 150,
            Currency = "USD",
            Lines = new List<BillingLineRequest>
            {
                new BillingLineRequest
                {
                    ProductId = productId,
                    Quantity = 2,
                    UnitPrice = 75,
                    Subtotal = 150
                }
            }
        };

        _mockValidator.Setup(v => v.ValidateAsync(billingRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var existingBillingEntity = new BillingEntity
        {
            Id = billingId,
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TotalAmount = 100,
            Currency = "USD",
            Lines = new List<BillingLineEntity>
            {
                new BillingLineEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Description = "Old Product",
                    Quantity = 1,
                    UnitPrice = 100,
                    Subtotal = 100
                }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync(existingBillingEntity);

        var updatedBillingEntity = new BillingEntity
        {
            Id = billingId,
            CustomerId = billingRequest.CustomerId,
            Date = billingRequest.Date,
            DueDate = billingRequest.DueDate,
            TotalAmount = billingRequest.TotalAmount,
            Currency = billingRequest.Currency,
            Lines = new List<BillingLineEntity>
            {
                new BillingLineEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Description = productRequest.Description,
                    Quantity = billingRequest.Lines[0].Quantity,
                    UnitPrice = billingRequest.Lines[0].UnitPrice,
                    Subtotal = billingRequest.Lines[0].Subtotal
                }
            }
        };

        _mockRepository.Setup(r => r.UpdateAsync(existingBillingEntity))
            .ReturnsAsync(updatedBillingEntity);

        // Act
        var result = await _billingService.UpdateAsync(billingId, billingRequest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(billingId);
        result.TotalAmount.Should().Be(billingRequest.TotalAmount);
        result.Currency.Should().Be(billingRequest.Currency);
        result.Lines.Should().HaveCount(1);
        result.Lines[0].Description.Should().Be(productRequest.Description);
    }

    [Test]
    public async Task UpdateAsync_NonExistingBillingId_ThrowsApplicationException()
    {
        // Arrange
        var billingId = Guid.NewGuid();
        var billingRequest = new BillingRequest
        {
            CustomerId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TotalAmount = 150,
            Currency = "USD",
            Lines = new List<BillingLineRequest>
            {
                new BillingLineRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 75,
                    Subtotal = 150
                }
            }
        };

        _mockValidator.Setup(v => v.ValidateAsync(billingRequest, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync((BillingEntity)null);

        // Act
        Func<Task> action = async () => await _billingService.UpdateAsync(billingId, billingRequest);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Billing with ID {billingId} not found.");
    }

    [Test]
    public async Task DeleteByIdAsync_ExistingBillingId_CallsRepositoryDelete()
    {
        // Arrange
        var billingId = Guid.NewGuid();
        var existingBillingEntity = new BillingEntity
        {
            Id = billingId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100,
            Currency = "USD"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync(existingBillingEntity);

        // Act
        await _billingService.DeleteByIdAsync(billingId);

        // Assert
        _mockRepository.Verify(r => r.DeleteByIdAsync(billingId), Times.Once);
    }

    [Test]
    public async Task DeleteByIdAsync_NonExistingBillingId_ThrowsApplicationException()
    {
        // Arrange
        var billingId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(billingId))
            .ReturnsAsync((BillingEntity)null);

        // Act
        Func<Task> action = async () => await _billingService.DeleteByIdAsync(billingId);

        // Assert
        await action.Should().ThrowAsync<ApplicationException>()
            .WithMessage($"Billing with ID {billingId} not found.");
    }
}


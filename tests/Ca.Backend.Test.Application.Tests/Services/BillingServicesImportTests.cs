using AutoMapper;
using Ca.Backend.Test.Application.Mappings;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Responses.Api;
using Ca.Backend.Test.Application.Services;
using Ca.Backend.Test.Application.Services.Interfaces;
using Ca.Backend.Test.Domain.Entities;
using Ca.Backend.Test.Infra.Data.Repository.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ca.Backend.Test.Application.Tests.Services
{
    [TestFixture]
    public class BillingServicesImportTests
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

        /*[Test]
        public async Task ImportBillingFromExternalApiAsync_ValidApiResponse_ImportsBillingSuccessfully()
        {
            // Arrange
            var billingApiResponse = new BillingApiResponse
            {
                InvoiceNumber = "INV-001",
                Customer = new CustomerApiResponse
                {
                    Id = Guid.NewGuid().ToString(), // Simulating a valid GUID here
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Address = "123 Main St"
                },
                Date = "2023-06-25",
                DueDate = "2023-07-25",
                TotalAmount = 100,
                Currency = "USD",
                Lines = new List<LineApiResponse>
                {
                    new LineApiResponse
                    {
                        ProductId = Guid.NewGuid().ToString(), // Simulating a valid GUID here
                        Description = "Test Product",
                        Quantity = 1,
                        UnitPrice = 100,
                        Subtotal = 100
                    }
                }
            };

            var billingApiResponseJson = JsonSerializer.Serialize(new List<BillingApiResponse> { billingApiResponse });

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(billingApiResponseJson)
            };

            _mockHttpClient
                .Setup(client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponseMessage);

            var customerEntity = new CustomerEntity { Id = Guid.Parse(billingApiResponse.Customer.Id) };
            _mockCustomerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                                   .ReturnsAsync(customerEntity);

            var productEntity = new ProductEntity { Id = Guid.Parse(billingApiResponse.Lines[0].ProductId), Description = "Test Product" };
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                                  .ReturnsAsync(productEntity);

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<BillingEntity>()))
                           .ReturnsAsync((BillingEntity entity) => entity);

            // Act
            await _billingService.ImportBillingFromExternalApiAsync();

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<BillingEntity>()), Times.Once);
        }*/

        [Test]
        public async Task ImportBillingFromExternalApiAsync_InvalidCustomer_ThrowsApplicationException()
        {
            // Arrange
            var billingApiResponse = new BillingApiResponse
            {
                InvoiceNumber = "INV-001",
                Customer = new CustomerApiResponse
                {
                    Id = Guid.NewGuid(), // Use um ID de cliente inválido aqui
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Address = "123 Main St"
                },
                Date = "2023-06-25",
                DueDate = "2023-07-25",
                TotalAmount = 100,
                Currency = "USD",
                Lines = new List<LineApiResponse>
                {
                    new LineApiResponse
                    {
                        ProductId = "456",
                        Description = "Test Product",
                        Quantity = 1,
                        UnitPrice = 100,
                        Subtotal = 100
                    }
                }
            };

            var billingApiResponseJson = JsonSerializer.Serialize(new List<BillingApiResponse> { billingApiResponse });

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(billingApiResponseJson)
            };

            _mockHttpClient
                .Setup(client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponseMessage);

            _mockCustomerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                                .ReturnsAsync((CustomerEntity)null);

            // Act
            Func<Task> action = async () => await _billingService.ImportBillingFromExternalApiAsync();

            // Assert
            await action.Should().ThrowAsync<ApplicationException>()
                        .WithMessage($"Customer with ID {billingApiResponse.Customer.Id} not found.");
        }


            
        /*[Test]
        public async Task ImportBillingFromExternalApiAsync_InvalidProduct_ThrowsApplicationException()
        {
            // Arrange
            var billingApiResponse = new BillingApiResponse
            {
                InvoiceNumber = "INV-001",
                Customer = new CustomerApiResponse
                {
                    Id = "1da9861b-0a5b-44a8-b04c-14c42c081add",
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Address = "123 Main St"
                },
                Date = "2023-06-25",
                DueDate = "2023-07-25",
                TotalAmount = 100,
                Currency = "USD",
                Lines = new List<LineApiResponse>
                {
                    new LineApiResponse
                    {
                        ProductId = "789", // Use an invalid product ID here
                        Description = "Test Product",
                        Quantity = 1,
                        UnitPrice = 100,
                        Subtotal = 100
                    }
                }
            };

            var billingApiResponseJson = JsonSerializer.Serialize(new List<BillingApiResponse> { billingApiResponse });

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(billingApiResponseJson)
            };

            _mockHttpClient
                .Setup(client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponseMessage);

            var customerEntity = new CustomerEntity { Id = Guid.Parse(billingApiResponse.Customer.Id) };
            _mockCustomerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                                   .ReturnsAsync(customerEntity);

            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                                  .ReturnsAsync((ProductEntity)null);

            // Act
            Func<Task> action = async () => await _billingService.ImportBillingFromExternalApiAsync();

            // Assert
            await action.Should().ThrowAsync<ApplicationException>()
                        .WithMessage($"Product with ID {billingApiResponse.Lines[0].ProductId} not found.");
        }*/
    }
}

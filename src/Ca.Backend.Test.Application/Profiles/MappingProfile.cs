using AutoMapper;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Domain.Entities;

namespace Ca.Backend.Test.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CustomerRequest, CustomerEntity>();
        CreateMap<CustomerEntity, CustomerResponse>();

        CreateMap<ProductRequest, ProductEntity>();
        CreateMap<ProductEntity, ProductResponse>();

        CreateMap<BillingRequest, BillingEntity>();
        CreateMap<BillingEntity, BillingResponse>();

        CreateMap<BillingLineRequest, BillingLineEntity>();
        CreateMap<BillingLineEntity, BillingLineResponse>();
    }
}


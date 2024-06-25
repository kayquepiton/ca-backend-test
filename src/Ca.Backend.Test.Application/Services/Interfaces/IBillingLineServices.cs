using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;

namespace Ca.Backend.Test.Application.Services.Interfaces;
public interface IBillingLineServices
{
    Task<BillingLineResponse> CreateAsync(BillingLineRequest billingLineRequest);
    Task<BillingLineResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<BillingLineResponse>> GetAllAsync();
    Task<BillingLineResponse> UpdateAsync(Guid id, BillingLineRequest billingLineRequest);
    Task DeleteByIdAsync(Guid id);    
}

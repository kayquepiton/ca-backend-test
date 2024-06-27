namespace Ca.Backend.Test.Application.Models.Request;
public class BillingLineRequest
{
    public Guid ProductId { get; set; }
    public ProductRequest Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}


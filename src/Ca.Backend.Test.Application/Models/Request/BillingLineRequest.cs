namespace Ca.Backend.Test.Application.Models.Request;
public class BillingLineRequest
{
    public Guid BillingId { get; set; }
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}


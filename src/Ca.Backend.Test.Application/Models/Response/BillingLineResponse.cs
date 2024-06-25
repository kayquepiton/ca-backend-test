namespace Ca.Backend.Test.Application.Models.Response;
public class BillingLineResponse
{
    public Guid Id { get; set; }
    public Guid BillingId { get; set; }
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

namespace Ca.Backend.Test.Application.Models.Request;
public class BillingRequest
{
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
}


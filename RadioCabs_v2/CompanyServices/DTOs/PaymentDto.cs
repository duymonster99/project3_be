namespace CompanyServices.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int? Amount { get; set; }
        public string? ContentPayment { get; set; }
        public string? PaymentTerm { get; set; }
        public DateTime? PaymentAt { get; set; }
    }
}

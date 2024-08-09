namespace AuthenticationServices.DTOs
{
    public class BookingDto
    {
        public string? Name { get; set; }
        public int? Mobile { get; set; }
        public string? FromCity { get; set; }
        public string? FromWard { get; set; }
        public string? FromDistrict { get; set; }
        public string? FromAddress { get; set; }
        public string? ToCity { get; set; }
        public string? ToWard { get; set; }
        public string? ToDistrict { get; set; }
        public string? ToAddress { get; set; }
        public bool? IsReceive { get; set; }
        public bool? IsNew { get; set; }
        public int? DriverId { get; set; }
    }
}

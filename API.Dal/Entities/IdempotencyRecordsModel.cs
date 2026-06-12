namespace API.Dal.Model {
    public class IdempotencyRecordsModel {
        public int Id { get; set; }
        public string idempotencyKey { get; set; } = string.Empty;
        public string requesHash { get; set; } = null!;
        public PaymentResponseModel paymentResponseBody { get; set; } = null!;
        public DateTime? createdDate { get; set; }
    }
}



namespace API.Dal.Model {
    public class IdempotencyRecordsModel {
        public int Id { get; set; }
        public string idempotencyKey { get; set; } = string.Empty;
        public string requestHash { get; set; } = null!;
        public PaymentResponseModel paymentResponseBody { get; set; } = null!;
        public PaymentModel paymentModel { get; set; } = null!;
        public DateTime? createdDate { get; set; }
    }
}



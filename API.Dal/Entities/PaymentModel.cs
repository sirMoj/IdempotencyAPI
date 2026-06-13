using System.ComponentModel.DataAnnotations;


namespace API.Dal.Model {
    public class PaymentModel {
        [Key]
        public string? Id { get; set; }
        public string orderId { get; set; } = null!;
        public decimal? amount { get; set; }
        public string paymentRef { get; set; } = null!;
    }
}

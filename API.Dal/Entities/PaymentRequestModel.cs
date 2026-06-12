using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Dal.Model {
    public class PaymentRequestModel {
        public int Id { get; set; }
        public string orderId { get; set; } = string.Empty;
        public decimal amount { get; set; } = 0;
    }
}

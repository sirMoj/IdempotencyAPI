using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.BLL.Model {
    public class PaymentRequest {
        public string orderId { get; set; } = null!;
        public decimal amount { get; set; }
    }
}

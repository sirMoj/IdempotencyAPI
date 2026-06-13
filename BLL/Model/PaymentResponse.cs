using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.BLL.Model {
    public class PaymentResponse {
        public int statuscode { get; set; }
        public string? paymentReference { get; set; }
        public string? status { get; set; }
        public string? message { get; set; }
    }
}

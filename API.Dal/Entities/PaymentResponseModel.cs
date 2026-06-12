using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Dal.Model {
    public class PaymentResponseModel {
        public int Id { get; set; }
        public string paymentRef { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;

    }
}

using API.BLL.Model;
using API.Dal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.BLL.Service {
    public interface IPaymentService {
        Task<PaymentResponse> PaymentAPIService(string idempotencyKey,PaymentRequest request);
    }
}

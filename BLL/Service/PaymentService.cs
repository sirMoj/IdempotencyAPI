using API.BLL.Model;
using API.Dal.Model;
using API.Dal.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.BLL.Service {
    public class PaymentService : IPaymentService {

        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository) { 
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponse> PaymentAPIService(string idempotencyKey,PaymentRequest request) {
            PaymentResponseModel responseModel;
            PaymentResponse paymentResponse = new PaymentResponse();
            PaymentModel paymentModel = await _paymentRepository.GetPaymentRecord(idempotencyKey);
            if (paymentModel.orderId != request.orderId || paymentModel.amount != request.amount) {
                paymentResponse.statuscode = 409;
                paymentResponse.message = "Payload does not match original request.";
                return paymentResponse;
            }else if (paymentModel.orderId == request.orderId && paymentModel.amount == request.amount) { 
                responseModel = await _paymentRepository.GetIdempotentRecord(idempotencyKey);
                paymentResponse = new PaymentResponse() { 
                    paymentReference = responseModel.paymentRef,
                    status = responseModel.status,
                };
                return paymentResponse;
            }else if (paymentModel == null) {
                paymentModel = this._initializeRequestModel(idempotencyKey,request);
                responseModel = await _paymentRepository.CreatePaymentTransaction(this._hashPaymentRequest(request),paymentModel);
                paymentResponse = new PaymentResponse() {
                    paymentReference = responseModel.paymentRef,
                    status = responseModel.status,
                };
                return paymentResponse;
            }
            return new PaymentResponse();
        }

        private PaymentModel _initializeRequestModel(string IdempotencyKey,PaymentRequest request) {
            PaymentModel paymentModel = new PaymentModel() { 
                paymentRef = IdempotencyKey,
                orderId = request.orderId,
                amount = request.amount,
            };
            return paymentModel;
        }

        private string _hashPaymentRequest(PaymentRequest request) {
            string hash = JsonSerializer.Serialize(request);
            byte[] inputBytes = Encoding.UTF8.GetBytes(hash);
            byte[] hashByte = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashByte);
        }

    }
}

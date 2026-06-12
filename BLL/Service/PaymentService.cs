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
            PaymentResponseModel paymentResponseModel;//for repository
            PaymentResponse paymentResponse = new PaymentResponse();// for APIservice response
            PaymentRequest paymentRequest = new PaymentRequest();

            PaymentModel paymentModel = await _paymentRepository.GetPaymentRecord(idempotencyKey);

            if (paymentModel != null) {
                paymentRequest = new PaymentRequest() {
                    orderId = paymentModel.orderId,
                    amount = paymentModel.amount
                };
            }

            string paymentModelHash = this._hashPaymentRequest(paymentRequest);
            string requestHash = this._hashPaymentRequest(request);

            if (paymentModel == null) {
                paymentModel = this._initializeRequestModel(idempotencyKey, request);
                paymentResponseModel = await _paymentRepository.CreatePaymentTransaction(idempotencyKey,requestHash, paymentModel);
                paymentResponse = new PaymentResponse() {
                    paymentReference = paymentResponseModel.paymentRef,
                    status = paymentResponseModel.status,
                };
            }
            else if (!requestHash.Equals(paymentModelHash)) {
                paymentResponse.statuscode = 409;
                paymentResponse.message = "Payload does not match original request.";
            }else {
                paymentResponseModel = await _paymentRepository.GetIdempotentRecord(idempotencyKey);
                paymentResponse = new PaymentResponse() { 
                    paymentReference = paymentResponseModel.paymentRef,
                    status = paymentResponseModel.status,
                };
            }
            return paymentResponse;
        }

        private PaymentModel _initializeRequestModel(string IdempotencyKey, PaymentRequest request) {
            PaymentModel paymentModel = new PaymentModel() {
                paymentRef = "PAY-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(99).ToString(),
                orderId = request.orderId,
                amount = request.amount,
            };
            return paymentModel;
        }

        private string _hashPaymentRequest(PaymentRequest request) {

            string normalized =$"{request.orderId}|{request.amount:F2}";
            byte[] inputBytes = Encoding.UTF8.GetBytes(normalized);
            byte[] hashByte = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashByte);
        }

    }
}

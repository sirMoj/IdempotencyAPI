using API.BLL.Model;
using API.Dal.Model;
using API.Dal.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.BLL.Service {
    public class PaymentService : IPaymentService {

        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository) {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponse> PaymentAPIService(string idempotencyKey, PaymentRequest request) {
            string requestHash = this._hashPaymentRequest(request);
            try {
                var paymentModel = this._initializeRequestModel(idempotencyKey, request);
                var paymentResponseModel = await _paymentRepository.CreatePaymentTransaction(idempotencyKey, requestHash, paymentModel);
                return new PaymentResponse() {
                    paymentReference = paymentResponseModel.paymentRef,
                    status = paymentResponseModel.status,
                };
            } catch (DbUpdateException ex) when (_IsUniqueConstraintViolation(ex)) {
                var idempotenRecord = await _paymentRepository.GetIdempotentRecord(idempotencyKey);
                if (idempotenRecord.requestHash != requestHash) {
                    return new PaymentResponse() {
                        statuscode = 409,
                        message = "Payload does not match original request."
                    };
                }
                var paymentResponseModel = await _paymentRepository.GetPaymentResponse(idempotencyKey);
                return new PaymentResponse() {
                    paymentReference = paymentResponseModel?.paymentRef,
                    status = paymentResponseModel?.status,
                };
            } catch (Exception ex){
                return new PaymentResponse() {
                    statuscode = 503, 
                    message = "Payment system is temporarily unavailable. Please retry shortly."
                };
            }

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

            string normalized = $"{request.orderId}|{request.amount:F2}";
            byte[] inputBytes = Encoding.UTF8.GetBytes(normalized);
            byte[] hashByte = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashByte);
        }

        private bool _IsUniqueConstraintViolation(DbUpdateException ex) {
            return ex.InnerException switch {
                SqlException sqlEx => sqlEx.Number is 2601 or 2627,
                _ => false
            };
        }
    }
}

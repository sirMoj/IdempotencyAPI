using API.Dal.Model;

namespace API.Dal.Repository {
    public interface IPaymentRepository {
        Task<PaymentResponseModel> CreatePaymentTransaction(string idempotencyKey,string hashedRequest, PaymentModel payment);
        Task<PaymentModel> GetPaymentRecord(string idempotencyKey);
        Task<PaymentResponseModel> GetIdempotentRecord(string key);
        Task<string> SavePaymentResponse(IdempotencyRecordsModel idempotentModel);
    }
}
using API.Dal.Model;

namespace API.Dal.Repository {
    public interface IPaymentRepository {
        Task<PaymentResponseModel> CreatePaymentTransaction(string idempotencyKey,string hashedRequest, PaymentModel payment);
        Task<IdempotencyRecordsModel> GetIdempotentRecord(string key);
        Task<PaymentResponseModel?> GetPaymentResponse(string key);
        Task<string> SavePaymentResponse(IdempotencyRecordsModel idempotentModel);
    }
}
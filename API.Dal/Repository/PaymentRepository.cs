using API.Dal.Context;
using API.Dal.Model;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Dal.Repository {
    public class PaymentRepository : IPaymentRepository{
        private readonly PaymentDbContext _context;


        public PaymentRepository(PaymentDbContext context) {
            _context = context;
        }

        public async Task<PaymentResponseModel> CreatePaymentTransaction(string hashedRequest,PaymentModel payment) {
            IdempotencyRecordsModel idempotencyRecordsModel = new IdempotencyRecordsModel() { 
                idempotencyKey = payment.paymentRef!,
                requesHash = hashedRequest,
                paymentResponseBody = new PaymentResponseModel() { 
                    paymentRef = payment.paymentRef!,
                    status = "success"
                },
                createdDate = DateTime.Now
            };
            try {
                var paymentIsCreated =  await _context.Payment.AddAsync(payment);
                var IdempotentIsCreated = await _context.idempotencyRecords.AddAsync(idempotencyRecordsModel);
                int result = await _context.SaveChangesAsync();
                return result > 0 ? idempotencyRecordsModel.paymentResponseBody: new PaymentResponseModel();
            } catch (Exception ex) {
                return new PaymentResponseModel();
            }
        }

        public async Task<PaymentModel> GetPaymentRecord(string idempotencyKey) {
            try {
                PaymentModel record = await _context.Payment.FirstOrDefaultAsync(predicate: payment => payment.paymentRef == idempotencyKey);
                return record != null ? record : null!;
            } catch (Exception ex) {
                return null!;
            }
        }

        public async Task<PaymentResponseModel> GetIdempotentRecord(string key) {
            try {
                IdempotencyRecordsModel idempotentModel = await _context.idempotencyRecords.FirstOrDefaultAsync(i => i.idempotencyKey == key);
                return idempotentModel!=null? idempotentModel.paymentResponseBody : null!;
            } catch (Exception e) {
                return null!;
            }
        }

        public async Task<string> SavePaymentResponse(IdempotencyRecordsModel idempotentModel) {
            try {
                _context.idempotencyRecords.AddAsync(idempotentModel);
                int result = await _context.SaveChangesAsync();
                return result > 0 ? "Success" : "Failed";
            } catch (Exception ex) {

            }
            return null;
        }

    }
}
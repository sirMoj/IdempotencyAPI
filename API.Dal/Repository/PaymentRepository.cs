using API.Dal.Context;
using API.Dal.Model;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Dal.Repository {
    public class PaymentRepository : IPaymentRepository {
        private readonly PaymentDbContext _context;


        public PaymentRepository(PaymentDbContext context) {
            _context = context;
        }

        public async Task<PaymentResponseModel> CreatePaymentTransaction(string idempotencyKey, string hashedRequest, PaymentModel payment) {
            IdempotencyRecordsModel idempotencyRecordsModel = new IdempotencyRecordsModel() {
                idempotencyKey = idempotencyKey,
                requestHash = hashedRequest,
                paymentResponseBody = new PaymentResponseModel() {
                    paymentRef = payment.paymentRef,
                    status = "success"
                },
                paymentModel = payment,
                createdDate = DateTime.Now
            };
            using var transaction = _context.Database.BeginTransaction();
            var IdempotentIsCreated = await _context.idempotencyRecords.AddAsync(idempotencyRecordsModel);
            int result = await _context.SaveChangesAsync();
            transaction.Commit();
            return result > 0 ? idempotencyRecordsModel.paymentResponseBody : null!;

        }
        public async Task<IdempotencyRecordsModel> GetIdempotentRecord(string key) {
            var idempotentModel = await _context.idempotencyRecords.FirstOrDefaultAsync(i => i.idempotencyKey == key);
            return idempotentModel != null ? idempotentModel : null!;
        }
        public async Task<PaymentResponseModel> GetPaymentResponse(string key) {
            var record = await _context.idempotencyRecords.AsNoTracking().FirstOrDefaultAsync(i => i.idempotencyKey == key);
            if (record == null || record.paymentResponseBody == null) {
                return null;
            } else {
                return record.paymentResponseBody;
            }
        }

        public async Task<string> SavePaymentResponse(IdempotencyRecordsModel idempotentModel) {
                await _context.idempotencyRecords.AddAsync(idempotentModel);
                int result = await _context.SaveChangesAsync();
                return result > 0 ? "Success" : "Failed";

        }

    }
}
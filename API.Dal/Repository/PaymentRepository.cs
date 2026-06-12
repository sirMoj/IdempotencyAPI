using API.Dal.Context;
using API.Dal.Model;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

        public async Task<PaymentResponseModel> CreatePaymentTransaction(string idempotencyKey,string hashedRequest,PaymentModel payment) {
            IdempotencyRecordsModel idempotencyRecordsModel = new IdempotencyRecordsModel() { 
                idempotencyKey = idempotencyKey,
                requesHash = hashedRequest,
                paymentResponseBody = new PaymentResponseModel() { 
                    paymentRef = payment.paymentRef,
                    status = "success"
                },
                paymentModel = payment,
                createdDate = DateTime.Now
            };
            try {
                using var transaction = _context.Database.BeginTransaction();
                var IdempotentIsCreated = await _context.idempotencyRecords.AddAsync(idempotencyRecordsModel);
                int result = await _context.SaveChangesAsync();
                transaction.Commit();
                return result > 0 ? idempotencyRecordsModel.paymentResponseBody: null!;
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task<PaymentModel?> GetPaymentRecord(string idempotencyKey) {
            try {
                IdempotencyRecordsModel? idempotencyRecordsModel = await _context.idempotencyRecords.FirstOrDefaultAsync(d=>d.idempotencyKey==idempotencyKey);
                return idempotencyRecordsModel?.paymentModel;
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task<PaymentResponseModel> GetIdempotentRecord(string key) {
            try {
                IdempotencyRecordsModel idempotentModel = await _context.idempotencyRecords.FirstOrDefaultAsync(i => i.idempotencyKey == key);
                return idempotentModel!=null? idempotentModel.paymentResponseBody : null!;
            } catch (Exception e) {
                throw;
            }
        }

        public async Task<string> SavePaymentResponse(IdempotencyRecordsModel idempotentModel) {
            try {
                await _context.idempotencyRecords.AddAsync(idempotentModel);
                int result = await _context.SaveChangesAsync();
                return result > 0 ? "Success" : "Failed";
            } catch (Exception ex) {
                throw;
            }
        }

    }
}
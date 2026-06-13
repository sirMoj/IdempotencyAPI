using API.Dal.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Dal.Context {
    public class PaymentDbContext : DbContext{

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base (options){
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentModel>(entity => {
                entity.ToTable("Payment");
                entity.HasKey(e=>e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<IdempotencyRecordsModel>(entity => {
                entity.ToTable("idempotencyRecords");
                entity.HasKey(e=>e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e=>e.idempotencyKey).IsUnique();
            });
        }

        public DbSet<PaymentModel> Payment { get; set; }
        public DbSet<IdempotencyRecordsModel> idempotencyRecords { get; set; }

    }
}

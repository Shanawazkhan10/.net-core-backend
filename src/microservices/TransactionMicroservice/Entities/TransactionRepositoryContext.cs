using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransactionMicroservice.Model;

namespace TransactionMicroservice.Entities
{
    public class TransactionRepositoryContext : DbContext
    {
        public TransactionRepositoryContext()
        {
        }

        public TransactionRepositoryContext(DbContextOptions<TransactionRepositoryContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Dev
            optionsBuilder.UseMySql("server=95.216.200.167; port=3306; database=we3_workz; userid=devwebdukandeveloper;password=devwebdukanDeveloper@D321;");

        }


        public DbSet<Common_Command_Output> Common_Command_Output { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Common_Command_Output>()
              .HasKey(Common_Command_Output_Row => new { Common_Command_Output_Row.Result_Id });

        }
    }
}

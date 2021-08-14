using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HelperMicroservice.Model;

namespace HelperMicroservice.Entities
{
    public class HelperRepositoryContext : DbContext
    {
        public HelperRepositoryContext()
        {
        }

        public HelperRepositoryContext(DbContextOptions<HelperRepositoryContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost; port=3306; database=ekyc; userid=root;password=shanawaz@12#;");
        }


        public DbSet<Common_Command_Output> Common_Command_Output { get; set; }
        public DbSet<Notification_List> Notification_List { get; set; }
        public DbSet<Notification_Display_Data> Notification_Display_Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Common
            modelBuilder.Entity<Common_Command_Output>()
              .HasKey(Common_Command_Output_Row => new { Common_Command_Output_Row.Result_Id });

            modelBuilder.Entity<Notification_List>()
               .HasKey(Notification_List_Row => new { Notification_List_Row.Org_Id, Notification_List_Row.Notification_Id });

            modelBuilder.Entity<Notification_Display_Data>()
              .HasKey(Notification_Display_Data => new { Notification_Display_Data.ID });

        }
    }
}

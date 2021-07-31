using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MasterDataMicroservice.Model;

namespace MasterDataMicroservice.Entities
{
    public class MasterRepositoryContext : DbContext
    {
        public MasterRepositoryContext()
        {
        }

        public MasterRepositoryContext(DbContextOptions<MasterRepositoryContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=95.216.200.167; port=3306; database=we3_workz; userid=devwebdukandeveloper;password=devwebdukanDeveloper@D321;");

        }


        public DbSet<Common_Command_Output> Common_Command_Output { get; set; }

        // Location
        public DbSet<Country_List> Country_List { get; set; }
        public DbSet<State_List> State_List { get; set; }
        public DbSet<City_List> City_List { get; set; }
        public DbSet<Location_List> Location_List { get; set; }

        // Config
        public DbSet<Config_Title> Config_Title { get; set; }
        public DbSet<Config_Gender> Config_Gender { get; set; }
        public DbSet<Config_AccessCode> Config_AccessCode { get; set; }

        public DbSet<Config_GetAccessCode> Config_GetAccessCode { get; set; }
        public DbSet<Config_StaffType> Config_StaffType { get; set; }

        // Menu
        public DbSet<Menu_List> Menu_List { get; set; }

        //Notification Master
        public DbSet<Notification_List> Notification_List { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Common
            modelBuilder.Entity<Common_Command_Output>()
              .HasKey(Common_Command_Output_Row => new { Common_Command_Output_Row.Result_Id });

            // Location
            modelBuilder.Entity<Country_List>()
               .HasKey(Country_List_Row => new { Country_List_Row.Country_Id });

            modelBuilder.Entity<State_List>()
               .HasKey(State_List_Row => new { State_List_Row.Country_Id, State_List_Row.State_Id });

            modelBuilder.Entity<City_List>()
               .HasKey(City_List_Row => new { City_List_Row.Country_Id, City_List_Row.State_Id, City_List_Row.City_Id });

            modelBuilder.Entity<Location_List>()
               .HasKey(Location_List_Row => new { Location_List_Row.Country_Id, Location_List_Row.State_Id, Location_List_Row.City_Id, Location_List_Row.Location_Id });


            // Config
            modelBuilder.Entity<Config_Title>()
               .HasKey(Config_Title_Row => new { Config_Title_Row.Title_Id });

            modelBuilder.Entity<Config_Gender>()
               .HasKey(Config_Gender_Row => new { Config_Gender_Row.Gender_Id });

            modelBuilder.Entity<Config_AccessCode>()
               .HasKey(Config_AccessCode_Row => new { Config_AccessCode_Row.AccessCode_Id });

            modelBuilder.Entity<Config_GetAccessCode>()
               .HasKey(Config_GetAccessCode_Row => new { Config_GetAccessCode_Row.name });

            modelBuilder.Entity<Config_StaffType>()
               .HasKey(Config_StaffType_Row => new { Config_StaffType_Row.StaffType_Id });

            //Menu
            modelBuilder.Entity<Menu_List>()
               .HasKey(Menu_List_Row => new { Menu_List_Row.Menu_Id, Menu_List_Row.Menu_Name, Menu_List_Row.Is_Active, Menu_List_Row.Is_Deleted });

            //Notification
            modelBuilder.Entity<Notification_List>()
             .HasKey(Notification_List_Row => new { Notification_List_Row.Notification_Id });

        }
    }
}

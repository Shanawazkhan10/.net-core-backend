using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserDataMicroservice.Model;

namespace UserDataMicroservice.Entities
{
    public class User_RepositoryContext : DbContext
    {
        public User_RepositoryContext()
        {
        }

        public User_RepositoryContext(DbContextOptions<User_RepositoryContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=95.216.200.167; port=3306; database=we3_workz; userid=devwebdukandeveloper;password=devwebdukanDeveloper@D321;");
        }


        public DbSet<Common_Command_Output> Common_Command_Output { get; set; }
        public DbSet<User_Details> User_Details { get; set; }
        public DbSet<User_Menu> User_Menu { get; set; }
        public DbSet<Application_List> Application_List { get; set; }
        public DbSet<Role_List> Role_List { get; set; }
        public DbSet<Role_Menu_List> Role_Menu_List { get; set; }
        public DbSet<User_List> User_List { get; set; }
        public DbSet<User_Application_Role_List> User_Application_Role_List { get; set; }
        public DbSet<New_User_List> New_User_List { get; set; }
        public DbSet<User_Emailid> User_Emailid { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Common_Command_Output>()
              .HasKey(Common_Command_Output_Row => new { Common_Command_Output_Row.Result_Id });

            modelBuilder.Entity<User_Details>()
               .HasKey(User_Details_Row => new { User_Details_Row.User_Id });

            modelBuilder.Entity<User_Menu>()
               .HasKey(User_Menu_Row => new { User_Menu_Row.Menu_Id });

            modelBuilder.Entity<Application_List>()
               .HasKey(Application_List_Row => new { Application_List_Row.Application_Id });

            modelBuilder.Entity<Role_List>()
               .HasKey(Role_List_Row => new { Role_List_Row.Application_Id, Role_List_Row.Role_Id });

            modelBuilder.Entity<Role_Menu_List>()
               .HasKey(Role_Menu_List_Row => new { Role_Menu_List_Row.Menu_Id });

            modelBuilder.Entity<User_List>()
               .HasKey(User_List_Row => new { User_List_Row.Application_Id, User_List_Row.User_Id });

            modelBuilder.Entity<New_User_List>()
               .HasKey(New_User_List_Row => new { New_User_List_Row.Result_Id });

            modelBuilder.Entity<User_Application_Role_List>()
               .HasKey(User_Application_Role_List_Row => new { User_Application_Role_List_Row.Application_Id, User_Application_Role_List_Row.User_Id });

            modelBuilder.Entity<User_Emailid>()
                .HasKey(User_Emailid_Row => new { User_Emailid_Row.User_Email, User_Emailid_Row.User_Id });

        }
    }
}

using SoftN_Trainings.Models.BO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.DAL
{
    public class SoftN_TrainingsContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Requisite> Requisites { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Training> Trainings { get; set; }

        public SoftN_TrainingsContext()
            : base("Data Source=asus_johna\\sqlexpress;Initial Catalog=SoftN_TrainingDB;Integrated Security=True")
        {
            Database.SetInitializer<SoftN_TrainingsContext>(
                new DropCreateDatabaseIfModelChanges<SoftN_TrainingsContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                .HasMany(up => up.Trainers)
                .WithMany(trainer => trainer.Sessions)
                .Map(mc =>
                {
                    mc.ToTable("Session_Trainer");
                    mc.MapLeftKey("SessionID");
                    mc.MapRightKey("TrainerID");
                });

            modelBuilder.Entity<Session>()
                .HasMany(up => up.Requisites)
                .WithMany(requisite => requisite.Sessions)
                .Map(mc =>
                {
                    mc.ToTable("Session_Requisite");
                    mc.MapLeftKey("SessionID");
                    mc.MapRightKey("RequisiteID");
                });

            base.OnModelCreating(modelBuilder);
        }

    }
}
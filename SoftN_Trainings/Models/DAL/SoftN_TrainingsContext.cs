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
        public DbSet<User> User { get; set; }
        public DbSet<Attendee> Attendee { get; set; }
        public DbSet<Inscription> Inscription { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Requisite> Requisite { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Trainer> Trainer { get; set; }
        public DbSet<Training> Training { get; set; }

        public SoftN_TrainingsContext()
            : base("Data Source=asus_johna\\sqlexpress;Initial Catalog=SoftN_TrainingDB;Integrated Security=True")
        {
            Database.SetInitializer<SoftN_TrainingsContext>(
                new DropCreateDatabaseIfModelChanges<SoftN_TrainingsContext>());
        }
    }
}
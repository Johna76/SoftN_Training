using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Sessions")]
    public class Session
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int MaxAttendees { get; set; }
        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }

        //navigation properties
        public virtual Training Training { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Inscription> Inscriptions { get; set; }
        public virtual ICollection<Trainer> Trainers { get; set; }
        public virtual ICollection<Requisite> Requisites { get; set; }
    }
}
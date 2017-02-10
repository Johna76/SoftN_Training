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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}",ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        [ForeignKey("Training")]
        public int TrainingID { get; set; }
        [ForeignKey("Location")]
        public int LocationID { get; set; }

        //navigation properties
        public virtual Training Training { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Inscription> Inscriptions { get; set; }
        public virtual ICollection<Trainer> Trainers { get; set; }
        public virtual ICollection<Requisite> Requisites { get; set; }
    }
}
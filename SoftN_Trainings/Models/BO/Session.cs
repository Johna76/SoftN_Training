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
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}",ApplyFormatInEditMode = true)]
        [Display(Name = "Datum")]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start uur")]
        public TimeSpan StartTime { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Eind uur")]
        public TimeSpan EndTime { get; set; }
        [Display(Name = "Max deelnemers")]
        [Range(1, 1000)]
        public int MaxAttendees { get; set; }
        [ForeignKey("Training")]
        public int TrainingID { get; set; }
        [ForeignKey("Location")]
        public int LocationID { get; set; }
        [NotMapped]
        [Display(Name = "Plaatsen vrij")]
        public int PlacesLeft { get; set; }

        //navigation properties
        public virtual Training Training { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Inscription> Inscriptions { get; set; }
        public virtual ICollection<Trainer> Trainers { get; set; }
        public virtual ICollection<Requisite> Requisites { get; set; }

        public Session()
        {
            Trainers = new List<Trainer>();
            Requisites = new List<Requisite>();
        }
    }
}
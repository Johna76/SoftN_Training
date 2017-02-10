using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Attendees")]
    public class Attendee
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FirstName { get; set; }
        [ForeignKey("Inscription")]
        public int InscriptionID { get; set; }

        //navigation properties
        public virtual Inscription Inscription { get; set; }
    }
}
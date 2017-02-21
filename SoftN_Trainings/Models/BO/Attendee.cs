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
        [Display(Name ="Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [ForeignKey("Inscription")]
        public int InscriptionID { get; set; }
        public string Fullname { get { return LastName + " " + FirstName; } }

        //navigation properties
        public virtual Inscription Inscription { get; set; }
    }
}
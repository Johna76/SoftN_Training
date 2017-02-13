using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Trainers")]
    public class Trainer
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Fullname { get { return LastName + " " + FirstName; } }

        //naviation properties
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
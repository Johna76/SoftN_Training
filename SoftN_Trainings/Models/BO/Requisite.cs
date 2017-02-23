using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Requisites")]
    public class Requisite
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }
        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        //navigation properties
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
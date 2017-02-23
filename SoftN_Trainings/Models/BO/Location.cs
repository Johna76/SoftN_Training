using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Locations")]
    public class Location
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Straat")]
        public string Street { get; set; }
        [Required]
        [Display(Name = "Nummer")]
        public string Number { get; set; }
        [Required]
        [Display(Name = "Postcode")]
        public string ZipCode { get; set; }
        [Required]
        [Display(Name = "Stad")]
        public string City { get; set; }
        [Display(Name = "Opmerking")]
        public string Remarks { get; set; }

        //naviation properties
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
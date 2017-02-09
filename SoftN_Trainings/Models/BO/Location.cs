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
        public string Street { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public String City { get; set; }

        //naviation properties
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
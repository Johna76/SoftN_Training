using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftN_Trainings.Models.BO
{
    [Table("Inscriptions")]
    public class Inscription
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public int NumberAttendees { get; set; }
        [Required]
        public Boolean WaitingList { get; set; }
        [ForeignKey("Session")]
        public int SessionId { get; set; }

        //naviation properties
        public virtual Session Session { get; set; }
        public virtual ICollection<Attendee> Attendees { get; set; }
    }
}
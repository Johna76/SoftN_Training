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
        [Display(Name ="Last name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a number")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Company name")]
        public string CompanyName { get; set; }
        [Display(Name = "Number attendees")]
        [Range(1,1000)]
        public int NumberAttendees { get; set; }
        [Required]
        [Display(Name = "Waiting list")]
        public Boolean WaitingList { get; set; }
        [ForeignKey("Session")]
        public int SessionID { get; set; }

        //naviation properties
        public virtual Session Session { get; set; }
        public virtual ICollection<Attendee> Attendees { get; set; }

        public Inscription()
        {
            Attendees = new List<Attendee>();
        }

        
    }
}
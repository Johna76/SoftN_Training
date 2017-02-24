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
        [Display(Name ="Achternaam")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a number")]
        [Display(Name = "Telefoonnummer")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Bedrijfsnaam")]
        public string CompanyName { get; set; }
        [Display(Name = "Aantal deelnemers")]
        [Range(1,1000)]
        public int NumberAttendees { get; set; }
        [Required]
        [Display(Name = "Wachtlijst")]
        public Boolean WaitingList { get; set; }
        public Guid GuidCheck { get; set; }
        [ForeignKey("Session")]
        public int SessionID { get; set; }

        //naviation properties
        public virtual Session Session { get; set; }
        public virtual ICollection<Attendee> Attendees { get; set; }

        /*public Inscription()
        {
            Attendees = new List<Attendee>();
        }*/

        
    }
}
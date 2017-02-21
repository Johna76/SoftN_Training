using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftN_Trainings.Models.BO;

namespace SoftN_Trainings.ViewModels
{
    public class InscriptionViewModel
    {
        public Inscription Inscription { get; set; }

        public IList<Attendee> Attendees { get; set; }

        public Session Session { get; set; }

        public Attendee Attendee { get; set; }

        public List<string> Attend { get; set; }

        public InscriptionViewModel()
        {
            Attend = new List<string>();
        }
    }
}
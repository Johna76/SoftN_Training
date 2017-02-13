using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftN_Trainings.Models.BO;
using System.Web.Mvc;

namespace SoftN_Trainings.ViewModels
{
    public class SessionCreateViewModel
    {
        public Session Session { get; set; }

        public IEnumerable<SelectListItem> AllTrainers { get; set; }
        public IEnumerable<SelectListItem> AllLocations { get; set; }
        public IEnumerable<SelectListItem> AllTrainings { get; set; }

        public List<int> SelectedTrainers { get; set; }



    }
}
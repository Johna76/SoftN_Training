﻿using SoftN_Trainings.Models.BO;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SoftN_Trainings.ViewModels
{
    public class SessionViewModel
    {
        public Session Session { get; set; }
        public IEnumerable<SelectListItem> AllTrainers { get; set; }
        public IEnumerable<SelectListItem> AllLocations { get; set; }
        public IEnumerable<SelectListItem> AllTrainings { get; set; }
        public IEnumerable<SelectListItem> AllRequisites { get; set; }
        public List<int> SelectedTrainers { get; set; }
        public List<int> SelectedRequisites { get; set; }

        public SessionViewModel()
        {
            SelectedTrainers = new List<int>();
            SelectedRequisites = new List<int>();
        }



    }
}
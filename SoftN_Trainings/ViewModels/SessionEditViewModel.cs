using SoftN_Trainings.Models.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftN_Trainings.ViewModels
{
    public class SessionEditViewModel
    {
        public Session Session { get; set; }
        public IEnumerable<SelectListItem> AllTrainers { get; set; }
        public IEnumerable<SelectListItem> AllLocations { get; set; }
        public IEnumerable<SelectListItem> AllTrainings { get; set; }

        private List<int> _selectedTrainer;
        public List<int> SelectedTrainers
        {
            get
            {
                if (_selectedTrainer == null)
                {
                    _selectedTrainer = Session.Trainers.Select(m => m.ID).ToList();
                }
                return _selectedTrainer;
            }
            set
            {
                _selectedTrainer = value;
            }
        }




    }
}
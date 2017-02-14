using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftN_Trainings.Models.BO;
using SoftN_Trainings.Models.DAL;
using SoftN_Trainings.ViewModels;

namespace SoftN_Trainings.Controllers
{
    public class SessionsController : Controller
    {
        private SoftN_TrainingsContext db = new SoftN_TrainingsContext();

        // GET: Sessions
        public ActionResult Index()
        {
            var sessions = db.Sessions.Include(s => s.Location).Include(s => s.Training);
            return View(sessions.ToList());
        }

        // GET: Sessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // GET: Sessions/Create
        public ActionResult Create()
        {
            SessionViewModel sessionVM = new SessionViewModel();
            
            sessionVM.AllTrainers = GetAllTrainers();
            
            sessionVM.AllLocations = GetAllLocations();

            sessionVM.AllTrainings = GetAllTrainings();

            sessionVM.AllRequisites = GetAllRequisites();

            return View(sessionVM);
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SessionViewModel sessionVM)
        {
            if (ModelState.IsValid)
            {   
                if(sessionVM.SelectedTrainers != null)
                {
                    foreach (int trainer in sessionVM.SelectedTrainers)
                    {
                        sessionVM.Session.Trainers.Add(db.Trainers.Find(trainer));
                    }
                }
                
                if(sessionVM.SelectedRequisites != null)
                {
                    foreach (int requisite in sessionVM.SelectedRequisites)
                    {
                        sessionVM.Session.Requisites.Add(db.Requisites.Find(requisite));
                    }
                }
                db.Sessions.Add(sessionVM.Session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sessionVM);
        }

        // GET: Sessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SessionViewModel sessionVM = new SessionViewModel
            {
                Session = db.Sessions.Include(i => i.Trainers).Include(i => i.Requisites).First(i => i.ID == id),
            };

            if (sessionVM.Session == null)
            {
                return HttpNotFound();
            }

            if (sessionVM.SelectedTrainers == null)
            {
                sessionVM.SelectedTrainers = sessionVM.Session.Trainers.Select(m => m.ID).ToList();
            }

            if (sessionVM.SelectedRequisites == null)
            {
                sessionVM.SelectedRequisites = sessionVM.Session.Requisites.Select(m => m.ID).ToList();
            }

            sessionVM.AllTrainers = GetAllTrainers();
            
            sessionVM.AllLocations = GetAllLocations();
            
            sessionVM.AllTrainings = GetAllTrainings();

            sessionVM.AllRequisites = GetAllRequisites();

            return View(sessionVM);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SessionViewModel sessionVM)
        {
            if (ModelState.IsValid)
            {
                Session originalSession = db.Sessions.Find(sessionVM.Session.ID);

                LoadSessionWithVM(originalSession, sessionVM);

                foreach (var trainer in originalSession.Trainers.ToList())
                {
                    originalSession.Trainers.Remove(trainer);
                }

                if(sessionVM.SelectedTrainers != null)
                {
                    foreach (var trainer in sessionVM.SelectedTrainers)
                    {
                        originalSession.Trainers.Add(db.Trainers.Find(trainer));
                    }
                }

                foreach (var requisite in originalSession.Requisites.ToList())
                {
                    originalSession.Requisites.Remove(requisite);
                }

                if(sessionVM.SelectedRequisites != null)
                {
                    foreach (var requisite in sessionVM.SelectedRequisites)
                    {
                        originalSession.Requisites.Add(db.Requisites.Find(requisite));
                    }
                }

                db.Entry(originalSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sessionVM);
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = db.Sessions.Find(id);
            db.Sessions.Remove(session);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetAllTrainers()
        {
            var allTrainersList = db.Trainers.ToList();
            IEnumerable<SelectListItem> alltrainers = allTrainersList.Select(o => new SelectListItem
            {
                Text = o.Fullname,
                Value = o.ID.ToString()
            });
            return alltrainers;
        }

        private IEnumerable<SelectListItem> GetAllLocations()
        {
            var allLocationsList = db.Locations.ToList();
            IEnumerable <SelectListItem> allLocations = allLocationsList.Select(o => new SelectListItem
            {
                Text = o.City,
                Value = o.ID.ToString()
            });
            return allLocations;
        }

        private IEnumerable<SelectListItem> GetAllTrainings()
        {
            var allTrainingsList = db.Trainings.ToList();
            IEnumerable <SelectListItem> allTrainings = allTrainingsList.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            });
            return allTrainings;
        }

        private IEnumerable<SelectListItem> GetAllRequisites()
        {
            var allRequisistesList = db.Requisites.ToList();
            IEnumerable<SelectListItem> allRequisites = allRequisistesList.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            });
            return allRequisites;
        }

        private void LoadSessionWithVM(Session session, SessionViewModel sessionViewModel)
        {
            session.Date = sessionViewModel.Session.Date;
            session.StartTime = sessionViewModel.Session.StartTime;
            session.EndTime = sessionViewModel.Session.EndTime;
            session.MaxAttendees = sessionViewModel.Session.MaxAttendees;
            session.TrainingID = sessionViewModel.Session.TrainingID;
            session.LocationID = sessionViewModel.Session.LocationID;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

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
            var sessionCreateViewModel = new SessionCreateViewModel();

            var allTrainersList = db.Trainers.ToList();
            sessionCreateViewModel.AllTrainers = allTrainersList.Select(o => new SelectListItem
            {
                Text = o.Fullname,
                Value = o.ID.ToString()
            });

            var allLocationsList = db.Locations.ToList();
            sessionCreateViewModel.AllLocations = allLocationsList.Select(o => new SelectListItem
            {
                Text = o.City,
                Value = o.ID.ToString()
            });

            var allTrainingsList = db.Trainings.ToList();
            sessionCreateViewModel.AllTrainings = allTrainingsList.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            });

            return View(sessionCreateViewModel);
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SessionCreateViewModel sessionCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                Session session = new Session
                {
                    StartDate = sessionCreateViewModel.Session.StartDate,
                    EndDate = sessionCreateViewModel.Session.EndDate,
                    MaxAttendees = sessionCreateViewModel.Session.MaxAttendees,
                    TrainingID = sessionCreateViewModel.Session.TrainingID,
                    LocationID = sessionCreateViewModel.Session.LocationID,
                };

                foreach(int trainer in sessionCreateViewModel.SelectedTrainers)
                {
                    session.Trainers.Add(db.Trainers.Find(trainer));
                }

                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sessionCreateViewModel);
        }

        // GET: Sessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sessionEditViewModel = new SessionEditViewModel
            {
                Session = db.Sessions.Include(i => i.Trainers).First(i => i.ID == id),
            };

            if (sessionEditViewModel.Session == null)
            {
                return HttpNotFound();
            }

            var allTrainersList = db.Trainers.ToList();
            sessionEditViewModel.AllTrainers = allTrainersList.Select(o => new SelectListItem
            {
                Text = o.Fullname,
                Value = o.ID.ToString()
            });

            var allLocationsList = db.Locations.ToList();
            sessionEditViewModel.AllLocations = allLocationsList.Select(o => new SelectListItem
            {
                Text = o.City,
                Value = o.ID.ToString()
            });

            var allTrainingsList = db.Trainings.ToList();
            sessionEditViewModel.AllTrainings = allTrainingsList.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            });

            return View(sessionEditViewModel);
           
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SessionEditViewModel sessionEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var originalSession = db.Sessions.Find(sessionEditViewModel.Session.ID);

                foreach (var trainer in originalSession.Trainers.ToList())
                {
                    originalSession.Trainers.Remove(trainer);
                }

                foreach (var trainer in sessionEditViewModel.SelectedTrainers)
                {
                    originalSession.Trainers.Add(db.Trainers.Find(trainer));
                }

                db.Entry(originalSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sessionEditViewModel);
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

        private void PopulateLocationDropDownList(object selectedLocation = null)
        {
            var locationsQuery = from d in db.Locations
                                   orderby d.City
                                   select d;
            ViewBag.DepartmentID = new SelectList(locationsQuery, "ID", "City", selectedLocation);
        }

        private void PopulateRequisiteDropDownList(object selectedRequisite = null)
        {
            var requisitesQuery = from d in db.Requisites
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(requisitesQuery, "ID", "Name", selectedRequisite);
        }

        private void PopulateTrainerDropDownList(object selectedTrainer = null)
        {
            var trainersQuery = from d in db.Trainers
                                   orderby d.LastName
                                   select d;
            ViewBag.DepartmentID = new SelectList(trainersQuery, "ID", "Name", selectedTrainer);
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

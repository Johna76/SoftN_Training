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
using PagedList;

namespace SoftN_Trainings.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private SoftN_TrainingsContext db = new SoftN_TrainingsContext();

        [AllowAnonymous]
        // GET: Sessions
        public ActionResult Index(string typeList, string currentFilter, string searchString, int? page, string sortOrder)
        {
            var sessions = db.Sessions.Include(s => s.Location).Include(s => s.Training).Include(s => s.Inscriptions);
            //List<Session> allSessions;   
            var allSessions = sessions;
            ViewBag.TypeList = typeList;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                var sessionsFiltered = sessions.Where(s => s.Training.Name.Contains(searchString));
                //allSessions = sessionsFiltered.ToList();
                allSessions = sessionsFiltered;
            }
            else
            {
                //allSessions = sessions.ToList();
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    allSessions = allSessions.OrderByDescending(s => s.Training.Name);
                    break;
                default: // Lastname ascending
                    allSessions = allSessions.OrderBy(s => s.Training.Name);
                    break;
            }

            //calculate places left for each session
            foreach (Session session in allSessions)
            {
                int totalInscriptionPlaces = 0;

                foreach(Inscription item in session.Inscriptions)
                {
                    if(item.WaitingList == false)
                    {
                        totalInscriptionPlaces += item.NumberAttendees;
                    }
                }
                session.PlacesLeft = session.MaxAttendees - totalInscriptionPlaces;
            }

            List<Session> filteredSessionsByDate = new List<Session>();
            foreach (Session item in allSessions)
            {
                if (item.Date >= DateTime.Today)
                {
                    filteredSessionsByDate.Add(item);
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (User.Identity.IsAuthenticated)
            {                
                if (typeList == null || typeList == "UpcomingSessions")
                {
                    return View(filteredSessionsByDate.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    return View(allSessions.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return View(filteredSessionsByDate.ToPagedList(pageNumber, pageSize));
            }
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
            SessionViewModel sessionVM = new SessionViewModel()
            {
                Session = new Session(),
            };
            
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
                        
            sessionVM.SelectedTrainers = sessionVM.Session.Trainers.Select(m => m.ID).ToList();            
            sessionVM.SelectedRequisites = sessionVM.Session.Requisites.Select(m => m.ID).ToList();

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
            var allTrainersList = db.Trainers.OrderBy(m => m.LastName).ToList();
            IEnumerable<SelectListItem> alltrainers = allTrainersList.Select(o => new SelectListItem
            {
                Text = o.Fullname,
                Value = o.ID.ToString()
            });
            return alltrainers;
        }

        private IEnumerable<SelectListItem> GetAllLocations()
        {
            var allLocationsList = db.Locations.OrderBy(m => m.City).ToList();
            IEnumerable <SelectListItem> allLocations = allLocationsList.Select(o => new SelectListItem
            {
                Text = o.City,
                Value = o.ID.ToString()
            });
            return allLocations;
        }

        private IEnumerable<SelectListItem> GetAllTrainings()
        {
            var allTrainingsList = db.Trainings.OrderBy(m => m.Name).ToList();
            IEnumerable <SelectListItem> allTrainings = allTrainingsList.Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            });
            return allTrainings;
        }

        private IEnumerable<SelectListItem> GetAllRequisites()
        {
            var allRequisistesList = db.Requisites.OrderBy(m => m.Name).ToList();
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

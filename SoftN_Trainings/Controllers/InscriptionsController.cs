using SoftN_Trainings.Models.BO;
using SoftN_Trainings.Models.DAL;
using SoftN_Trainings.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SoftN_Trainings.Controllers
{
    [Authorize]
    public class InscriptionsController : Controller
    {
        private SoftN_TrainingsContext db = new SoftN_TrainingsContext();

        // GET: Inscriptions
        public ActionResult Index(string typeList)
        {
            var inscriptions = db.Inscriptions.Include(i => i.Session);
            List<Inscription> allInscriptions = inscriptions.ToList();
            List<Inscription> filteredInscriptionsByDate = new List<Inscription>();

            foreach (Inscription item in allInscriptions)
            {
                if (item.Session.Date >= DateTime.Today)
                {
                    filteredInscriptionsByDate.Add(item);
                }
            }

            if(typeList == null || typeList == "UpcomingInscrip")
            {
                return View(filteredInscriptionsByDate);
            }
            else
            {
                return View(allInscriptions);
            }            
        }

        // GET: Inscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Inscription inscription = db.Inscriptions.Find(id);

            if (inscription == null)
            {
                return HttpNotFound();
            }
            return View(inscription);
        }

        // GET: Inscriptions/Create
        [AllowAnonymous]
        public ActionResult Create(int? sessionId)
        {
            if (sessionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InscriptionViewModel inscriptionVM = new InscriptionViewModel
            {
                Inscription = new Inscription
                {
                    SessionID = sessionId ?? default(int),
                },
                Session = db.Sessions.Find(sessionId),
            };

            if (!User.Identity.IsAuthenticated && inscriptionVM.Session.Date < DateTime.Today)
            {
                return View("CreateError");
            }

            return View(inscriptionVM);
        }

        // POST: Inscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InscriptionViewModel inscriptionVM)
        {
            if (ModelState.IsValid)
            {
                Boolean createInscription = false;

                if (inscriptionVM.Inscription.WaitingList == true)
                {
                    createInscription = true;
                }
                else
                {
                    Session _session = CalculatePlacesLeft(inscriptionVM.Inscription);

                    if ((_session.PlacesLeft - inscriptionVM.Inscription.NumberAttendees) >= 0)
                    {
                        createInscription = true;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Er zijn nog maar " + _session.PlacesLeft + " plaatsen beschikbaar. Gelieve u op de wachtlijst te zetten of een andere sessie te kiezen");
                        foreach (Attendee item in inscriptionVM.Attendees)
                        {
                            inscriptionVM.Attend.Add(item.LastName.ToString());
                            inscriptionVM.Attend.Add(item.FirstName.ToString());
                        }
                    }
                }
                if(createInscription == true)
                {
                    inscriptionVM.Inscription.GuidCheck = Guid.NewGuid();
                    db.Inscriptions.Add(inscriptionVM.Inscription);
                    int id = inscriptionVM.Inscription.ID;

                    foreach (Attendee a in inscriptionVM.Attendees)
                    {
                        a.InscriptionID = id;
                        db.Attendees.Add(a);
                    };

                    db.SaveChanges();
                    return RedirectToAction("Index","Home");
                }
            }
            return View(inscriptionVM);
        }

        // GET: Inscriptions/Edit/5
        [AllowAnonymous]
        public ActionResult Edit(int? id, Guid? validation)
        {
            if (id == null || (validation == null && !User.Identity.IsAuthenticated))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InscriptionViewModel inscriptionVM = new InscriptionViewModel
            {
                Inscription = db.Inscriptions.Find(id),
            };

            if (inscriptionVM.Inscription == null)
            {
                return HttpNotFound();
            }

            if((validation.Equals(inscriptionVM.Inscription.GuidCheck) && inscriptionVM.Inscription.Session.Date >= DateTime.Today) || User.Identity.IsAuthenticated)
            {
                foreach (Attendee item in inscriptionVM.Inscription.Attendees)
                {
                    inscriptionVM.Attend.Add(item.LastName.ToString());
                    inscriptionVM.Attend.Add(item.FirstName.ToString());
                }

                return View(inscriptionVM);
            }
            else
            {
                return View("EditError");
            }
        }

        // POST: Inscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Edit(InscriptionViewModel inscriptionVM)
        {
            if (ModelState.IsValid)
            {
                Boolean updateInscription = false;

                Inscription originalInscription = db.Inscriptions.Find(inscriptionVM.Inscription.ID);

                if(originalInscription == null)
                {
                    return HttpNotFound();
                }                

                if (inscriptionVM.Inscription.WaitingList == true)
                {
                    updateInscription = true;
                }
                else
                {
                    Session _session = CalculatePlacesLeft(inscriptionVM.Inscription);

                    if (originalInscription.WaitingList == false)
                    {
                        _session.PlacesLeft += originalInscription.NumberAttendees;
                    }

                    if ((_session.PlacesLeft - inscriptionVM.Inscription.NumberAttendees) >= 0)
                    {
                        updateInscription = true;

                    }
                    else
                    {
                        ModelState.AddModelError("", "Er is onvoldoende plaats in deze sessie. Gelieve u op de wachtlijst te zetten of een andere sessie te kiezen");
                    }
                }

                if (updateInscription == true)
                {
                    foreach (Attendee a in originalInscription.Attendees.ToList())
                    {
                        db.Attendees.Remove(a);
                    }

                    int id = originalInscription.ID;

                    foreach (Attendee a in inscriptionVM.Attendees)
                    {
                        a.InscriptionID = id;
                        db.Attendees.Add(a);
                    };

                    LoadInscriptionWithVM(originalInscription, inscriptionVM.Inscription);
                    db.Entry(originalInscription).State = EntityState.Modified;
                    db.SaveChanges();

                    if (User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Index", "Inscriptions");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(inscriptionVM);
        }

        // GET: Inscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscription inscription = db.Inscriptions.Find(id);
            if (inscription == null)
            {
                return HttpNotFound();
            }
            return View(inscription);
        }

        // POST: Inscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscription inscription = db.Inscriptions.Find(id);
            db.Inscriptions.Remove(inscription);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        private Session CalculatePlacesLeft(Inscription inscription)
        {
            Session _session = db.Sessions
                                         .Include(i => i.Inscriptions)
                                         .First(i => i.ID == inscription.SessionID);

            int totalInscriptionPlaces = 0;

            foreach (Inscription item in _session.Inscriptions)
            {
                if (item.WaitingList == false)
                {
                    totalInscriptionPlaces += item.NumberAttendees;
                }
            }
            _session.PlacesLeft = _session.MaxAttendees - totalInscriptionPlaces;
            return _session;
        }

        private void LoadInscriptionWithVM(Inscription original, Inscription vmInscription)
        {
            original.LastName = vmInscription.LastName;
            original.FirstName = vmInscription.FirstName;
            original.Email = vmInscription.Email;
            original.PhoneNumber = vmInscription.PhoneNumber;
            original.CompanyName = vmInscription.CompanyName;
            original.NumberAttendees = vmInscription.NumberAttendees;
            original.WaitingList = vmInscription.WaitingList;
            original.SessionID = vmInscription.SessionID;
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

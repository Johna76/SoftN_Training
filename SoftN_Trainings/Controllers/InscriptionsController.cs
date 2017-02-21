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
using Microsoft.Ajax.Utilities;

namespace SoftN_Trainings.Controllers
{
    [Authorize]
    public class InscriptionsController : Controller
    {
        private SoftN_TrainingsContext db = new SoftN_TrainingsContext();

        // GET: Inscriptions
        public ActionResult Index()
        {
            var inscriptions = db.Inscriptions.Include(i => i.Session);
            return View(inscriptions.ToList());
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
                if (inscriptionVM.Inscription.WaitingList == true)
                {
                    db.Inscriptions.Add(inscriptionVM.Inscription);
                    db.SaveChanges();

                    int id = inscriptionVM.Inscription.ID;
                    
                    foreach (Attendee a in inscriptionVM.Attendees)
                    {
                        a.InscriptionID = id;
                        db.Attendees.Add(a);
                    };

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    Session _session = CalculatePlacesLeft(inscriptionVM.Inscription);

                    if ((_session.PlacesLeft - inscriptionVM.Inscription.NumberAttendees) >= 0)
                    {
                        db.Inscriptions.Add(inscriptionVM.Inscription);
                        db.SaveChanges();

                        int id = inscriptionVM.Inscription.ID;

                        foreach (Attendee a in inscriptionVM.Attendees)
                        {
                            a.InscriptionID = id;
                            db.Attendees.Add(a);
                        };

                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Er is onvoldoende plaats in deze sessie. Gelieve u op de wachtlijst te zetten of een andere sessie te kiezen");
                    }
                }
                
            }
            return View(inscriptionVM);
        }

        // GET: Inscriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
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

            foreach(Attendee item in inscriptionVM.Inscription.Attendees)
            {                
                inscriptionVM.Attend.Add(item.LastName.ToString());
                inscriptionVM.Attend.Add(item.FirstName.ToString());
                inscriptionVM.Attend.Add(item.ID.ToString());
            }
            
            return View(inscriptionVM);
        }

        // POST: Inscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InscriptionViewModel inscriptionVM)
        {
            if (ModelState.IsValid)
            {
                Inscription originalInscription = db.Inscriptions.Find(inscriptionVM.Inscription.ID);

                if(inscriptionVM.Inscription.WaitingList == true)
                {
                    LoadInscriptionWithVM(originalInscription, inscriptionVM.Inscription);
                    db.Entry(originalInscription).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    Session _session = CalculatePlacesLeft(inscriptionVM.Inscription);

                    if(originalInscription.WaitingList == false)
                    {
                        _session.PlacesLeft += originalInscription.NumberAttendees;
                    }
                    
                    if ((_session.PlacesLeft - inscriptionVM.Inscription.NumberAttendees) >= 0)
                    {
                        LoadInscriptionWithVM(originalInscription, inscriptionVM.Inscription);
                        db.Entry(originalInscription).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Er is onvoldoende plaats in deze sessie. Gelieve u op de wachtlijst te zetten of een andere sessie te kiezen");
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttendee(InscriptionViewModel inscriptionVM)
        {
            return View(inscriptionVM);
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

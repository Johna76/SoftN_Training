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

namespace SoftN_Trainings.Controllers
{
    public class RequisitesController : Controller
    {
        private SoftN_TrainingsContext db = new SoftN_TrainingsContext();

        // GET: Requisites
        public ActionResult Index()
        {
            return View(db.Requisites.ToList());
        }

        // GET: Requisites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requisite requisite = db.Requisites.Find(id);
            if (requisite == null)
            {
                return HttpNotFound();
            }
            return View(requisite);
        }

        // GET: Requisites/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Requisites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] Requisite requisite)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Requisites.Add(requisite);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(requisite);
        }

        // GET: Requisites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requisite requisite = db.Requisites.Find(id);
            if (requisite == null)
            {
                return HttpNotFound();
            }
            return View(requisite);
        }

        // POST: Requisites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requisite requisiteToUpdate = db.Requisites.Find(id);
            if (TryUpdateModel(requisiteToUpdate, "",
                new string[] { "Name", "Description" }))
            {
                try
                {
                    db.SaveChanges();
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return RedirectToAction("Index");
            //hou geen rekening met concurrencycheck !!! nog doen
        }

        // GET: Requisites/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Requisite requisite = db.Requisites.Find(id);
            if (requisite == null)
            {
                return HttpNotFound();
            }
            return View(requisite);
        }

        // POST: Requisites/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Requisite requisite = db.Requisites.Find(id);
                db.Requisites.Remove(requisite);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
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

using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;
using SoftN_Trainings.Models.BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftN_Trainings.Controllers
{
    public class EmailsController : Controller
    {
        // GET: Emails
        public ActionResult Index(Inscription inscription)
        {
            


            return View();
        }

        // GET: Emails/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Emails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Emails/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Emails/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Emails/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Emails/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Emails/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        private System.Net.Mail.Attachment CreateIcal (Inscription inscription)
        {
            var calendar = new Ical.Net.Calendar();
            
            calendar.Events.Add(new Event
            {
                Class = "PUBLIC",
                Summary = inscription.Session.Training.Name,
                Created = new CalDateTime(DateTime.Now),
                Description = inscription.Session.Training.Description,
                Start = new CalDateTime(Convert.ToDateTime(inscription.Session.Date + inscription.Session.StartTime)),
                End = new CalDateTime(Convert.ToDateTime(inscription.Session.Date + inscription.Session.EndTime)),
                Sequence = 0,
                Uid = Guid.NewGuid().ToString(),
                Location = inscription.Session.Location.Street + " " + inscription.Session.Location.Number + " " +
                           inscription.Session.Location.ZipCode + " " + inscription.Session.Location.City,

            });
            
            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(calendar);
            var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
            MemoryStream ms = new MemoryStream(bytesCalendar);
            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "event.ics", "text/calendar");

            return attachment;
        }

    }
}

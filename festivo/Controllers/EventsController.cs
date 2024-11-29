using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using festivo.Models;

namespace festivo.Controllers
{
    public class EventsController : Controller
    {
        private festivoEntities1 db = new festivoEntities1();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.EventType).Include(e => e.User);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.EventTypes = new SelectList(db.EventTypes, "EventTypeID", "TypeName");
            ViewBag.Organizers = new SelectList(db.Users, "UserID", "Name");
            return View();
        }


        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,Title,Description,EventTypeID,Venue,City,Date,StartTime,Price,OrganizerID,MaxCapacity")] Event @event, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Save the uploaded image
                    string filePath = SaveUploadedFile(ImageFile);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        @event.Image = filePath;
                    }
                }

                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "TypeName", @event.EventTypeID);
            ViewBag.OrganizerID = new SelectList(db.Users, "UserID", "Name", @event.OrganizerID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "TypeName", @event.EventTypeID);
            ViewBag.OrganizerID = new SelectList(db.Users, "UserID", "Name", @event.OrganizerID);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,Title,Description,EventTypeID,Venue,City,Date,StartTime,Price,Image,OrganizerID,MaxCapacity")] Event @event, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Check if a new image is uploaded
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(@event.Image))
                    {
                        string oldFilePath = Server.MapPath(@event.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Save the new uploaded image
                    string filePath = SaveUploadedFile(ImageFile);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        @event.Image = filePath;
                    }
                }

                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "TypeName", @event.EventTypeID);
            ViewBag.OrganizerID = new SelectList(db.Users, "UserID", "Name", @event.OrganizerID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);

            // Delete the associated image if it exists
            if (!string.IsNullOrEmpty(@event.Image))
            {
                string filePath = Server.MapPath(@event.Image);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            db.Events.Remove(@event);
            db.SaveChanges();
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

        // Helper Method to Save Uploaded Files
        private string SaveUploadedFile(HttpPostedFileBase file)
        {
            try
            {
                string folderPath = Server.MapPath("~/image/");

                // Ensure the folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique file name to avoid conflicts
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                // Save the file
                file.SaveAs(filePath);

                // Return relative path to store in the database
                return "/image/" + fileName;
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine("File Upload Error: " + ex.Message);
                return null;
            }
        }
    }
}

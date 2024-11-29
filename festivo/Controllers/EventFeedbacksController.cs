using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using festivo.Models;

namespace festivo.Controllers
{
    public class EventFeedbacksController : Controller
    {
        private festivoEntities1 db = new festivoEntities1();

        // GET: EventFeedbacks
        public ActionResult Index()
        {
            var eventFeedbacks = db.EventFeedbacks.Include(e => e.Event).Include(e => e.User);
            return View(eventFeedbacks.ToList());
        }

        // GET: EventFeedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventFeedback eventFeedback = db.EventFeedbacks.Find(id);
            if (eventFeedback == null)
            {
                return HttpNotFound();
            }
            return View(eventFeedback);
        }

        // GET: EventFeedbacks/Create
        public ActionResult Create()
        {
            ViewBag.EventID = new SelectList(db.Events, "EventID", "Title");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name");
            return View();
        }

        // POST: EventFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedbackID,EventID,UserID,Rating,Comment")] EventFeedback eventFeedback)
        {
            if (ModelState.IsValid)
            {
                db.EventFeedbacks.Add(eventFeedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "EventID", "Title", eventFeedback.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", eventFeedback.UserID);
            return View(eventFeedback);
        }

        // GET: EventFeedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventFeedback eventFeedback = db.EventFeedbacks.Find(id);
            if (eventFeedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "Title", eventFeedback.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", eventFeedback.UserID);
            return View(eventFeedback);
        }

        // POST: EventFeedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeedbackID,EventID,UserID,Rating,Comment")] EventFeedback eventFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventFeedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "EventID", "Title", eventFeedback.EventID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", eventFeedback.UserID);
            return View(eventFeedback);
        }

        // GET: EventFeedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventFeedback eventFeedback = db.EventFeedbacks.Find(id);
            if (eventFeedback == null)
            {
                return HttpNotFound();
            }
            return View(eventFeedback);
        }

        // POST: EventFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventFeedback eventFeedback = db.EventFeedbacks.Find(id);
            db.EventFeedbacks.Remove(eventFeedback);
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
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL;
using Logic;
using makeyourtournament.Models;

namespace makeyourtournament.Controllers
{
    public class TeamController : Controller
    {
        private DatabaseContainer db = new DatabaseContainer();
        private LogicClass logic;

        public TeamController()
        {
            logic = new LogicClass(db);
        }
        // GET: Team
        public ActionResult Index()
        {
            String username = User.Identity.Name;
            var user = db.UserSet.Where(i => i.name == username).First();
            var teamSet = db.TeamSet.Include(t => t.User).Where(t => t.UserId == user.Id);
            TeamEdit teamModel = new TeamEdit();
            teamModel.TeamList = teamSet;
            return View(teamModel);
        }

        // GET: Team/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.TeamSet.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Team/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.UserSet, "Id", "name");
            return View();
        }

        // POST: Team/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name")] Team team)
        {
            //TODO will nicht valedieren
            if (ModelState.IsValid && team.name != null)
            {
                String username = User.Identity.Name;
                var user = db.UserSet.Where(i => i.name == username).First();
                logic.newTeam(team,user);
                
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.TeamSet.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserSet, "Id", "name", team.UserId);
            return View(team);
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

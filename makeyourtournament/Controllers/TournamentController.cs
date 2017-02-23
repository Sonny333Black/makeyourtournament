using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DAL;

using makeyourtournament.Models;
using Logic;

namespace makeyourtournament.Controllers
{
    public class TournamentController : Controller
    {
        private DatabaseContainer db = new DatabaseContainer();
        private LogicClass logic;

        public TournamentController()
        {
            logic = new LogicClass(db);
        }
        // GET: Tournament
        public ActionResult Index()
        {
            CreateTournamentViewModel createModel = new CreateTournamentViewModel();
            IEnumerable<DAL.Modus> modus = db.ModusSet.ToList();

            createModel.Modus = modus;        

            return View(createModel);
        }       

        // POST: Tournament/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                String username = User.Identity.Name;
                var user = db.UserSet.Where(i => i.name == username).First();

                tournament.Modus = db.ModusSet.Find(Int32.Parse(Request.Form["modus"]));
                if (Int32.Parse(Request.Form["modus"]) == 1)
                {
                    tournament.countGroups = Int32.Parse(Request.Form["anzahlGruppen"]);
                }else
                {
                    tournament.countGroups = 1;
                }
                
                tournament.countTeamsForUser = Int32.Parse(Request.Form["teamsProSpieler"]);
                tournament.countUser = Int32.Parse(Request.Form["anzahlSpieler"]);
                tournament.status = 0;
                tournament.winner = 0;

                if (tournament.name == null)
                {                    
                    return Redirect("Index");
                }
                logic.createTournament(user, tournament);

                PickTeamsViewModel pickTeams = new PickTeamsViewModel();
                pickTeams.Tournament = tournament;
                pickTeams.User = user;                
                pickTeams.Friends = logic.giveFriends(user);
                return View("PickTeams", pickTeams);

            }

            return RedirectToAction("Tournament", new { tour_id = tournament.Id });
        }

        
        public ActionResult Edit(int? tour_id)
        {
            Tournament tour;
            
            if (tour_id == null)
            {
                tour = db.TournamentSet.Find(Int32.Parse(Request.Form["tour_id"]));
            }else
            {
                tour = db.TournamentSet.Find(tour_id);
            }

            String username = User.Identity.Name;
            var user = db.UserSet.Where(i => i.name == username).First();

            //für Gruppe
            if (tour.Modus.Id != 3 && tour.owner == user.Id)
            {
                //for tournaments with groups
                if (tour.GroupCard.Count() <= 0)
                {
                    if (tour.status == 0)
                    {
                        PickTeamsViewModel pickTeams = new PickTeamsViewModel();
                        pickTeams.Tournament = tour;
                        pickTeams.User = user;
                        pickTeams.Friends = logic.giveFriends(user);
                        return View("PickTeams", pickTeams);
                    }
                }else if(tour.status == 0)
                {                    
                    EditGroupPickViewModel groupPickModel = new EditGroupPickViewModel();
                    groupPickModel.User = user;
                    groupPickModel.Tournament = tour;
                    return View("EditGroupPick",groupPickModel);
                }
                return RedirectToAction("Tournament", new { tour_id = tour.Id });
            }

            
            //für KO Turniere
            if (tour.Modus.Id == 3 && tour.owner == user.Id)
            {
                if (tour.status <= 0)
                {
                    if (tour.Matching.Count() == 0)
                    {
                        PickTeamsViewModel pickTeams = new PickTeamsViewModel();
                        pickTeams.Tournament = tour;
                        pickTeams.User = user;
                        pickTeams.Friends = logic.giveFriends(user);
                        return View("PickTeams", pickTeams);
                    }
                    else
                    {
                        TournamentViewModel koPickModel = new TournamentViewModel();
                        List<Team> teamNames = getTeamNamesFromTour(tour);
                        koPickModel.User = user;
                        koPickModel.Tournament = tour;
                        koPickModel.TeamNames = teamNames;
                        return View("EditKOPick", koPickModel);
                    }
                }
                return RedirectToAction("Tournament", new { tour_id = tour.Id });
            }
            return Redirect("/");
        }
       
        public JsonResult searchUserPick(string user_id, string position)
        {
            User user = db.UserSet.Find(Int32.Parse(user_id));
            var data = Json(new
            {
                action = "teamsFromUser",
                teams = user.Team.Select(s => new { id = s.Id, name = s.name }),
                position = position,
            }, JsonRequestBehavior.AllowGet);
            return data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PickTeams()
        {
            Tournament tour = db.TournamentSet.Find(Int32.Parse(Request.Form["tour_id"]));
            String username = User.Identity.Name;
            var user = db.UserSet.Where(i => i.name == username).First();

            if (tour.Modus.Id == 1) { 
                List<DAL.User> userList = new List<DAL.User>();
                for (int i = 0; i < tour.countUser ; i++){
                    userList.Add(db.UserSet.Find(Int32.Parse(Request.Form[i+ "user"])));
                }
                List<DAL.Team> teamList = new List<DAL.Team>();
                for (int i = 0; i < tour.countUser; i++){
                    for (int j = 0; j < tour.countTeamsForUser; j++){
                        teamList.Add(db.TeamSet.Find(Int32.Parse(Request.Form[i + "team"+j])));
                    }
                }

                logic.pickTeams(user,userList,teamList,tour.Id);           

                return RedirectToAction("Edit", new { tour_id = tour.Id });
            }
            if (tour.Modus.Id == 2)
            {
                List<DAL.User> userList = new List<DAL.User>();
                for (int i = 0; i < tour.countUser; i++)
                {
                    userList.Add(db.UserSet.Find(Int32.Parse(Request.Form[i + "user"])));
                }
                List<DAL.Team> teamList = new List<DAL.Team>();
                for (int i = 0; i < tour.countUser; i++)
                {
                    for (int j = 0; j < tour.countTeamsForUser; j++)
                    {
                        teamList.Add(db.TeamSet.Find(Int32.Parse(Request.Form[i + "team" + j])));
                    }
                }
                logic.pickTeams(user, userList, teamList, tour.Id);

            }
            if (tour.Modus.Id == 3)
            {
                List<DAL.User> userList = new List<DAL.User>();
                for (int i = 0; i < tour.countUser; i++)
                {
                    userList.Add(db.UserSet.Find(Int32.Parse(Request.Form[i + "user"])));
                }
                List<DAL.Team> teamList = new List<DAL.Team>();
                for (int i = 0; i < tour.countUser; i++)
                {
                    for (int j = 0; j < tour.countTeamsForUser; j++)
                    {
                        teamList.Add(db.TeamSet.Find(Int32.Parse(Request.Form[i + "team" + j])));
                    }
                }
                logic.pickTeams(user, userList, teamList, tour.Id);
                
                TournamentViewModel koPickModel = new TournamentViewModel();
                List<Team> teamNames = getTeamNamesFromTour(tour);
                koPickModel.User = user;
                koPickModel.Tournament = tour;
                koPickModel.TeamNames = teamNames;
                return View("EditKOPick", koPickModel);
            }            

            return RedirectToAction("Edit", new { tour_id = tour.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewKOPick()
        {
            logic.newKOPick(Int32.Parse(Request.Form["tour_id"]));
            return RedirectToAction("Edit", new { tour_id = Int32.Parse(Request.Form["tour_id"]) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MixTeamsToGroupNew()
        {
            Tournament tour = db.TournamentSet.Find(Int32.Parse(Request.Form["tour_id"]));
            logic.mixTeamsToGroupNew(Int32.Parse(Request.Form["tour_id"]));
            return RedirectToAction("Edit", new { tour_id = Int32.Parse(Request.Form["tour_id"]) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartTournament()
        {
            int tour_id = Int32.Parse(Request.Form["tour_id"]);
            logic.startTournament(tour_id);
            return RedirectToAction("Tournament", new { tour_id = tour_id });
        }

        
        public ActionResult Tournament(int? tour_id)
        {
            String username = User.Identity.Name;
            DAL.User user = null;
            if (username != "")
            {
                user = db.UserSet.Where(i => i.name == username).First();
            }else
            {
                User tempUser = new DAL.User();
                tempUser.name = "";
                user = tempUser;
            }
            
            TournamentViewModel tourModel = new TournamentViewModel();
            Tournament tour = db.TournamentSet.Find(tour_id);
            List<Team> teamNames = getTeamNamesFromTour(tour);

            tourModel.Tournament = tour;
            tourModel.User = user;
            tourModel.TeamNames = teamNames;

            return View("Tournament",tourModel);
        }
        
        private List<Team> getTeamNamesFromTour(Tournament tour)
        {
            List<Team> teamNames = new List<Team>();
            if (tour.GroupCard.Count() > 0)
            {
                foreach(var card in tour.GroupCard)
                {
                    teamNames.Add(card.Team);
                }
            }else
            {
                foreach (var match in tour.Matching)
                {
                    Boolean saveTeamA = true;
                    Boolean saveTeamB = true;
                    foreach(var item in teamNames)
                    {
                        if (match.teamA == item.Id)
                        {
                            saveTeamA = false;
                        }
                        if (match.teamB == item.Id)
                        {
                            saveTeamB = false;
                        }
                    }
                    if (saveTeamA && match.teamA != -1)
                    {
                        teamNames.Add(db.TeamSet.Find(match.teamA));
                    }
                    if (saveTeamB && match.teamB != -1)
                    {
                        teamNames.Add(db.TeamSet.Find(match.teamB));
                    }
                    saveTeamA = true;
                    saveTeamB = true;
                }
            }
            return teamNames;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMatch()
        {
            Matching match = db.MatchingSet.Find((Int32.Parse(Request.Form["match_id"])));
            int goalA;
            int goalB;
            bool checkgoalA = Int32.TryParse(Request.Form["goalA"],out goalA);

            bool checkgoalB = Int32.TryParse(Request.Form["goalB"],out goalB);
            if (checkgoalA && checkgoalB)
            {
                logic.saveMatch(match, goalA, goalB);
            }
            
            Tournament tour = match.Tournament;
            return RedirectToAction("Tournament", new { tour_id = tour.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveMatchKO()
        {            
            Matching match = db.MatchingSet.Find((Int32.Parse(Request.Form["match_id"])));
            Tournament tour = match.Tournament;
            if (db.TeamSet.Where(i => i.Id == match.teamA).Count() == 0 || db.TeamSet.Where(i => i.Id == match.teamB).Count() == 0)
            {
                return RedirectToAction("Tournament", new { tour_id = tour.Id });
            }
            if (Int32.Parse(Request.Form["goalA"]) == Int32.Parse(Request.Form["goalB"]))
            {
                return RedirectToAction("Tournament", new { tour_id = tour.Id });
            }
            logic.saveMatchKO(match, Int32.Parse(Request.Form["goalA"]), Int32.Parse(Request.Form["goalB"]));            
            return RedirectToAction("Tournament", new { tour_id = tour.Id });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EndGroup()
        {
            logic.endGroup(Int32.Parse(Request.Form["tour_id"]));
            return Redirect("/");
        }
        public ActionResult FromGroupToKO()
        {            
            logic.fromGroupToKO(Int32.Parse(Request.Form["tour_id"]));
            return RedirectToAction("Tournament", new { tour_id = Int32.Parse(Request.Form["tour_id"]) });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TournamentKOFinished()
        {
            logic.tournamentKOFinished(Int32.Parse(Request.Form["tour_id"]));
            return Redirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartKOTournament()
        {
            logic.startKOTournament(Int32.Parse(Request.Form["tour_id"]));
            return RedirectToAction("Tournament", new { tour_id = Int32.Parse(Request.Form["tour_id"]) });
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

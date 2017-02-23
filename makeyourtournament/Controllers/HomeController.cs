using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using makeyourtournament.Models;

namespace makeyourtournament.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContainer db = new DatabaseContainer();
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                LandingViewModel model = new LandingViewModel();

                String username = User.Identity.Name;
                var user = db.UserSet.Where(i => i.name == username).First();

                List<DAL.User> friendList = new List<User>();
                foreach (var item in user.User_has_friends)
                {
                    User friend = db.UserSet.Find(item.friend_id);
                    friendList.Add(friend);
                }

                model.User = user;
                model.FriendList = friendList;

                return View(model);               
            }
            return View();
        }

        public ActionResult Impressum()
        {
            ViewBag.Message = "Impressum";

            return View();
        }

       
    }
}
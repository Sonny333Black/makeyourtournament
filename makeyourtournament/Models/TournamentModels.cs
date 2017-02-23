using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace makeyourtournament.Models
{
    public class TeamEdit
    {
        public IEnumerable<DAL.Team> TeamList { get; set; }

        public DAL.Team TeamDAL { get; set; }


    }

    public class LandingViewModel
    {
        public DAL.User User { get; set; }

        public IEnumerable<DAL.User> FriendList { get; set; }
    }

    public class CreateTournamentViewModel
    {
        public DAL.Tournament TournamentDAL { get; set; }

        public IEnumerable<DAL.Modus> Modus { get; set; }
    }

    public class PickTeamsViewModel
    {
        public DAL.Tournament Tournament { get; set; }
        
        public DAL.User User { get; set; }

        public IEnumerable<DAL.User> Friends { get; set; }
    }
    public class EditGroupPickViewModel
    {
        public DAL.Tournament Tournament { get; set; }
        public DAL.User User { get; set; }
    }
    public class TournamentViewModel
    {
        public DAL.Tournament Tournament { get; set; }
        public DAL.User User { get; set; }

        public List<DAL.Team> TeamNames { get; set; }
    }

    public class KOMatchesViewModel
    {
        public DAL.Tournament Tournament { get; set; }
        public DAL.User User { get; set; }
        public DAL.Matching Match { get; set; }

        public List<DAL.Team> TeamNames { get; set; }
    }


}

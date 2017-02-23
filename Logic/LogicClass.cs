using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using WebSocketServer;
using WebSocket4Net;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Logic
{
    public class LogicClass
    {
        private WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket("ws://localhost:22888/Handler1.ashx");
        private DatabaseContainer db;
        private String msg;
        

        public LogicClass(DatabaseContainer db)
        {
            this.db = db;            
        }

        public Statistic makeZeroStatistik()
        {
            Statistic stat = new Statistic();
            stat.goals = 0;
            stat.owngoals = 0;
            stat.points = 0;
            stat.wins = 0;
            stat.loses = 0;
            stat.draws = 0;
            stat.totalGames = 0;
            db.StatisticSet.Add(stat);
            return stat;
            
        }
        private void initSocket()
        {
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            websocket.Closed += new EventHandler(websocket_Closed);
            websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
            websocket.Open();
        }
        public void newUser(User user)
        {
            Statistic tempStat = makeZeroStatistik();

            user.friend_key = generateRandomString(50);

            user.Statistic = tempStat;
            db.StatisticSet.Add(tempStat);
            db.UserSet.Add(user);
            db.SaveChangesAsync();
        }
        public void deleteUser(User user)
        {
            User dump = db.UserSet.Where(i => i.email == "dumpUser@makeyourtournament.de").First();
            foreach (var item in user.Team)
            {
                dump.Team.Add(item);
            }

            db.StatisticSet.Remove(user.Statistic);
            db.UserSet.Remove(user);
            db.SaveChanges();
        }
        public static String generateRandomString(int length = 50)
        {
            Random random = new Random();
            String characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int characterLength = characters.Length;
            String randomString = "";
            for (int i = 0; i < length; i++)
            {
                randomString += characters[random.Next(0, characterLength - 1)];
            }
            return randomString;
        }

        public void newTeam(Team team,User user)
        {            
            Statistic tempStat = makeZeroStatistik();
            User myUser = db.UserSet.Find(user.Id);
            Team newTeam = team;
            newTeam.Statistic = tempStat;
            newTeam.User = myUser;
            newTeam.UserId = myUser.Id;
            
            db.StatisticSet.Add(tempStat);
            db.TeamSet.Add(newTeam);
            db.SaveChanges();

            initSocket();
            msg = "Team " + newTeam.name + " wurde erstellt.";
        }

        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            //es gibt keine Errors
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            //beenden
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //alles gut gegangen
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            websocket.Send(msg);
            websocket.Close();
        }

        public void newFriend(User user,String key)
        {

            User friend = db.UserSet.Where(i => i.friend_key == key).FirstOrDefault();
            int isFriend = 3;
            if (friend != null)
            {
                isFriend = db.User_has_friendsSet.Where(i => i.User_Id == user.Id && i.friend_id == friend.Id).Count();
            }
            if (isFriend == 0 && friend != null)
            {
                User_has_friends newFriend = new User_has_friends();
                newFriend.User = user;
                newFriend.UserId = user.Id;
                newFriend.User_Id = user.Id;
                newFriend.friend_id = friend.Id;
                db.User_has_friendsSet.Add(newFriend);
                db.SaveChanges();

                User_has_friends newFriendForFriend = new User_has_friends();
                newFriendForFriend.User = friend;
                newFriendForFriend.UserId = friend.Id;
                newFriendForFriend.User_Id = friend.Id;
                newFriendForFriend.friend_id = user.Id;
                db.User_has_friendsSet.Add(newFriendForFriend);
                db.SaveChanges();
            }
        }

        public void deleteFriend(User user, int friend_id)
        {
            User_has_friends temp = db.User_has_friendsSet.Where(i => i.User_Id == user.Id && i.friend_id == friend_id).First();
            db.User_has_friendsSet.Remove(temp);

            User_has_friends temp2 = db.User_has_friendsSet.Where(i => i.User_Id == friend_id && i.friend_id == user.Id).First();
            db.User_has_friendsSet.Remove(temp2);

            db.SaveChanges();
        }

        public void pickTeams(User user, List<DAL.User> userList, List<DAL.Team> teamList,int tour_id) {
            Tournament tour = db.TournamentSet.Find(tour_id);

            for (int i = 0; i < userList.Count(); i++)
            {
                if (userList[i].Id != user.Id)
                {
                    userList[i].Tournament.Add(tour);
                }
            }

            if (tour.Modus.Id == 1)
            {                
                for (int i = 0; i < teamList.Count(); i++)
                {
                    GroupCard card = new GroupCard();
                    card.groupNumber = 0;
                    card.Statistic = makeZeroStatistik();
                    card.Team = teamList[i];
                    card.TeamId = teamList[i].Id;
                    card.Tournament = tour;
                    card.TournamentId = tour.Id;
                    db.GroupCardSet.Add(card);
                    db.SaveChanges();
                }
                mixTeamsToGroups(tour);
                db.SaveChanges();
            }
            if (tour.Modus.Id == 2)
            {
                for (int i = 0; i < teamList.Count(); i++)
                {
                    GroupCard card = new GroupCard();
                    card.groupNumber = 1;
                    card.Statistic = makeZeroStatistik();
                    card.Team = teamList[i];
                    card.TeamId = teamList[i].Id;
                    card.Tournament = tour;
                    card.TournamentId = tour.Id;
                    db.GroupCardSet.Add(card);
                }
                getMatching(tour);
                tour.status = 1;
                db.SaveChanges();
            }
            if (tour.Modus.Id == 3)
            {
                makeKOMatches(tour, teamList);
            }

        }
        protected void mixTeamsToGroups(Tournament tour)
        {
            foreach (var item in tour.GroupCard)
            {
                item.groupNumber = 0;
            }
            db.SaveChanges();
            //wird immer aufgerundet
            int countTeamsPerGroup = (int)Math.Ceiling(((double)tour.GroupCard.Count() / tour.countGroups));
            foreach (var item in tour.GroupCard)
            {
                Random rnd = new Random();
                int randomNumber = rnd.Next(1, tour.countGroups + 1);
                int numberOfMembersInGroup = db.GroupCardSet.Where(i => i.groupNumber == randomNumber && i.TournamentId == tour.Id).Count();
                while (numberOfMembersInGroup >= countTeamsPerGroup)
                {
                    randomNumber = rnd.Next(1, tour.countGroups + 1);
                    numberOfMembersInGroup = db.GroupCardSet.Where(i => i.groupNumber == randomNumber && i.TournamentId == tour.Id).Count();
                }
                item.groupNumber = randomNumber;
                db.SaveChanges();
            }
        }
        public void createTournament(User user,Tournament tournament)
        {
            tournament.User.Add(user);
            tournament.owner = user.Id;
            db.TournamentSet.Add(tournament);
            db.SaveChanges();

            initSocket();
            msg = "Turnier " + tournament.name + " wurde erstellt.";
        }
        public List<DAL.User> giveFriends(User user)
        {
            List<DAL.User> friends = new List<DAL.User>();
            friends.Add(user);
            foreach (var item in user.User_has_friends)
            {
                User tempFriend = db.UserSet.Find(item.friend_id);
                friends.Add(tempFriend);
            }
            return friends;
        }

        public void newKOPick(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            List<DAL.Team> teamList = new List<DAL.Team>();
            if (tour.status == 0)
            {
                foreach (var match in tour.Matching)
                {
                    if (match.teamA != -1 && match.teamB != -1)
                    {
                        teamList.Add(db.TeamSet.Find(match.teamA));
                        teamList.Add(db.TeamSet.Find(match.teamB));
                    }
                }
                db.MatchingSet.RemoveRange(db.MatchingSet.Where(i => i.Tournament.Id == tour.Id));
                makeKOMatches(tour, teamList);
            }
        }
        public void mixTeamsToGroupNew(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            mixTeamsToGroups(tour);
        }
        protected void getMatching(Tournament tour)
        {
            List<List<GroupCard>> teamList = new List<List<GroupCard>>();
            for (int i = 0; i < tour.countGroups; i++)
            {
                int teamCountInGroup = 0;
                foreach (var item in tour.GroupCard)
                {
                    if ((item.groupNumber - 1) == i)
                    {
                        teamList.Add(new List<GroupCard>());
                        teamList[i].Add(item);

                        teamCountInGroup++;
                    }
                }
            }
            for (int groupNumber = 0; groupNumber < tour.countGroups; groupNumber++)
            {
                if (teamList[groupNumber].Count() % 2 == 1)
                {
                    int a = teamList[groupNumber].Count() - 1;
                    int n = (teamList[groupNumber].Count() - 1) / 2;

                    for (int i = 0; i < teamList[groupNumber].Count(); i++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            makeMatch(teamList[groupNumber][k], teamList[groupNumber][a - k], tour);
                        }
                        for (int l = 0; l < teamList[groupNumber].Count() - 1; l++)
                        {
                            GroupCard temp = teamList[groupNumber][l];
                            teamList[groupNumber][l] = teamList[groupNumber][l + 1];
                            teamList[groupNumber][l + 1] = temp;
                        }
                    }
                }
                else
                {
                    int a = teamList[groupNumber].Count() - 1;
                    int n = teamList[groupNumber].Count() / 2;

                    for (int i = 0; i < teamList[groupNumber].Count() - 1; i++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            makeMatch(teamList[groupNumber][k], teamList[groupNumber][a - k], tour);
                        }
                        for (int l = 1; l < teamList[groupNumber].Count() - 1; l++)
                        {
                            GroupCard temp = teamList[groupNumber][l];
                            teamList[groupNumber][l] = teamList[groupNumber][l + 1];
                            teamList[groupNumber][l + 1] = temp;
                        }
                    }
                }
            }

            if (tour.Modus.Id == 2)
            {
                for (int groupNumber = 0; groupNumber < tour.countGroups; groupNumber++)
                {
                    if (teamList[groupNumber].Count() % 2 == 1)
                    {
                        int a = teamList[groupNumber].Count() - 1;
                        int n = (teamList[groupNumber].Count() - 1) / 2;

                        for (int i = 0; i < teamList[groupNumber].Count(); i++)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                makeMatch(teamList[groupNumber][a - k], teamList[groupNumber][k], tour);
                            }
                            for (int l = 0; l < teamList[groupNumber].Count() - 1; l++)
                            {
                                GroupCard temp = teamList[groupNumber][l];
                                teamList[groupNumber][l] = teamList[groupNumber][l + 1];
                                teamList[groupNumber][l + 1] = temp;
                            }
                        }
                    }
                    else
                    {
                        int a = teamList[groupNumber].Count() - 1;
                        int n = teamList[groupNumber].Count() / 2;

                        for (int i = 0; i < teamList[groupNumber].Count() - 1; i++)
                        {
                            for (int k = 0; k < n; k++)
                            {
                                makeMatch(teamList[groupNumber][a - k], teamList[groupNumber][k], tour);
                            }
                            for (int l = 1; l < teamList[groupNumber].Count() - 1; l++)
                            {
                                GroupCard temp = teamList[groupNumber][l];
                                teamList[groupNumber][l] = teamList[groupNumber][l + 1];
                                teamList[groupNumber][l + 1] = temp;
                            }
                        }
                    }
                }
            }
        }
        protected void makeMatch(GroupCard cardA, GroupCard cardB, Tournament tour)
        {
            Matching match = new Matching();
            match.teamA = cardA.Team.Id;
            match.teamB = cardB.Team.Id;
            match.goalA = -1;
            match.goalB = -1;
            match.Round = db.RoundSet.Find(1);
            tour.Matching.Add(match);

            db.SaveChanges();
        }
        public void startTournament(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            getMatching(tour);
            tour.status = 1;
            db.SaveChanges();
        }

        public void saveMatch(Matching match, int goalA, int goalB)
        {
            if (match.goalA == -1)
            {
                saveNewMatch(match, goalA, goalB);
            }
            else
            {
                clearMatch(match);
                saveNewMatch(match, goalA, goalB);
            }
            
        }

        protected void clearMatch(Matching match)
        {
            Tournament tour = match.Tournament;
            int goalA = match.goalA;
            int goalB = match.goalB;

            Team teamA = db.TeamSet.Find(match.teamA);
            GroupCard cardA = db.GroupCardSet.Where(i => i.TeamId == teamA.Id && i.Tournament.Id == tour.Id).First();
            Statistic statCard = db.StatisticSet.Find(cardA.Statistic.Id);
            removePointsToStat(statCard, goalA, goalB);


            Team teamB = db.TeamSet.Find(match.teamB);
            GroupCard cardB = db.GroupCardSet.Where(i => i.TeamId == teamB.Id && i.Tournament.Id == tour.Id).First();
            statCard = db.StatisticSet.Find(cardB.Statistic.Id);
            removePointsToStat(statCard, goalB, goalA);


            if (teamA.User.Id != teamB.User.Id)
            {
                statCard = db.StatisticSet.Find(teamA.Statistic.Id);
                removePointsToStat(statCard, goalA, goalB);
                User user = teamA.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                removePointsToStat(statCard, goalA, goalB);

                statCard = db.StatisticSet.Find(teamB.Statistic.Id);
                removePointsToStat(statCard, goalB, goalA);
                user = teamB.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                removePointsToStat(statCard, goalB, goalA);
            }
        }
        protected void clearMatchKO(DAL.Matching match)
        {
            Tournament tour = match.Tournament;
            int goalA = match.goalA;
            int goalB = match.goalB;

            if (match.goalA != -1 && match.goalB != -1)
            {
                Team teamA = db.TeamSet.Find(match.teamA);
                Statistic statCard = db.StatisticSet.Find(teamA.Statistic.Id);
                removePointsToStat(statCard, goalA, goalB);
                User user = teamA.User;
                statCard = (db.StatisticSet.Find(user.Statistic.Id));
                removePointsToStat(statCard, goalA, goalB);

                Team teamB = db.TeamSet.Find(match.teamB);
                statCard = db.StatisticSet.Find(teamB.Statistic.Id);
                removePointsToStat(statCard, goalB, goalA);
                user = teamB.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                removePointsToStat(statCard, goalB, goalA);
            }
        }
        protected void removePointsToStat(Statistic statCard, int goals, int owngoals)
        {
            int result = giveResult(statCard.goals, statCard.owngoals);
            statCard.goals = statCard.goals - goals;
            statCard.owngoals = statCard.owngoals - owngoals;
            if (result == 1)
            {
                statCard.points = statCard.points - 3;
                statCard.wins = statCard.wins - 1;
            }
            else if (result == 2)
            {
                statCard.points = statCard.points - 1;
                statCard.draws = statCard.draws - 1;
            }
            else
            {
                statCard.points = statCard.points - 0;
                statCard.loses = statCard.loses - 1;
            }
            statCard.totalGames = statCard.totalGames - 1;
            db.SaveChanges();
        }
        protected int giveResult(int goals, int owngoals)
        {
            if (goals > owngoals)
            {
                return 1;
            }
            if (goals == owngoals)
            {
                return 2;
            }
            if (goals < owngoals)
            {
                return 3;
            }
            return 4;
        }
        protected void saveNewMatch(Matching match, int goalA, int goalB)
        {
            Tournament tour = match.Tournament;

            match.goalA = goalA;
            match.goalB = goalB;
            db.SaveChanges();

            Team teamA = db.TeamSet.Find(match.teamA);
            Team teamB = db.TeamSet.Find(match.teamB);

            GroupCard cardA = db.GroupCardSet.Where(i => i.TeamId == teamA.Id && i.Tournament.Id == tour.Id).First();
            Statistic statCard = db.StatisticSet.Find(cardA.Statistic.Id);
            givePointsToStat(statCard, goalA, goalB);

            GroupCard cardB = db.GroupCardSet.Where(i => i.TeamId == teamB.Id && i.Tournament.Id == tour.Id).First();
            statCard = db.StatisticSet.Find(cardB.Statistic.Id);
            givePointsToStat(statCard, goalB, goalA);

            if (teamA.User.Id != teamB.User.Id)
            {
                statCard = db.StatisticSet.Find(teamA.Statistic.Id);
                givePointsToStat(statCard, goalA, goalB);
                User user = teamA.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                givePointsToStat(statCard, goalA, goalB);

                statCard = db.StatisticSet.Find(teamB.Statistic.Id);
                givePointsToStat(statCard, goalB, goalA);
                user = teamB.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                givePointsToStat(statCard, goalB, goalA);
            }

            initSocket();
            msg = teamA.name + " " + goalA + " : " + goalB + " " + teamB.name;

        }
        protected void saveNewMatchKO(Matching match, int goalA, int goalB)
        {
            Tournament tour = match.Tournament;
            match.goalA = goalA;
            match.goalB = goalB;
            db.SaveChanges();


            Team teamA = db.TeamSet.Find(match.teamA);
            Team teamB = db.TeamSet.Find(match.teamB);
            if (teamA.User.Id != teamB.User.Id)
            {
                Statistic statCard = db.StatisticSet.Find(teamA.Statistic.Id);
                givePointsToStat(statCard, goalA, goalB);
                User user = teamA.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                givePointsToStat(statCard, goalA, goalB);

                statCard = db.StatisticSet.Find(teamB.Statistic.Id);
                givePointsToStat(statCard, goalB, goalA);
                user = teamB.User;
                statCard = db.StatisticSet.Find(user.Statistic.Id);
                givePointsToStat(statCard, goalB, goalA);
            }
        }
        protected void givePointsToStat(Statistic statCard, int goals, int owngoals)
        {
            statCard.goals = statCard.goals + goals;
            statCard.owngoals = statCard.owngoals + owngoals;
            int result = giveResult(goals, owngoals);
            if (result == 1)
            {
                statCard.points = statCard.points + 3;
                statCard.wins = statCard.wins + 1;
            }
            else if (result == 2)
            {
                statCard.points = statCard.points + 1;
                statCard.draws = statCard.draws + 1;
            }
            else
            {
                statCard.points = statCard.points + 0;
                statCard.loses = statCard.loses + 1;
            }
            statCard.totalGames = statCard.totalGames + 1;
            db.SaveChanges();
        }

        public void endGroup(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            tour.status = 3;
            GroupCard card = db.GroupCardSet.Where(i => i.Tournament.Id == tour.Id).OrderByDescending(x => x.Statistic.points).ThenByDescending(x => x.Statistic.goals).ThenByDescending(x => x.Statistic.owngoals).First();
            tour.winner = card.Team.Id;
            db.SaveChanges();
        }
        public void fromGroupToKO(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            tour.status = 2;
            db.SaveChanges();
            List<GroupCard> cards = new List<GroupCard>();
            cards = db.GroupCardSet.Where(i => i.Tournament.Id == tour.Id).OrderByDescending(x => x.Statistic.points).ThenByDescending(x => x.Statistic.goals).ThenByDescending(x => x.Statistic.owngoals).ToList();

            List<List<GroupCard>> tourArrayWithTeams = new List<List<GroupCard>>();

            for (int i = 0; i < tour.countGroups; i++)
            {
                int j = 0;
                foreach (var card in cards)
                {
                    if (card.groupNumber == i + 1)
                    {
                        tourArrayWithTeams.Add(new List<GroupCard>());
                        tourArrayWithTeams[i].Add(card);
                        j++;
                    }
                }
            }

            int anzahlGruppen = tour.countGroups;
            List<List<GroupCard>> gruppen = tourArrayWithTeams;
            int roundstartAt = 0;
            if (anzahlGruppen < 3)
            {
                roundstartAt = 4;
            }
            else if (anzahlGruppen < 5)
            {
                roundstartAt = 3;
            }
            else if (anzahlGruppen >= 5)
            {
                roundstartAt = 2;
            }

            if (anzahlGruppen % 2 == 0)
            {
                for (int i = 0; i < anzahlGruppen; i = i + 2)
                {

                    Matching match = new Matching();
                    match.teamA = gruppen[i][0].Team.Id;
                    match.teamB = gruppen[i + 1][1].Team.Id;
                    match.goalA = -1;
                    match.goalB = -1;
                    match.Round = db.RoundSet.Find(roundstartAt);
                    tour.Matching.Add(match);
                    db.SaveChanges();

                    match = new Matching();
                    match.teamA = gruppen[i + 1][0].Team.Id;
                    match.teamB = gruppen[i][1].Team.Id;
                    match.goalA = -1;
                    match.goalB = -1;
                    match.Round = db.RoundSet.Find(roundstartAt);
                    tour.Matching.Add(match);
                    db.SaveChanges();
                }
            }
            if (roundstartAt == 2)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }
            else if (roundstartAt == 3)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }
            else if (roundstartAt == 4)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }
        }
        protected void goToNextRound(Matching match)
        {
            Round round = match.Round;
            Tournament tour = match.Tournament;
            List<Matching> matchesTemp = new List<Matching>();
            matchesTemp = db.MatchingSet.Where(i => i.Tournament.Id == tour.Id && i.Round.Id > 1).ToList();
            List<Matching> matches = new List<Matching>();
            matches.Add(null);
            int currentArrayNumber = 0;
            for (int i = 0; i < matchesTemp.Count(); i++)
            {
                matches.Add(matchesTemp[i]);
            }
            for (int i = 1; i < matches.Count(); i++)
            {
                if (matches[i].Id == match.Id)
                {
                    currentArrayNumber = i;
                }
            }
            int matchNr = currentArrayNumber;
            int indikator = 0;

            switch (round.Id)
            {
                case 2:
                    indikator = 8;
                    break;
                case 3:
                    if (matchNr <= 4)
                    {
                        indikator = 4;
                    }
                    else
                    {
                        indikator = 8;
                    }
                    break;
                case 4:
                    if (matchNr <= 2)
                    {
                        indikator = 2;
                    }

                    else if (matchNr == 5 || matchNr == 6)
                    {
                        indikator = 4;
                    }
                    else
                    {
                        indikator = 8;
                    }
                    break;
                case 5:
                    if (match.goalA > match.goalB)
                    {
                        tour.winner = match.teamA;
                        db.SaveChanges();
                    }
                    if (match.goalA < match.goalB)
                    {
                        tour.winner = match.teamB;
                        db.SaveChanges();
                    }
                    return;
            }

            if (match.goalA > match.goalB)
            {
                int newMatchNumber = 0;
                if (matchNr % 2 == 1)
                {
                    newMatchNumber = (matchNr + indikator) - ((matchNr - 1) / 2);
                    clearMatchKO(matches[newMatchNumber]);
                    matches[newMatchNumber].teamA = match.teamA;
                }
                else
                {
                    newMatchNumber = (matchNr + indikator) - (matchNr / 2);
                    clearMatchKO(matches[newMatchNumber]);
                    matches[newMatchNumber].teamB = match.teamA;
                }

                db.SaveChanges();

            }
            if (match.goalA == match.goalB)
            {
                return;
            }
            if (match.goalA < match.goalB)
            {
                int newMatchNumber = 0;
                if (matchNr % 2 == 1)
                {
                    newMatchNumber = (matchNr + indikator) - ((matchNr - 1) / 2);
                    clearMatchKO(matches[newMatchNumber]);
                    matches[newMatchNumber].teamA = match.teamB;
                }
                else
                {
                    newMatchNumber = (matchNr + indikator) - (matchNr / 2);
                    clearMatchKO(matches[newMatchNumber]);
                    matches[newMatchNumber].teamB = match.teamB;
                }
                db.SaveChanges();
            }
        }
        public void tournamentKOFinished(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            tour.status = 3;
            db.SaveChanges();
        }
        public void startKOTournament(int tour_id)
        {
            Tournament tour = db.TournamentSet.Find(tour_id);
            tour.status = 2;
            db.SaveChanges();
        }
        public void saveMatchKO(Matching match,int newGoalA,int newGoalB)
        {
            if (match.goalA == -1 || match.goalB == -1)
            {
                saveNewMatchKO(match, newGoalA, newGoalB);
                goToNextRound(match);
            }
            else
            {
                clearMatchKO(match);
                saveNewMatchKO(match, newGoalA, newGoalB);
                goToNextRound(match);
            }
        }
        private List<DAL.Team> shuffelTeamList(List<DAL.Team> list)
        {
            List<DAL.Team> tempList = list;
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                DAL.Team value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return tempList;
        }
        protected void makeKOMatches(Tournament tour, List<DAL.Team> teamList)
        {
            int countTeams = teamList.Count();
            teamList = shuffelTeamList(teamList);
            int roundstartAt = 0;
            if (countTeams <= 4)
            {
                roundstartAt = 4;
            }
            else if (countTeams <= 8)
            {
                roundstartAt = 3;
            }
            else if (countTeams <= 16)
            {
                roundstartAt = 2;
            }

            for (int i = 0; i < countTeams; i = i + 2)
            {
                Matching match = new Matching();
                match.teamA = teamList[i].Id;
                match.teamB = teamList[i + 1].Id;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(roundstartAt);
                tour.Matching.Add(match);
                db.SaveChanges();
            }

            if (roundstartAt == 2)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(3);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }
            else if (roundstartAt == 3)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(4);
                tour.Matching.Add(match);
                db.SaveChanges();
                match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }
            else if (roundstartAt == 4)
            {
                Matching match = new Matching();
                match.teamA = -1;
                match.teamB = -1;
                match.goalA = -1;
                match.goalB = -1;
                match.Round = db.RoundSet.Find(5);
                tour.Matching.Add(match);
                db.SaveChanges();
            }

            db.SaveChanges();
        }
        
    }
    
}

namespace makeyourtournament.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Logic;
    using Models;
    using Microsoft.AspNet.Identity;
    using DAL;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<makeyourtournament.Models.ApplicationDbContext>
    {
        private LogicClass logic;
        private DatabaseContainer db = new DatabaseContainer();

        public Configuration()
        {
            logic = new LogicClass(db);
            AutomaticMigrationsEnabled = false;
            ContextKey = "makeyourtournament.Models.ApplicationDbContext";
        }

        protected override void Seed(makeyourtournament.Models.ApplicationDbContext context)
        {

            makeModusForTournament();
            makeRoundsForTournament();

            String email = "test@test.de";
            String username = "Test";
            String pass = "123";


            context.Users.AddOrUpdate(x => x.Id,
                new ApplicationUser() { UserName = username, PasswordHash = pass, Email = email }
                );


            User user2 = new User();
            user2.email = email;
            user2.name = username;
            user2.friend_key = generateRandomString(50);
            Statistic stat = logic.makeZeroStatistik();
            user2.Statistic = stat;
            db.StatisticSet.Add(stat);
            db.UserSet.Add(user2);
            db.SaveChanges();

            user2 = new User();
            user2.email = "dumpUser@makeyourtournament.de";
            user2.name = "gelöschter User";
            user2.friend_key = generateRandomString(50);
            stat = logic.makeZeroStatistik();
            user2.Statistic = stat;
            db.StatisticSet.Add(stat);
            db.UserSet.Add(user2);
            db.SaveChanges();

        }

        private void makeModusForTournament()
        {
            Modus mod = new Modus();
            mod.name = "Gruppen & KO";
            mod.description = "Erst wird in Gruppen gespielt und dann KO Phase.";
            db.ModusSet.Add(mod);

            Modus mod2 = new Modus();
            mod2.name = "Liga";
            mod2.description = "Es wird nur jeder gegne jeden gespielt.";
            db.ModusSet.Add(mod2);

            Modus mod3 = new Modus();
            mod3.name = "KO";
            mod3.description = "Es wird direkt in die KO-Phase gehen.";
            db.ModusSet.Add(mod3);

            db.SaveChanges();

        }

        private void makeRoundsForTournament()
        {
            Round round1 = new Round();
            round1.name = "Gruppen Phase";
            db.RoundSet.Add(round1);

            Round round2 = new Round();
            round2.name = "Achtelfinale";
            db.RoundSet.Add(round2);

            Round round3 = new Round();
            round3.name = "Viertelfinale";
            db.RoundSet.Add(round3);

            Round round4 = new Round();
            round4.name = "Halbfinale";
            db.RoundSet.Add(round4);

            Round round5 = new Round();
            round5.name = "Finale";
            db.RoundSet.Add(round5);

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
    }


}

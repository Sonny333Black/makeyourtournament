﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DatabaseContainer : DbContext
    {
        public DatabaseContainer()
            : base("name=DatabaseContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Statistic> StatisticSet { get; set; }
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<Tournament> TournamentSet { get; set; }
        public virtual DbSet<Team> TeamSet { get; set; }
        public virtual DbSet<GroupCard> GroupCardSet { get; set; }
        public virtual DbSet<Modus> ModusSet { get; set; }
        public virtual DbSet<Matching> MatchingSet { get; set; }
        public virtual DbSet<Round> RoundSet { get; set; }
        public virtual DbSet<User_has_friends> User_has_friendsSet { get; set; }
    }
}

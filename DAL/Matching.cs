//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Matching
    {
        public int Id { get; set; }
        public int teamA { get; set; }
        public int teamB { get; set; }
        public int goalA { get; set; }
        public int goalB { get; set; }
        public Nullable<int> TournamentId { get; set; }
    
        public virtual Round Round { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}

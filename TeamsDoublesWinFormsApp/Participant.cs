using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsDoublesWinFormsApp
{

    public class Participant
    {
        public string? ID;
        public string? Name;
        public string? NameEN;
        public string? NameSR;
        public string? NameExtra;
        public string? Branch;
        public string? isLive;
        public string? Sport;

        public Participant(string? ID, string? Name, string? NameEN, string? NameSR, string? NameExtra, string? Branch, string? isLive, string? Sport)
        {
            this.ID = ID;
            this.Name = Name;
            this.NameEN = NameEN;
            this.NameSR = NameSR;
            this.NameExtra = NameExtra;
            this.Branch = Branch;
            this.Sport = isLive;
            this.isLive = Sport;
        }
    }
}

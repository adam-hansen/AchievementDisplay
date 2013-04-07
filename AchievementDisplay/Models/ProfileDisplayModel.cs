using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AchievementDisplay.Models
{
    public class ProfileDisplayModel
    {
        //display total games
        public int steamGameTotal { get; set; }

        public int steamActualGameTotal { get; set; }

        //display games with achievements
        public int gamesWithAch { get; set; }

        //total poss achivement
        public int possAch { get; set; }

        //total earned
        public int obtainedAch { get; set; }

        public IEnumerable<GameDisplay> listOfGames { get; set; }

        public string priceTotal { get; set; }

    }
}
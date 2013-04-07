using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AchievementDisplay.Models;

namespace AchievementDisplay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public PartialViewResult GetProfile(string q)
        {
            //pull up owned games
            if (q == null && q.Length != 17)
            {
                return null;//replace with error message
            }
            try
            {
                var client = new SteamWebAPI();
                var resp = client.GetOwnedGames(q);

                var model = new ProfileDisplayModel();
                model.steamGameTotal = resp.game_count;


                var gameStats = new List<GameDisplay>();

                //loop through each game and get the counts
                //maybe add this list to the profile display to show to the user
                int testStop = 0;

                foreach (var app in resp.games)
                {
                    var game = client.GetPlayerStatesByGame(app.appid.ToString(), q);
                    if(game != null)
                        model.steamActualGameTotal++; //if no excption on the get stats call we know its an actual game
                        

                    if (game.achievements != null)
                    {
                        var displayGame = new GameDisplay();
                        displayGame.Name = game.gameName;
                        displayGame.AppId = app.appid.ToString();
                        model.gamesWithAch++;

                        foreach (var ach in game.achievements)
                        {
                            model.possAch++;
                            displayGame.PossibleAch++;

                            if (ach.achieved != 0)
                            {
                                model.obtainedAch++;
                                displayGame.ObtainedAch++;
                            }
                                    
                        }

                        gameStats.Add(displayGame);//add player stats to be used by the model
                    }                       

                    //testStop++;
                    //if (testStop == 10)
                    //    break;
                }

                //do the logic to order and limit games here then pass into the model
                model.listOfGames = gameStats.OrderByDescending(x => x.ObtainedAch); //.OrderBy(x => x.achievements.Where(a => a.achieved != 0));

                return PartialView("_ProfileDisplay", model);//this will be real return
            }
            catch (Exception e)
            {
                //yum yum
                return null;
            }
        }

    }


}

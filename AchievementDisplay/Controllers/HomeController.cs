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

        public ActionResult Testing()
        {
            var price = new SteamWebAPI().GetPriceByGame("8890");

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public PartialViewResult GetProfile(string q)
        {
            //pull up owned games
            q = q.Trim();
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
                int totalPrice = 0;

                var gameStats = new List<GameDisplay>();

                //loop through each game and get the counts
                //maybe add this list to the profile display to show to the user
                int testStop = 0;

                foreach (var app in resp.games)
                {

                    //lookup price data
                    int price = 0;
                    if (app.appid != null)
                    {
                        price = getGamePrice(app.appid.ToString(), client);
                        if (price > 0)
                        {
                            totalPrice = totalPrice + price;
                        }
                        else
                            price = 0;
                    }

                    var game = client.GetPlayerStatesByGame(app.appid.ToString(), q);
                    if (game != null)
                    {
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

                            displayGame.price = formatPrice(price);

                            gameStats.Add(displayGame);//add player stats to be used by the model
                        }    
                    }

                    //testStop++;
                    //if (testStop == 10)
                    //    break;
                }

                model.priceTotal = formatPrice(totalPrice);

                //do the logic to order and limit games here then pass into the model
                model.listOfGames = gameStats.OrderByDescending(x => x.ObtainedAch); //.OrderBy(x => x.achievements.Where(a => a.achieved != 0));

                return PartialView("_ProfileDisplay", model);//this will be real return
            }
            catch(Exception e)
            {

                //yum
                return null;
            }
            
        }

        private string formatPrice(int input)
        {
            var str = input.ToString();
            if (str.Length < 2)
                str = "0.0" + str;
            else if (str.Length < 3)
                str = "0." + str;
            else
                str = str.Insert(str.Length - 2, ".");
            return "$" + str;
        }

        private int getGamePrice(string appId, SteamWebAPI client)
        {
            return client.GetPriceByGame(appId);
        }

    }


}

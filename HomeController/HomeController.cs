using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Tamagotchi.Controllers
{
    public class HomeController : Controller
    {

        Random rnd = new Random();
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            //Initialize the values if the session data is null 
            int full = HttpContext.Session.GetInt32("Fullness") ?? 20;
            int happy = HttpContext.Session.GetInt32("Happiness") ?? 20;
            int meal = HttpContext.Session.GetInt32("Meals") ?? 3;
            int energy = HttpContext.Session.GetInt32("Energy") ?? 50;
            if (TempData["img"] == null)
            {
                TempData["img"] = "/img/kittynutral.gif";
            }
            if (full <= 0 || energy <= 0)
            {
                HttpContext.Session.SetString("Message", "Tamagochi dided... need to take care of it better :/");
                ViewBag.Message = HttpContext.Session.GetString("Message");
                TempData["gamestatus"] = "loose";
                HttpContext.Session.Clear();
                TempData["img"] = "/img/dead.gif";
                return View("Index");
            }
            else if (full >= 100 && happy >= 100 && energy >= 100)
            {
                HttpContext.Session.SetString("Message", "Tamagotchi is all happy and good now so you won!!");
                ViewBag.Message = HttpContext.Session.GetString("Message");
                TempData["gamestatus"] = "win";
                HttpContext.Session.Clear();
                TempData["img"] = "/img/kittyfly.gif";
                return View("Index");
            }
            HttpContext.Session.SetInt32("Fullness", full);
            HttpContext.Session.SetInt32("Happiness", happy);
            HttpContext.Session.SetInt32("Meals", meal);
            HttpContext.Session.SetInt32("Energy", energy);
            ViewBag.Fullness = HttpContext.Session.GetInt32("Fullness");
            ViewBag.Happiness = HttpContext.Session.GetInt32("Happiness");
            ViewBag.Meals = HttpContext.Session.GetInt32("Meals");
            ViewBag.Energy = HttpContext.Session.GetInt32("Energy");
            ViewBag.Message = HttpContext.Session.GetString("Message");
            ViewBag.Image = TempData["img"];

            return View();
        }

        [HttpPost]
        [Route("feed")]
        public IActionResult Feed()
        {
            //Every time you play with or feed your dojodachi there should be a 25% chance that it won't like it. Energy or meals will still decrease, but happiness and fullness won't change.
            int ate = rnd.Next(5, 10);
            int remainingMeal = (int)HttpContext.Session.GetInt32("Meals");
            int remainingFull = (int)HttpContext.Session.GetInt32("Fullness");
            if (remainingMeal <= 0)
            {
                HttpContext.Session.SetString("Message", "You don't have enough meal...Make him work!");
                ViewBag.Message = HttpContext.Session.GetString("Message");
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetInt32("Meals", remainingMeal - 1);

            if (Liked())
            {
                HttpContext.Session.SetInt32("Fullness", remainingFull + ate);
                HttpContext.Session.SetString("Message", "Feeding the pet. Fullness increased by " + ate);
                TempData["img"] = "/img/kittyeat.gif";
            }
            else
            {
                HttpContext.Session.SetString("Message", "Feeding the pet. But the pet wasn't hungry...");
                TempData["img"] = "/img/kittyangry.gif";
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("play")]
        public IActionResult Play()
        {
            //Playing with your Dojodachi costs 5 energy and gains a random amount of happiness between 5 and 10
            int happy = rnd.Next(5, 10);
            int remainingEnergy = (int)HttpContext.Session.GetInt32("Energy");
            int gainedHappy = (int)HttpContext.Session.GetInt32("Happiness");
            HttpContext.Session.SetInt32("Energy", remainingEnergy - 5);
            if (Liked())
            {
                HttpContext.Session.SetInt32("Happiness", gainedHappy + happy);
                HttpContext.Session.SetString("Message", "Playing with the pet. Happiness increased by " + happy);
                TempData["img"] = "/img/kittyplay.gif";
            }
            else
            {
                HttpContext.Session.SetString("Message", "Playing with the pet. But the pet didn't feel like playing...");
                TempData["img"] = "/img/kittyangry.gif";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("work")]
        public IActionResult Work()
        {
            //Working costs 5 energy and earns between 1 and 3 meals
            int earnMeal = rnd.Next(1, 4);
            int remainingEnergy = (int)HttpContext.Session.GetInt32("Energy");
            HttpContext.Session.SetInt32("Energy", remainingEnergy - 5);
            int remainingMeal = (int)HttpContext.Session.GetInt32("Meals");
            HttpContext.Session.SetInt32("Meals", remainingMeal + earnMeal);
            HttpContext.Session.SetString("Message", "Pet is working now and earnend " + earnMeal + "meals");
            TempData["img"] = "/img/kittyworking.gif";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("sleep")]
        public IActionResult Sleep()
        {
            //Sleeping earns 15 energy and decreases fullness and happiness each by 5
            int remainingEnergy = (int)HttpContext.Session.GetInt32("Energy");
            HttpContext.Session.SetInt32("Energy", remainingEnergy + 15);
            int remainingFull = (int)HttpContext.Session.GetInt32("Fullness");
            HttpContext.Session.SetInt32("Fullness", remainingFull - 5);
            int remainingHappy = (int)HttpContext.Session.GetInt32("Happiness");
            HttpContext.Session.SetInt32("Happiness", remainingHappy - 5);
            HttpContext.Session.SetString("Message", "Pet is sleeping now. earns 15 energy and decreases fullness and happiness each by 5");
            TempData["img"] = "/img/kittysleep.gif";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("restart")]
        public IActionResult restart()
        {   
            TempData["img"] = null;
            return RedirectToAction("Index");
        }

        public bool Liked()
        {
            int liked = rnd.Next(1, 5);
            if (liked == 4)
            {
                return false;
            }
            return true;
        }
    }
}
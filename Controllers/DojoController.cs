using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Dojodachi.Controllers
{
    public class DojoController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi") == null)
            {
                HttpContext.Session.SetObjectAsJson("Dachi", new DachiInfo());
            }
            ViewBag.Dachi = HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi");
            ViewBag.Message = "Congrats a new Dachi!";
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = "";

            if (ViewBag.Dachi.fullness > 99 && ViewBag.Dachi.happiness > 99 && ViewBag.Dachi.energy > 99)
            {
                ViewBag.Message = "Yay! You won!!!!!";
            }
            return View();
        }

        [HttpGetAttribute]
        [Route("feed")]
        public IActionResult Feed()
        {
            DachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi");
            Random rand = new Random();
            ViewBag.GameStatus = "running";
            int fullnessAmount = rand.Next(5,11);
            int chance = rand.Next(1,5);
            if(EditDachi.meals > 0)
            {
                EditDachi.meals--;
                if (chance == 1)
                {
                    ViewBag.Reaction = ":(";
                    ViewBag.Message = "You fed your Dojodachi! He didn't like your food. Fullness +0, Meals -1";
                }
            }
            HttpContext.Session.SetObjectAsJson("Dachi", EditDachi);
            ViewBag.Dachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("play")]
        public IActionResult Play()
        {
            DachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi");
            Random rand = new Random();
            ViewBag.GameStatus = "running";
            int happinessAmount = rand.Next(5, 11);
            int chance = rand.Next(1, 5);
            
            if(EditDachi.energy > 4)
            {
                EditDachi.energy -= 5;
                if(chance == 1)
                {
                    ViewBag.Reaction = ":(";
                    ViewBag.Message = "You played with your Dojodachi! But he didn't like it. Happiness +0, Energy -5";
                }
                else
                {
                    EditDachi.happiness += happinessAmount;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = $"You played with your Dojodachi! Happiness +{happinessAmount}, Energy -5";
                }
            }
            HttpContext.Session.SetObjectAsJson("Dachi", EditDachi);
            ViewBag.Dachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("work")]
        public IActionResult Work()
        {
            DachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi");
            Random rand = new Random();
            ViewBag.GameStatus = "running";
            int mealsAmount = rand.Next(1, 4);
            int chance = rand.Next(1, 5);
            
            if(EditDachi.energy > 4)
            {
                EditDachi.energy -= 5;
                EditDachi.meals += mealsAmount;
                ViewBag.Reaction = ":)";
                ViewBag.Message = $"You sent your Dojodachi to work! Meals +{mealsAmount}, Energy -5";
            }
            HttpContext.Session.SetObjectAsJson("Dachi", EditDachi);
            ViewBag.Dachi = EditDachi;
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("sleep")]
        public IActionResult Sleep()
        {
            DachiInfo EditDachi = HttpContext.Session.GetObjectFromJson<DachiInfo>("Dachi");
            Random rand = new Random();
            EditDachi.fullness -= 5;
            EditDachi.happiness -= 5;
            EditDachi.energy += 15;
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = ":)";
            ViewBag.Message = "Your Dojodachi is sleeping! Energy +15, Happiness -5, Fullness -5";
            
            HttpContext.Session.SetObjectAsJson("Dachi", EditDachi);
            ViewBag.Dachi = EditDachi;
            if (ViewBag.Dachi.fullness < 1 || ViewBag.Dachi.happiness < 1)
            {
                ViewBag.Message = "O no! Your dojodachi is ded";
            }
            return View("Index");
        }

        [HttpGetAttribute]
        [Route("restart")]
        public IActionResult Restart()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index"); 
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
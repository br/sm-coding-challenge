using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sm_coding_challenge.Models;
using sm_coding_challenge.Services.DataProvider;

namespace sm_coding_challenge.Controllers
{
    public class HomeController : Controller
    {

        private IDataProvider _dataProvider;
        public HomeController(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Player(string id)
        {
            return Json(await _dataProvider.GetPlayerById(id));
        }

        [HttpGet]
        public async Task<IActionResult> Players(string id)
        {
            // dee : We should remove duplicated Ids fetched
            var idList = id.Split(',').Distinct();

            var returnList = new List<PlayerModel>();
            foreach (var anId in idList)
            {
                returnList.Add(await _dataProvider.GetPlayerById(anId));
            }
            return Json(returnList);
        }

        [HttpGet]
        public IActionResult LatestPlayers(string id)
        {
            throw new NotImplementedException("Method Needs to be Implemented");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

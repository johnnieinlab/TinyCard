using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using TinyBank.Web.Extensions;

namespace TinyBank.Web.Controllers
{
    [Route("card")]
    public class CardController : Controller
    {
        private readonly ICardService _cards;
        private readonly ILogger<HomeController> _logger;
        private readonly TinyBankDbContext _dbContext;


        public CardController(
            TinyBankDbContext dbContext,
            ILogger<HomeController> logger,
            ICardService cards)
        {
            _logger = logger;
            _cards = cards;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("checkout")]
        [HttpPost]
        public IActionResult CheckOut([FromBody] CheckoutCardOptions options)
        {
            var res = _cards.CheckOut(options);
            
                return Json(res);
            
        }
    }
}

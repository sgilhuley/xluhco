﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using xluhco.web.Repositories;

namespace xluhco.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShortLinkRepository _repo;
        private readonly IMemoryCache _memoryCache;

        public HomeController(IShortLinkRepository repo, IMemoryCache memoryCache)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [ResponseCache(Duration = int.MaxValue)]
        public IActionResult Index()
        {
            return View("Index");
        }

        [Authorize]
       public IActionResult List()
        {
            var orderedLinks = _memoryCache.GetOrCreate("allLinks", entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return _repo.GetShortLinks().OrderBy(x => x.ShortLinkCode).ToList();
            });

            return View("List", orderedLinks);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using protoUnitofWorkDapper.Models;

namespace protoUnitofWorkDapper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration config;


        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            this.config = config;
        }

        // GET: MembersController
        public ActionResult Index()
        {
            var members = new List<Member>();

            using (var dal = new DalSession(config))
            {
                members = new MemberRepository(dal.UnitOfWork).Get();
            }

            return View(members);
        }

        public ActionResult Index2(int page = 0, int size = 10)
        {
            PagedResults<Member> pagedResults;

            using (var dal = new DalSession(config))
            {
                pagedResults = new MemberRepository(dal.UnitOfWork).GetPaged(page, size);
            }

            return View(pagedResults);
        }

        // GET: MembersController/Details/5
        public ActionResult Details(int id)
        {
            return View(GetMember(id));
        }

        private Member GetMember(int id)
        {
            if (id < 1)
                throw new KeyNotFoundException();

            Member member;

            using (var dal = new DalSession(config))
            {
                member = new MemberRepository(dal.UnitOfWork).Get(id);
            }

            return member;
        }

        // GET: MembersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member model)
        {
            model.DateCreated = DateTime.Now;
            model.Status = MemberStatus.New;

            if (ModelState.IsValid)
            {

                using (var dal = new DalSession(config))
                {
                    new MemberRepository(dal.UnitOfWork).Insert(model);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: MembersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(GetMember(id));
        }

        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Member model)
        {
            var member = GetMember(id);

            member.Name = model.Name;
            member.Status = model.Status;

            if (ModelState.IsValid)
            {
                using (var dal = new DalSession(config))
                {
                    new MemberRepository(dal.UnitOfWork).Update(member);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: MembersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MembersController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

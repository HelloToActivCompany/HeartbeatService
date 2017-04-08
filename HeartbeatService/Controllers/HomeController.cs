using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HeartbeatService.Models;

namespace HeartbeatService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEndpointRepository _repository;

        public HomeController(IEndpointRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAll());
        }

        [HttpPost]
        public RedirectResult Add(Endpoint item)
        {
            if (!string.IsNullOrWhiteSpace(item?.Url))
            {
                _repository.Add(item);
            }

            return Redirect("Index");
        }

        [HttpGet]
        public RedirectResult Remove(int id)
        {
            _repository.Remove(id);

            return Redirect("/Home/Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

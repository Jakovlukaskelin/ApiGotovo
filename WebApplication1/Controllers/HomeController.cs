using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Service;
using WebApplication2.Model;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
        {
            List<SuperStore> records = await _apiService.GetAllDataAsync(page, pageSize);
            return View(records);
        }
    }
}

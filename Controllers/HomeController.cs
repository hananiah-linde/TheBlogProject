using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;
using X.PagedList;

namespace TheBlogProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBlogEmailSender _emailSender;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext context)
    {
        _logger = logger;
        _emailSender = emailSender;
        _context = context;
    }

    public async Task<IActionResult> Index(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 5;

        var blogs = _context.Blogs
            .Include(b => b.Author)
            .OrderByDescending(b => b.Created)
            .ToPagedListAsync(pageNumber, pageSize);

        ViewData["MainText"] = "Blog";
        ViewData["SubText"] = "A Blog by Hananiah Linde";

        return View(await blogs);

    }

    public IActionResult About()
    {
        ViewData["MainText"] = "Blog";
        ViewData["SubText"] = "About";
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["MainText"] = "Blog";
        ViewData["SubText"] = "Contact";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactMe model)
    {
        model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
        await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

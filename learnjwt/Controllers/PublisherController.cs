using learnjwt.AppContext;
using learnjwt.Models;
using learnjwt.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learnjwt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : Controller
{
    private readonly ILogger<BookController> _logger;

    private readonly MyDbContext _context;

    public PublisherController(ILogger<BookController> logger, MyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public IActionResult Insert([FromBody] DtoPublisherRequest publisher)
    {
        try
        {
            var userId = User.FindFirst("UserID")?.Value;
            var newPublisher = new Publisher();
            newPublisher.Name = publisher.Name;
            newPublisher.UserID = int.Parse(userId);
            _context.Publishers.Add(newPublisher);
            _context.SaveChanges();

            return Created($"api/publisher/{newPublisher.Id}", new
            {
                status = "success",
                data = newPublisher
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { status = "error", message = e.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        var publishers = _context.Publishers.Include(u => u.User).Select(p => new
        {
            Id = p.Id,
            Name = p.Name,
            AddedBy = new
            {
                Id = p.User.Id,
                Name = p.User.Name
            }
        }).OrderBy(o=>o.Id).ToList();
        return Ok(new
        {
            status = "success",
            data = publishers
        });
    }


    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        try
        {
           _context.Publishers.Remove(_context.Publishers.Where(p => p.Id == id).FirstOrDefault());
           _context.SaveChanges();
           return Ok(new {status = "success"});
        }
        catch (Exception e)
        {
            return BadRequest(new { status = "error", message = e.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        try
        {
            var publisher = _context.Publishers.Where(p => p.Id == id).FirstOrDefault();
            return Ok(new
            {
                status = "success",
                data = publisher
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { status = "error", message = e.Message });
        }
    }
}
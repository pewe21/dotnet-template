using learnjwt.AppContext;
using learnjwt.Models;
using learnjwt.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learnjwt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        
        private readonly MyDbContext _context;

        public BookController(ILogger<BookController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var books =  _context.Books.Include(u => u.User).Select(p => new
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                AddedBy = new
                {
                    Id = p.User.Id,
                    Name = p.User.Name,
                },
                    Description = p.Description,
                
            }).OrderBy(o=>o.Code).ToList();
            _logger.LogInformation("BookController diakses dengan token yang valid.");
            return Ok(new
            {
                status = "success",
                data = books
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Upsert([FromBody] DtoBookRequest book)
        {
            _logger.LogInformation("Book With Code "+ book.Code);
            var userId = User.FindFirst("UserID")?.Value;
            var bookData = _context.Books.Where(b => b.Code == book.Code).FirstOrDefault();

            if (bookData == null)
            {
                var newBook = new Book();
                newBook.Code = book.Code;
                newBook.Name = book.Title;
                newBook.Description = book.Desc;
                newBook.UserID = int.Parse(userId);
                _context.Books.Add(newBook);
                _context.SaveChanges();
                return Created($"api/book/{newBook.Id}", new
                {
                    status = "success",
                    data = newBook
                });
            }
            
            bookData.Name = book.Title;
            bookData.Code = book.Code;
            bookData.Description = book.Desc;
            bookData.UserID =  int.Parse(userId);
            _context.Books.Update(bookData);
            _context.SaveChanges();
            
            return Ok(new
            {
                status = "success",
                data = bookData
            });
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                _context.Books.Remove(_context.Books.Where(b => b.Id == id).FirstOrDefault());
                _context.SaveChanges();
                return Ok(new { status = "success" });
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
                var book = _context.Books.Where(u => u.Id == id).Include(u => u.User ).Select(p => new 
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    User = new { p.User.Id, p.User.Name },
                    Description = p.Description
                }).FirstOrDefault();
                return Ok(new { status = "success",data = book });
            }
            catch (Exception e)
            {
                return BadRequest(new { status = "error", message = e.Message });
            }
        }
    }
}

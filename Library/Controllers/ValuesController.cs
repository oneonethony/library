using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Models;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        BooksContext db;
        public ValuesController(BooksContext context)
        {
            db = context;
            if (!db.Books.Any())
            {
                db.Books.Add(new Book { Name = "Carrie", Author = "Stephen King", Year = 1974 });
                db.Books.Add(new Book { Name = "Call of cthulhu", Author = "Howard Lovecraft", Year = 1928 });
                db.Books.Add(new Book { Name = "War and Peace", Author = "Leo Tolstoy", Year = 1869 });
                db.Books.Add(new Book { Name = "Pet Sematary", Author = "Stephen King", Year = 1983 });
                db.Books.Add(new Book { Name = "1984", Author = "George Orwell", Year = 1949 });
                db.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            return await db.Books.ToListAsync();
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            Book book = await db.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
                return NotFound();
            return new ObjectResult(book);
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody]Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();
            return Ok(book);
        }

        // PUT api/users/
        [HttpPut]
        public async Task<ActionResult<Book>> Put(Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }
            if (!db.Books.Any(x => x.Id == book.Id))
            {
                return NotFound();
            }

            db.Update(book);
            await db.SaveChangesAsync();
            return Ok(book);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> Delete(int id)
        {
            Book book = db.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Ok(book);
        }
    }
}

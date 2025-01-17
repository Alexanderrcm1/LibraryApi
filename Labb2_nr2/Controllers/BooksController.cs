using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labb2_nr2.DTOs.AuthorDTOs;
using Labb2_nr2.DTOs.BookDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Labb2_nr2.Models;

namespace Labb2_nr2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _context.Books
	            .Include(b => b.Authors)
                .Select(b => new
            {
                b.BookId,
                b.Title,
                b.Isbn,
                b.ReleaseDate,
                b.Rating,
                Authors = b.Authors.Select(a => new AuthorGetDTO
                {
                    AuthorId = a.AuthorId,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList()
            }).ToListAsync();
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
	        var book = await _context.Books
		        .Include(b => b.Authors)
		        .Where(b => b.BookId == id)
		        .Select(b => new
	        {
                b.BookId,
                b.Title,
                b.Isbn,
                b.ReleaseDate,
                b.Rating,
                Authors = b.Authors.Select(a => new AuthorGetDTO
                {
                    AuthorId = a.AuthorId,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList()
	        }).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookPutDTO Dto)
        {
	        var book = await _context.Books.FindAsync(id);
            if (id != book?.BookId)
            {
                return BadRequest("Book not found.");
            }

            book.Title = Dto.Title;
            book.Isbn = Dto.Isbn;
            book.ReleaseDate = Dto.ReleaseDate;
            book.Rating = Dto.Rating;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookDTO Dto)
        {

			var authors = await _context.Authors.Where(a => Dto.AuthorIds.Contains(a.AuthorId)).ToListAsync();
			var books = await _context.Books.ToListAsync();

            if (!authors.Any())
            {
	            return NotFound();
            }
	        var book = new Book()
	        {
		        Title = Dto.Title,
		        Isbn = Dto.Isbn,
		        ReleaseDate = Dto.ReleaseDate,
		        Rating = Dto.Rating,
                Authors = authors,
	        };


			_context.Books.Add(book);
            await _context.SaveChangesAsync();

            var response = new
            {
                BookId = book.BookId,
                Title = book.Title,
                Isbn = book.Isbn,
                ReleaseDate = book.ReleaseDate,
                Rating = book.Rating,
                Authors = authors.Select(a => new AuthorGetDTO
                {
                    AuthorId = a.AuthorId,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList()
            };

            return CreatedAtAction("GetBook", new { id = book.BookId }, response);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}

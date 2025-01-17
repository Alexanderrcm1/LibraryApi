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
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

		public AuthorsController(LibraryDbContext context)
		{
			_context = context;
		}

		// GET: api/Authors
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
	        var authors = await _context.Authors
		        .Include(a => a.Books)
		        .Select(a => new
	        {
		        a.AuthorId,
		        a.FirstName,
		        a.LastName,
		        Books = a.Books.Select(b => new BookGetDTO
		        {
			        BookId = b.BookId,
			        Title = b.Title,
			        Isbn = b.Isbn,
			        Rating = b.Rating,
			        ReleaseDate = b.ReleaseDate
		        }).ToList()

	        }).ToListAsync();

	        return Ok(authors);
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
	        var author = await _context.Authors
		        .Include(a => a.Books)
		        .Where(a => a.AuthorId == id)
		        .Select(a => new
	        {
		        a.AuthorId,
		        a.FirstName,
		        a.LastName,
		        Books = a.Books.Select(b => new BookGetDTO
		        {
			        BookId = b.BookId,
			        Title = b.Title,
			        Isbn = b.Isbn,
			        Rating = b.Rating,
			        ReleaseDate = b.ReleaseDate
		        }).ToList()
	        }).FirstOrDefaultAsync();

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDTO Dto)
        {
	        var author = await _context.Authors.FindAsync(id);
            if (id != author?.AuthorId)
            {
                return BadRequest("Author not found.");
            }

            author.FirstName = Dto.FirstName;
            author.LastName = Dto.LastName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(AuthorDTO Dto)
        {
	        var author = new Author()
	        {
		        FirstName = Dto.FirstName,
		        LastName = Dto.LastName
	        };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(a => a.AuthorId == id);
        }
    }
}

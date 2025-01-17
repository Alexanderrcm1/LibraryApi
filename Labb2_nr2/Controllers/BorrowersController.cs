using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labb2_nr2.DTOs.BorrowerDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Labb2_nr2.Models;

namespace Labb2_nr2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BorrowersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/Borrowers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Borrower>>> GetBorrowers()
        {
            return await _context.Borrowers.ToListAsync();
        }

        // GET: api/Borrowers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Borrower>> GetBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);

            if (borrower == null)
            {
                return NotFound();
            }

            return borrower;
        }

        // PUT: api/Borrowers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrower(int id, BorrowerDTO Dto)
        {
	        var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null ||id != borrower.BorrowerId)
            {
                return BadRequest("Borrower not found.");
            }

            borrower.FirstName = Dto.FirstName;
            borrower.LastName = Dto.LastName;
            borrower.GotLoanCard = Dto.GotLoanCard;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowerExists(id))
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

        // POST: api/Borrowers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Borrower>> PostBorrower(BorrowerDTO Dto)
        {
	        var borrower = new Borrower()
	        {
		        FirstName = Dto.FirstName,
		        LastName = Dto.LastName,
		        GotLoanCard = Dto.GotLoanCard
	        };
            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBorrower", new { id = borrower.BorrowerId }, borrower);
        }

        // DELETE: api/Borrowers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowerExists(int id)
        {
            return _context.Borrowers.Any(e => e.BorrowerId == id);
        }
    }
}

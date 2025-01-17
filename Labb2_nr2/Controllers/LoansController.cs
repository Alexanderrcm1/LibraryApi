using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labb2_nr2.DTOs.BookDTOs;
using Labb2_nr2.DTOs.BorrowerDTOs;
using Labb2_nr2.DTOs.LoanDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Labb2_nr2.Models;

namespace Labb2_nr2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            return await _context.Loans
	            .Include(l => l.Borrower)
	            .Include(l => l.Book)
	            .ToListAsync();

        }

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
	        var loan = await _context.Loans
		        .Include(l => l.Book)
		        .Include(l => l.Borrower)
		        .FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null)
            {
                return NotFound();
            }

            return loan;
        }

        // PUT: api/Loans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, LoanReturnDTO Dto)
        {
	        var loan = await _context.Loans.FindAsync(id);
            if (loan == null || id != loan.LoanId)
            {
                return BadRequest("Loan not found.");
            }

            loan.LoanDate = Dto.LoanDate;
            loan.ReturnedDate = Dto.ReturnedDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
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

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(LoanDTO Dto)
        {
	        var borrower = await _context.Borrowers
		        .Where(b => b.BorrowerId == Dto.BorrowerId)
		        .FirstOrDefaultAsync();

	        if (borrower == null || borrower.GotLoanCard == false)
	        {
		        return BadRequest("Borrower got no loan card.");
	        }

			var book = await _context.Books
				.Where(b => b.BookId == Dto.BookId)
				.FirstOrDefaultAsync();

			if (book == null)
			{
				return BadRequest("Book does not exist.");
			}

			var existingLoan = await _context.Loans
				.Where(l => l.BookId == Dto.BookId && l.BorrowerId == Dto.BorrowerId && l.ReturnedDate == null)
				.FirstOrDefaultAsync();

			if (existingLoan != null)
			{
				return BadRequest("Borrower already loaning this book.");
			}

			var loan = new Loan()
	        {
		        BookId = Dto.BookId,
		        BorrowerId = Dto.BorrowerId,
		        LoanDate = Dto.LoanDate
	        };
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.LoanId }, loan);
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}

namespace Labb2_nr2.Models;

public class Loan
{
	public int LoanId { get; set; }
	public int BookId { get; set; }
	public int BorrowerId { get; set; }
	public required DateOnly LoanDate { get; set; }
	public DateOnly? ReturnedDate { get; set; }

	public Book? Book { get; set; }
	public Borrower? Borrower { get; set; }
}
namespace Labb2_nr2.DTOs.LoanDTOs;

public class LoanDTO
{
	public int BookId { get; set; }
	public int BorrowerId { get; set; }
	public required DateOnly LoanDate { get; set; }
}
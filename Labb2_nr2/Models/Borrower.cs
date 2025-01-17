using System.ComponentModel.DataAnnotations.Schema;

namespace Labb2_nr2.Models;

public class Borrower
{
	public int BorrowerId { get; set; }
	[Column(TypeName = "varchar(20)")]
	public required string FirstName { get; set; }
	[Column(TypeName = "varchar(20)")]
	public required string LastName { get; set; }
	public required bool GotLoanCard { get; set; }
}
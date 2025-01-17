using System.ComponentModel.DataAnnotations.Schema;

namespace Labb2_nr2.Models;

public class Author
{
	public int AuthorId { get; set; }
	[Column(TypeName = "varchar(20)")]
	public required string FirstName { get; set; }
	[Column(TypeName = "varchar(20)")]
	public required string LastName { get; set; }

	public List<Book> Books { get; set; } = new List<Book>();
}
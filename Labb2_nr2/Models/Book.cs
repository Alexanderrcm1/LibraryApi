using System.ComponentModel.DataAnnotations.Schema;

namespace Labb2_nr2.Models;

public class Book
{
	public int BookId { get; set; }
	[Column(TypeName = "varchar(50)")]
	public required string Title { get; set; }
	[Column(TypeName = "varchar(50)")]
	public required string Isbn { get; set; }
	public required DateOnly ReleaseDate { get; set; }
	public int Rating { get; set; }

	public List<Author> Authors { get; set; } = new List<Author>();
}
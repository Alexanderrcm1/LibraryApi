namespace Labb2_nr2.DTOs.BookDTOs;

public class BookDTO
{
	public required string Title { get; set; }
	public required string Isbn { get; set; }
	public required DateOnly ReleaseDate { get; set; }
	public int Rating { get; set; }

	public List<int> AuthorIds { get; set; } = new List<int>();
}
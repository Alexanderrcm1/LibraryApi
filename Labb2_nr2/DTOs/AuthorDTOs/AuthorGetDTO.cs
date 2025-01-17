namespace Labb2_nr2.DTOs.AuthorDTOs;

public class AuthorGetDTO
{
	public int AuthorId { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
}
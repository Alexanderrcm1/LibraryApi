﻿namespace Labb2_nr2.DTOs.BookDTOs;

public class BookGetDTO
{
	public int BookId { get; set; }
	public required string Title { get; set; }
	public required string Isbn { get; set; }
	public required DateOnly ReleaseDate { get; set; }
	public int Rating { get; set; }

}
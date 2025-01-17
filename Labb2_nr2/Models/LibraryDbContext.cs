using Microsoft.EntityFrameworkCore;

namespace Labb2_nr2.Models;

public class LibraryDbContext : DbContext
{
	public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
	{
		
	}

	public DbSet<Author> Authors { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Borrower> Borrowers { get; set; }
	public DbSet<Loan> Loans { get; set; }

}
using Microsoft.EntityFrameworkCore;
using MyNotes.Models; // Adjust namespace

public class AppDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}
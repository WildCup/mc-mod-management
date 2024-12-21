using McHelper.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	public DbSet<Mod> Mods { get; set; }
	public DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	   => optionsBuilder.UseNpgsql("Host=localhost; Port=5432; Database=mchelper; User Id=postgres; Password=instanceS16x@;");
}

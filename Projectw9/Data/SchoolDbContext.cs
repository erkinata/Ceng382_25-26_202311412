
using Microsoft.EntityFrameworkCore;
using Projectw9.Models;
namespace Projectw9.Data

{
 public class SchoolDbContext : DbContext
 {
 public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
 : base(options)
 {
 }
 public DbSet<Class> Classes { get; set; }
 }
}

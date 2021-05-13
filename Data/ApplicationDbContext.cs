using Estudos_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Estudos_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Produto> Produtos { get; set; }
    }
}
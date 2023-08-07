using Microsoft.EntityFrameworkCore;

namespace BullWeb.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
        
    }
}
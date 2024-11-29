using Microsoft.EntityFrameworkCore;

namespace PracticeFinal.WebAPI.Models
{
    public class UserRequestContext : DbContext
    {
        public DbSet<UserRequest> UserRequests { get; set; }
        public UserRequestContext(DbContextOptions<UserRequestContext> options) : base(options)
        {
        }
    }
}

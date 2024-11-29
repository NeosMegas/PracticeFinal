using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PracticeFinal.WebAPI.Models.SiteItems;

namespace PracticeFinal.WebAPI.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }
        public DbSet<MusketeerService> MusketeerServices { get; set; }
        public DbSet<MusketeerProject> MusketeerProjects { get; set; }
        public DbSet<MusketeerBlogItem> MusketeerBlogItems { get; set; }
        public DbSet<SiteInfo> SiteInfos { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

    }
}

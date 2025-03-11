using learnjwt.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace learnjwt.AppContext;

public class MyDbContext: DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options): base(options){}
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Book> Books { get; set; }
    
    public DbSet<Publisher> Publishers { get; set; }
}
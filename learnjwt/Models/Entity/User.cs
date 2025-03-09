using System.ComponentModel.DataAnnotations;

namespace learnjwt.Models.Entity;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required,StringLength(10)]
    public string Username { get; set; }

    [Required, StringLength(20)]
    public string Password { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    public string Address { get; set; }

    public ICollection<Book> Books { get; set; } = new HashSet<Book>();
}
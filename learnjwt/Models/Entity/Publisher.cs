using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace learnjwt.Models.Entity;
[Index(nameof(Name), IsUnique = true)]
public class Publisher
{
    [Key]
    public int Id { get; set; }

    [Required,StringLength(50)]
    public string Name { get; set; }
    
    [ForeignKey("User")]
    public int UserID { get; set; }
    public User User { get; set; }
}
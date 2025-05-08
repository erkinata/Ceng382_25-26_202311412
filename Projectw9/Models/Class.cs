using System.ComponentModel.DataAnnotations;
namespace Projectw9.Models

{
 public class Class
 {
    
 [Key]
 public int Id { get; set; }
 [Required]

 public string ClassName { get; set; }
 [Required]
 public int StudentCount { get; set; }
 public string Description { get; set; }
 [Required]
 public bool IsActive { get; set; } = true;
 }
}
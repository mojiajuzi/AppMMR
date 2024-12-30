using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppMMR.Models;
public class BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; } = DateTime.UtcNow;

    public bool Validate(out List<ValidationResult> results)
    {
        var context = new ValidationContext(this);
        results = [];
        return Validator.TryValidateObject(this, context, results, true);
    }
}
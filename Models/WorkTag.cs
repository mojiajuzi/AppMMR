using AppMMR.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppMMR.Models;

public class WorkTagModel
{
    public int WorkId { get; set; }
    public virtual WorkModel Work { get; set; }
    public int TagId { get; set; }
    public virtual TagModel Tag { get; set; }
    [Required] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required] public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
using AppMMR.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppMMR.Models;

public class ContactTagModel
{
    public int ContactId { get; set; }
    public virtual ContactModel Contact { get; set; }
    public int TagId { get; set; }
    public virtual TagModel Tag { get; set; }
    [Required] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required] public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
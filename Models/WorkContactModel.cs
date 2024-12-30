using System;
using System.ComponentModel.DataAnnotations;

namespace AppMMR.Models;

public class WorkContactModel
{
    public int WorkId { get; set; }
    public virtual WorkModel Work { get; set; }
    public int ContactId { get; set; }
    public virtual ContactModel Contact { get; set; }

    [DisplayFormat(DataFormatString = "{0:c2}", ApplyFormatInEditMode = true)]
    public decimal Amount { get; set; }  // 金额

    public bool IsCome { get; set; } = false;  // 是否来访，默认为false

    [Required]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required]
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
}
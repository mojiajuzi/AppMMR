using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppMMR.Models;

[Table("Contacts")]
[Index(nameof(Phone), IsUnique = true)]
public class ContactModel : BaseModel
{
    [Required(ErrorMessage = "联系人姓名不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "联系人姓名应在2-50字符之间")]
    public string Name { get; set; }

    [RegularExpression(@"\d{11}$", ErrorMessage = "电话号码格式不对")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "邮箱地址不对")]
    public string? Email { get; set; }

    public string? Wechat { get; set; }
    public bool Active { get; set; } = true;
    public virtual TagModel Tags { get; set; }

    public ICollection<ContactTagModel> ContactTags { get; set; } = new List<ContactTagModel>();
    public virtual ICollection<WorkContactModel> WorkContacts { get; set; } = new List<WorkContactModel>();
    public virtual ICollection<WorkPaymentModel> WorkPayments { get; set; } = new List<WorkPaymentModel>();
}
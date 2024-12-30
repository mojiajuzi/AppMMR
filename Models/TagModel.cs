using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMMR.Models;

[Table("tags")]
[Index(nameof(Name), IsUnique = true)]
public class TagModel : BaseModel
{
    [Required(ErrorMessage = "标签名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "名称长度在2-50字符之间")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "请设置标签状态")] public bool Active { get; set; } = true;

    public virtual ICollection<ContactTagModel> ContactTags { get; set; } = new List<ContactTagModel>();
    public virtual ICollection<WorkTagModel> WorkTags { get; set; } = new List<WorkTagModel>();
}
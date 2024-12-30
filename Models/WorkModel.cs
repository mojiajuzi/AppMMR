using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AppMMR.Models;
using AppMMR.Models.Enums;

namespace AppMMR.Models;

public class WorkModel : BaseModel
{
    [Required(ErrorMessage = "项目名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "项目名称长度需在2-50字符之间")]
    public string Name { get; set; }

    [MaxLength(250, ErrorMessage = "项目描述不能超过250字符")]
    public string Description { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
    public DateTime StartAt { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
    public DateTime EndAt { get; set; }

    [Required(ErrorMessage = "请输入项目金额")]
    [DisplayFormat(DataFormatString = "{0:c2}", ApplyFormatInEditMode = true)]
    public decimal Funds { get; set; }

    public WorkStatusEnum Status { get; set; } = WorkStatusEnum.PreStart;

    public virtual ICollection<WorkTagModel> WorkTags { get; set; } = new List<WorkTagModel>();
    public virtual ICollection<WorkContactModel> WorkContacts { get; set; } = new List<WorkContactModel>();
    public virtual ICollection<WorkPaymentModel> WorkPayments { get; set; } = new List<WorkPaymentModel>();

    [NotMapped]
    public decimal TotalIncome => WorkPayments?.Where(p => p.IsIncome).Sum(p => p.Amount) ?? 0;

    [NotMapped]
    public decimal TotalExpense => WorkPayments?.Where(p => !p.IsIncome).Sum(p => p.Amount) ?? 0;

    [NotMapped]
    public decimal Balance => TotalIncome - TotalExpense;
}
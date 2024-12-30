using AppMMR.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppMMR.Models;

public class WorkPaymentModel : BaseModel
{
    [Required]
    public int WorkId { get; set; }
    public virtual WorkModel Work { get; set; }

    public int? ContactId { get; set; }
    public virtual ContactModel Contact { get; set; }

    [Required(ErrorMessage = "请输入金额")]
    [Range(0.01, double.MaxValue, ErrorMessage = "金额必须大于0")]
    public decimal Amount { get; set; }

    [Required]
    public bool IsIncome { get; set; }  // true表示收入，false表示支出

    public bool HasInvoice { get; set; } = false;  // 是否开具发票

    [MaxLength(250, ErrorMessage = "备注不能超过250字符")]
    public string Remark { get; set; } = string.Empty;

    [Required]
    public DateTime PaymentDate { get; set; }
}
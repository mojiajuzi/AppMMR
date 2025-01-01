using System.ComponentModel;

namespace AppMMR.Models.Enums
{
    public enum WorkStatusEnum
    {
        [Description("未开始")]
        PreStart = 0,

        [Description("进行中")]
        InProgress = 1,

        [Description("已完成")]
        Completed = 2,

        [Description("已取消")]
        Cancelled = 3
    }
}
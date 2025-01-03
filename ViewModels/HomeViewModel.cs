using AppMMR.Entities;
using AppMMR.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace AppMMR.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly AppDbContext _dbContext;

    [ObservableProperty]
    private int totalWorkCount;

    [ObservableProperty]
    private int preStartCount;

    [ObservableProperty]
    private int inProgressCount;

    [ObservableProperty]
    private int completedCount;

    [ObservableProperty]
    private int cancelledCount;

    [ObservableProperty]
    private decimal totalIncome;

    [ObservableProperty]
    private decimal totalExpense;

    [ObservableProperty]
    private decimal balance;

    public HomeViewModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadStatistics();
    }

    public void LoadStatistics()
    {
        try
        {
            var works = _dbContext.Works.AsNoTracking().ToList();

            TotalWorkCount = works.Count;
            PreStartCount = works.Count(w => w.Status == WorkStatusEnum.PreStart);
            InProgressCount = works.Count(w => w.Status == WorkStatusEnum.InProgress);
            CompletedCount = works.Count(w => w.Status == WorkStatusEnum.Completed);
            CancelledCount = works.Count(w => w.Status == WorkStatusEnum.Cancelled);

            var payments = _dbContext.WorkPayments.AsNoTracking().ToList();
            TotalIncome = payments.Where(p => p.IsIncome).Sum(p => p.Amount);
            TotalExpense = payments.Where(p => !p.IsIncome).Sum(p => p.Amount);
            Balance = TotalIncome - TotalExpense;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载统计数据失败: {ex.Message}");
        }
    }
}

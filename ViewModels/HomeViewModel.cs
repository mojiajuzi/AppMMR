using AppMMR.Entities;
using AppMMR.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using AppMMR.Pages;
using System.Collections.ObjectModel;
using System.Linq;
using AppMMR.Models;

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

    [ObservableProperty]
    private ObservableCollection<WorkModel> recentWorks;

    public HomeViewModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        RecentWorks = new ObservableCollection<WorkModel>();
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

            // 加载最近的项目
            var recent = _dbContext.Works
                .AsNoTracking()
                .OrderByDescending(w => w.DateCreated)
                .Take(3)
                .ToList();

            RecentWorks.Clear();
            foreach (var work in recent)
            {
                RecentWorks.Add(work);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载统计数据失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NavigateToWorkList(WorkStatusEnum? status = null)
    {
        try
        {
            // 先切换到项目 Tab
            await Shell.Current.GoToAsync("//Work");

            // 获取 WorkPage 的 ViewModel 并设置筛选状态
            if (status.HasValue && Shell.Current.CurrentPage is WorkPage workPage)
            {
                var viewModel = workPage.BindingContext as WorkViewModel;
                if (viewModel != null)
                {
                    viewModel.FilterStatus = status.Value;
                    viewModel.LoadWorks();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ViewWorkDetail(WorkModel work)
    {
        if (work == null) return;

        var parameters = new Dictionary<string, object>
        {
            { "WorkId", work.Id }
        };
        await Shell.Current.GoToAsync("WorkDetail", parameters);
    }
}

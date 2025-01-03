using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMMR.ViewModels
{
    [QueryProperty(nameof(WorkId), "WorkId")]
    public partial class WorkDetailViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private WorkModel workData;

        [ObservableProperty]
        private int workId;

        public WorkDetailViewModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        partial void OnWorkIdChanged(int value)
        {
            LoadWork(value);
        }

        public void LoadWork(int workId)
        {
            try
            {
                var work = _dbContext.Works
                    .AsNoTracking()
                    .Include(w => w.WorkTags)
                        .ThenInclude(wt => wt.Tag)
                    .Include(w => w.WorkPayments)
                    .FirstOrDefault(w => w.Id == workId);

                if (work != null)
                {
                    // 计算财务数据
                    work.TotalIncome = work.WorkPayments?.Where(p => p.IsIncome).Sum(p => p.Amount) ?? 0;
                    work.TotalExpense = work.WorkPayments?.Where(p => !p.IsIncome).Sum(p => p.Amount) ?? 0;
                    work.Balance = work.TotalIncome - work.TotalExpense;

                    WorkData = work;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载项目失败: {ex.Message}");
            }
        }
    }
}

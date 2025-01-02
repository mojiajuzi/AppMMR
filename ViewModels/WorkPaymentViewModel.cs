using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkPaymentViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<WorkPaymentModel> workPayments;

        [ObservableProperty]
        private decimal totalIncome;

        [ObservableProperty]
        private decimal totalExpense;

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private int workId;

        public WorkPaymentViewModel(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            WorkPayments = new ObservableCollection<WorkPaymentModel>();
        }

        public void LoadWorkPayments(int workId)
        {
            try
            {
                WorkId = workId;
                var payments = _dbContext.WorkPayments
                    .AsNoTracking()
                    .Where(wp => wp.WorkId == workId)
                    .OrderByDescending(wp => wp.PaymentDate)
                    .ToList();

                WorkPayments = new ObservableCollection<WorkPaymentModel>(payments);

                // 计算统计数据
                TotalIncome = payments.Where(p => p.IsIncome).Sum(p => p.Amount);
                TotalExpense = payments.Where(p => !p.IsIncome).Sum(p => p.Amount);
                Balance = TotalIncome - TotalExpense;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载收支记录失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddPayment()
        {
            // 添加收支记录的实现
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppMMR.Entities;
using AppMMR.Models;
using AppMMR.Pages;
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
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        [ObservableProperty]
        private ObservableCollection<WorkPaymentModel> workPayments;

        [ObservableProperty]
        private int workId;

        [ObservableProperty]
        private decimal totalIncome;

        [ObservableProperty]
        private decimal totalExpense;

        [ObservableProperty]
        private decimal balance;

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
                    .Include(wp => wp.Contact)
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
            try
            {
                var page = _serviceProvider.GetRequiredService<WorkPaymentFormPage>();
                var viewModel = page.BindingContext as WorkPaymentFormViewModel;
                if (viewModel != null)
                {
                    viewModel.WorkId = this.WorkId;
                }

                await Navigation.PushModalAsync(page);

                // 订阅页面消失事件以刷新数据
                page.Disappearing += (s, e) =>
                {
                    LoadWorkPayments(WorkId);
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "导航失败，请重试", "确定");
            }
        }

        [RelayCommand]
        private async Task EditPayment(WorkPaymentModel payment)
        {
            try
            {
                // 先加载完整的支付记录（包括联系人信息）
                var fullPayment = await _dbContext.WorkPayments
                    .AsNoTracking()
                    .Include(wp => wp.Contact)
                    .FirstOrDefaultAsync(wp => wp.Id == payment.Id);

                if (fullPayment == null)
                {
                    await Application.Current.MainPage.DisplayAlert("错误", "找不到支付记录", "确定");
                    return;
                }

                var page = _serviceProvider.GetRequiredService<WorkPaymentFormPage>();
                var viewModel = page.BindingContext as WorkPaymentFormViewModel;
                if (viewModel != null)
                {
                    viewModel.WorkId = this.WorkId;
                    viewModel.IsEdit = true;
                    viewModel.IsIncome = fullPayment.IsIncome;
                    viewModel.Amount = fullPayment.Amount;
                    viewModel.HasInvoice = fullPayment.HasInvoice;
                    viewModel.Remark = fullPayment.Remark;
                    viewModel.PaymentDate = fullPayment.PaymentDate;
                    viewModel.EditingPayment = fullPayment;  // 移到最后设置，这样会触发联系人加载
                }

                await Navigation.PushModalAsync(page);

                // 订阅页面消失事件以刷新数据
                page.Disappearing += (s, e) =>
                {
                    LoadWorkPayments(WorkId);
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "导航失败，请重试", "确定");
            }
        }
    }
}

using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkContactViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<WorkContactModel> workContacts;

        [ObservableProperty]
        private int workId;

        public WorkContactViewModel(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            WorkContacts = new ObservableCollection<WorkContactModel>();
        }

        public void LoadWorkContacts(int workId)
        {
            try
            {
                WorkId = workId;
                var contacts = _dbContext.WorkContacts
                    .AsNoTracking()
                    .Include(wc => wc.Contact)
                    .Where(wc => wc.WorkId == workId)
                    .OrderBy(wc => wc.Contact.Name)
                    .ToList();

                WorkContacts = new ObservableCollection<WorkContactModel>(contacts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载联系人失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddContact()
        {
            // 添加联系人的实现
        }

        [RelayCommand]
        private async Task MakePhoneCall(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return;

            try
            {
                PhoneDialer.Open(phoneNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"拨打电话失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SendEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            try
            {
                var message = new EmailMessage
                {
                    To = new List<string> { email }
                };
                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"发送邮件失败: {ex.Message}");
            }
        }
    }
}

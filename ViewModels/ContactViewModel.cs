using AppMMR.Entities;
using AppMMR.Models;
using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace AppMMR.ViewModels
{
    public partial class ContactViewModel : ViewModelBase
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<ContactModel> contacts;

        [ObservableProperty]
        private ObservableCollection<ContactModel> searchResults = [];

        [ObservableProperty]
        private string? searchText = string.Empty;

        public ContactViewModel(IServiceProvider serviceProvider, AppDbContext appDbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = appDbContext;
            LoadContacts();
        }

        public void LoadContacts()
        {
            try
            {
                var contactList = _dbContext.Contacts
                    .AsNoTracking()
                    .Include(c => c.ContactTags)
                        .ThenInclude(ct => ct.Tag)
                    .OrderBy(c => c.Name)
                    .ToList();

                Contacts = new ObservableCollection<ContactModel>(contactList);
                SearchResults = Contacts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载联系人失败: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            PerformSearch(value);
        }

        [RelayCommand]
        private void PerformSearch(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    SearchResults = Contacts;
                    return;
                }

                var results = _dbContext.Contacts
                    .AsNoTracking()
                    .Include(c => c.ContactTags)
                        .ThenInclude(ct => ct.Tag)
                    .Where(c => EF.Functions.Like(c.Name, $"%{query}%") ||
                               EF.Functions.Like(c.Phone ?? "", $"%{query}%") ||
                               EF.Functions.Like(c.Email ?? "", $"%{query}%") ||
                               EF.Functions.Like(c.Wechat ?? "", $"%{query}%"))
                    .OrderBy(c => c.Name)
                    .ToList();

                SearchResults = new ObservableCollection<ContactModel>(results);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"搜索失败: {ex.Message}");
                SearchResults = Contacts;
            }
        }

        [RelayCommand]
        private async Task AddContact()
        {
            try
            {
                await Navigation.PushModalAsync(_serviceProvider.GetRequiredService<ContactFormPage>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task UpdateContact(ContactModel contact)
        {
            if (contact == null) return;

            try
            {
                var page = _serviceProvider.GetRequiredService<ContactFormPage>();
                var viewModel = page.BindingContext as ContactFormViewModel;
                if (viewModel != null)
                {
                    viewModel.ContactData = contact;
                }
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        public void Refresh()
        {
            LoadContacts();
        }

        [RelayCommand]
        private async Task MakePhoneCall(string phoneNumber)
        {
            try
            {
                if (PhoneDialer.Default.IsSupported)
                {
                    var result = await Application.Current.MainPage.DisplayAlert(
                        "确认", 
                        $"是否拨打电话 {phoneNumber}？", 
                        "确定", 
                        "取消");

                    if (result)
                    {
                        phoneNumber = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());
                        PhoneDialer.Default.Open(phoneNumber);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "提示", 
                        "当前设备不支持拨打电话", 
                        "确定");
                }
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"拨打电话失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "错误", 
                    "当前设备不支持拨打电话", 
                    "确定");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"拨打电话失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "错误", 
                    "无法拨打电话，请确保已授予权限", 
                    "确定");
            }
        }

        [RelayCommand]
        private async Task SendEmail(string email)
        {
            try
            {
                if (Email.Default.IsComposeSupported)
                {
                    var result = await Application.Current.MainPage.DisplayAlert(
                        "确认", 
                        $"是否发送邮件至 {email}？", 
                        "确定", 
                        "取消");

                    if (result)
                    {
                        var message = new EmailMessage
                        {
                            Subject = "联系人邮件",
                            Body = string.Empty,
                            BodyFormat = EmailBodyFormat.PlainText,  // Android 建议使用 PlainText
                            To = new List<string> { email }
                        };

                        await Email.Default.ComposeAsync(message);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "提示", 
                        "当前设备不支持发送邮件", 
                        "确定");
                }
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"发送邮件失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "错误", 
                    "当前设备不支持发送邮件", 
                    "确定");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"发送邮件失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "错误", 
                    "无法发送邮件，请确保已设置邮件账户", 
                    "确定");
            }
        }
    }
}

using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkContactFormViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        [ObservableProperty]
        private ObservableCollection<ContactModel> availableContacts;

        [ObservableProperty]
        private ContactModel selectedContact;

        [ObservableProperty]
        private bool isCome = true;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private int workId;

        public WorkContactFormViewModel(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            LoadContacts();
        }

        private void LoadContacts()
        {
            try
            {
                var contacts = _dbContext.Contacts
                    .AsNoTracking()
                    .Where(c => c.Active)
                    .OrderBy(c => c.Name)
                    .ToList();

                AvailableContacts = new ObservableCollection<ContactModel>(contacts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载联系人失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (SelectedContact == null)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "请选择联系人", "确定");
                    return;
                }

                // 检查是否已存在
                var exists = await _dbContext.WorkContacts
                    .AnyAsync(wc => wc.WorkId == WorkId && wc.ContactId == SelectedContact.Id);

                if (exists)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "该联系人已添加到项目中", "确定");
                    return;
                }

                // 创建新的 WorkContact，不要设置导航属性
                var workContact = new WorkContactModel
                {
                    WorkId = WorkId,
                    ContactId = SelectedContact.Id,
                    IsCome = IsCome,
                    Amount = Amount,
                    CreateTime = DateTime.Now,
                    DateModified = DateTime.Now
                };

                // 添加并保存
                _dbContext.WorkContacts.Add(workContact);
                await _dbContext.SaveChangesAsync();
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "保存失败，请重试", "确定");
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Navigation.PopModalAsync();
        }
    }
}

using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkPaymentFormViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        [ObservableProperty]
        private ObservableCollection<ContactModel> availableContacts;

        [ObservableProperty]
        private ContactModel selectedContact;

        [ObservableProperty]
        private bool isIncome = true;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private bool hasInvoice;

        [ObservableProperty]
        private string remark = string.Empty;

        [ObservableProperty]
        private DateTime paymentDate = DateTime.Now;

        [ObservableProperty]
        private int workId;

        [ObservableProperty]
        private bool isEdit;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedContact))]
        private WorkPaymentModel editingPayment;

        partial void OnEditingPaymentChanged(WorkPaymentModel value)
        {
            if (value != null)
            {
                LoadContacts(value.ContactId);
            }
        }

        public WorkPaymentFormViewModel(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            LoadContacts();
        }

        private async void LoadContacts(int? selectedContactId = null)
        {
            try
            {
                var contacts = await _dbContext.Contacts
                    .AsNoTracking()
                    .Where(c => c.Active)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                AvailableContacts = new ObservableCollection<ContactModel>(contacts);

                if (selectedContactId.HasValue)
                {
                    SelectedContact = contacts.FirstOrDefault(c => c.Id == selectedContactId);
                }
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
                if (Amount <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "请输入正确的金额", "确定");
                    return;
                }

                if (IsEdit)
                {
                    // 更新现有记录
                    if (EditingPayment != null)
                    {
                        // 从数据库获取最新的实体
                        var payment = await _dbContext.WorkPayments.FindAsync(EditingPayment.Id);
                        if (payment != null)
                        {
                            payment.IsIncome = IsIncome;
                            payment.Amount = Amount;
                            payment.HasInvoice = HasInvoice;
                            payment.Remark = Remark;
                            payment.PaymentDate = PaymentDate;
                            payment.ContactId = SelectedContact?.Id;
                            payment.DateModified = DateTime.Now;

                            _dbContext.WorkPayments.Update(payment);
                        }
                    }
                }
                else
                {
                    // 创建新记录
                    var payment = new WorkPaymentModel
                    {
                        WorkId = WorkId,
                        IsIncome = IsIncome,
                        Amount = Amount,
                        HasInvoice = HasInvoice,
                        Remark = Remark,
                        PaymentDate = PaymentDate,
                        ContactId = SelectedContact?.Id,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    };

                    _dbContext.WorkPayments.Add(payment);
                }

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

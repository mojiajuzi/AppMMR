using AppMMR.Models;
using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TabItemViewModel> tabItems;

        private readonly IServiceProvider _serviceProvider;

        public WorkTabViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            var detailPage = _serviceProvider.GetRequiredService<WorkDetailPage>();
            var contactPage = _serviceProvider.GetRequiredService<WorkContactPage>();
            var paymentPage = _serviceProvider.GetRequiredService<WorkPaymentPage>();

            TabItems = new ObservableCollection<TabItemViewModel>
            {
                new TabItemViewModel
                {
                    Title = "项目详情",
                    Icon = "info.png",
                    PageContent = detailPage.Content
                },
                new TabItemViewModel
                {
                    Title = "相关联系人",
                    Icon = "contacts.png",
                    PageContent = contactPage.Content
                },
                new TabItemViewModel
                {
                    Title = "收支记录",
                    Icon = "payment.png",
                    PageContent = paymentPage.Content
                }
            };
        }

        public void LoadWork(WorkModel work)
        {
            var detailPage = _serviceProvider.GetRequiredService<WorkDetailPage>();
            var contactPage = _serviceProvider.GetRequiredService<WorkContactPage>();
            var paymentPage = _serviceProvider.GetRequiredService<WorkPaymentPage>();

            // 设置工作数据
            (detailPage.BindingContext as WorkDetailViewModel)?.LoadWork(work.Id);
            (contactPage.BindingContext as WorkContactViewModel)?.LoadWorkContacts(work.Id);
            (paymentPage.BindingContext as WorkPaymentViewModel)?.LoadWorkPayments(work.Id);

            // 更新选项卡
            InitializeTabs();
        }
    }
}

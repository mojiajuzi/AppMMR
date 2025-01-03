using AppMMR.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AppMMR.Pages;

public partial class WorkTabPage : TabbedPage
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkTabViewModel _viewModel;

    public WorkTabPage(WorkTabViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _serviceProvider = serviceProvider;

        InitializeTabs();
    }

    private void InitializeTabs()
    {
        var detailPage = _serviceProvider.GetRequiredService<WorkDetailPage>();
        var contactPage = _serviceProvider.GetRequiredService<WorkContactPage>();
        var paymentPage = _serviceProvider.GetRequiredService<WorkPaymentPage>();

        detailPage.BindingContext = _viewModel.DetailViewModel;
        contactPage.BindingContext = _viewModel.ContactViewModel;
        paymentPage.BindingContext = _viewModel.PaymentViewModel;

        Children.Add(new NavigationPage(detailPage)
        {
            Title = "项目详情",
            IconImageSource = "circle_info.svg"
        });

        Children.Add(new NavigationPage(contactPage)
        {
            Title = "相关联系人",
            IconImageSource = "address_book.svg"
        });

        Children.Add(new NavigationPage(paymentPage)
        {
            Title = "收支记录",
            IconImageSource = "credit_card.svg"
        });
    }
}
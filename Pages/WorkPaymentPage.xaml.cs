using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkPaymentPage : ContentPage
{
    public WorkPaymentPage(WorkPaymentViewModel workPaymentViewModel)
    {
        InitializeComponent();
        BindingContext = workPaymentViewModel;
    }
}
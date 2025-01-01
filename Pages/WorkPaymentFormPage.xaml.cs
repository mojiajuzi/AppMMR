using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkPaymentFormPage : ContentPage
{
    public WorkPaymentFormPage(WorkPaymentFormViewModel workPaymentFormViewModel)
    {
        InitializeComponent();
        BindingContext = workPaymentFormViewModel;
    }
}
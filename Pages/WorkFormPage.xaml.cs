using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkFormPage : ContentPage
{
    public WorkFormPage(WorkFormViewModel workFormViewModel)
    {
        InitializeComponent();
        BindingContext = workFormViewModel;
    }
}
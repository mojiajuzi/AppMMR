using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkContactPage : ContentPage
{
    public WorkContactPage(WorkContactViewModel workContactViewModel)
    {
        InitializeComponent();
        BindingContext = workContactViewModel;
    }
}
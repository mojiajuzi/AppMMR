using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkContactFormPage : ContentPage
{
    public WorkContactFormPage(WorkContactFormViewModel workContactFormViewModel)
    {
        InitializeComponent();
        BindingContext = workContactFormViewModel;
    }
}
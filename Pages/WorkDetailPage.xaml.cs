using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkDetailPage : ContentPage
{
    public WorkDetailPage(WorkDetailViewModel workDetailViewModel)
    {
        InitializeComponent();
        BindingContext = workDetailViewModel;
    }
}
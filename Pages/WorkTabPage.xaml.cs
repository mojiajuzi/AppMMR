using AppMMR.ViewModels;


namespace AppMMR.Pages;

public partial class WorkTabPage : TabbedPage
{


    public WorkTabPage(WorkTabViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

    }
}
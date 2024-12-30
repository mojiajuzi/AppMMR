using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class TagFormPage : ContentPage
{
    public TagFormPage(TagFormViewModel tagFormViewModel)
    {
        InitializeComponent();
        BindingContext = tagFormViewModel;
    }
}
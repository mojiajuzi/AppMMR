using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class ContactPage : ContentPage
{
    private readonly ContactViewModel _contactViewModel;
    public ContactPage(ContactViewModel contactViewModel)
    {
        InitializeComponent();
        BindingContext = contactViewModel;
        _contactViewModel = contactViewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _contactViewModel.LoadContacts();
    }
}
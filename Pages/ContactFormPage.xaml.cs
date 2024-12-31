using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class ContactFormPage : ContentPage
{
	public ContactFormPage(ContactFormViewModel contactFormViewModel)
	{
		InitializeComponent();
		BindingContext = contactFormViewModel;
	}
}
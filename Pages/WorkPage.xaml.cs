using AppMMR.ViewModels;

namespace AppMMR.Pages;

public partial class WorkPage : ContentPage
{
	private readonly WorkViewModel _workViewModel;
	
	public WorkPage(WorkViewModel workViewModel)
	{
		InitializeComponent();
		BindingContext = workViewModel;
		_workViewModel = workViewModel;
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		_workViewModel.LoadWorks();
	}
}
using AppMMR.ViewModels;

namespace AppMMR.Pages
{
    public partial class TagPage : ContentPage
    {

        public TagPage(TagViewModel tagViewModel)
        {
            InitializeComponent();
            BindingContext = tagViewModel;
        }
    }
}

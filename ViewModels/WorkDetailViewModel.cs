using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMMR.ViewModels
{
    public partial class WorkDetailViewModel : ViewModelBase
    {
        [ObservableProperty] private string workName = "workDetailPage";

        [RelayCommand]
        private void EditWork()
        {

        }

        [RelayCommand]
        private void DeleteWork()
        {
        }
    }
}

using AppMMR.Models;
using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private WorkDetailViewModel detailViewModel;

        [ObservableProperty]
        private WorkContactViewModel contactViewModel;

        [ObservableProperty]
        private WorkPaymentViewModel paymentViewModel;

        public WorkTabViewModel(
            WorkDetailViewModel detailViewModel,
            WorkContactViewModel contactViewModel,
            WorkPaymentViewModel paymentViewModel)
        {
            DetailViewModel = detailViewModel;
            ContactViewModel = contactViewModel;
            PaymentViewModel = paymentViewModel;
        }

        public void LoadWork(WorkModel work)
        {
            if (work == null) return;

            DetailViewModel.LoadWork(work.Id);
            ContactViewModel.LoadWorkContacts(work.Id);
            PaymentViewModel.LoadWorkPayments(work.Id);
        }
    }
}

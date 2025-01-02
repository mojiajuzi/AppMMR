using AppMMR.Entities;
using AppMMR.Models;
using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkViewModel : ViewModelBase
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<WorkModel> works;

        [ObservableProperty]
        private ObservableCollection<WorkModel> searchResults = [];

        [ObservableProperty]
        private string? searchText = string.Empty;

        public WorkViewModel(IServiceProvider serviceProvider, AppDbContext appDbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = appDbContext;
            LoadWorks();
        }

        public void LoadWorks()
        {
            try
            {
                var workList = _dbContext.Works
                    .AsNoTracking()
                    .Include(w => w.WorkTags)
                        .ThenInclude(wt => wt.Tag)
                    .Include(w => w.WorkContacts)
                        .ThenInclude(wc => wc.Contact)
                    .OrderByDescending(w => w.StartAt)
                    .ToList();

                Works = new ObservableCollection<WorkModel>(workList);
                SearchResults = Works;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载项目失败: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            PerformSearch(value);
        }

        [RelayCommand]
        private void PerformSearch(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    SearchResults = Works;
                    return;
                }

                var results = _dbContext.Works
                    .AsNoTracking()
                    .Include(w => w.WorkTags)
                        .ThenInclude(wt => wt.Tag)
                    .Include(w => w.WorkContacts)
                        .ThenInclude(wc => wc.Contact)
                    .Where(w => EF.Functions.Like(w.Name, $"%{query}%") ||
                               EF.Functions.Like(w.Description ?? "", $"%{query}%"))
                    .OrderByDescending(w => w.StartAt)
                    .ToList();

                SearchResults = new ObservableCollection<WorkModel>(results);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"搜索失败: {ex.Message}");
                SearchResults = Works;
            }
        }

        [RelayCommand]
        private async Task AddWork()
        {
            try
            {
                await Navigation.PushModalAsync(_serviceProvider.GetRequiredService<WorkFormPage>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task UpdateWork(WorkModel work)
        {
            if (work == null) return;

            try
            {
                var page = _serviceProvider.GetRequiredService<WorkFormPage>();
                var viewModel = page.BindingContext as WorkFormViewModel;
                if (viewModel != null)
                {
                    viewModel.WorkData = work;
                }
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        public void Refresh()
        {
            LoadWorks();
        }

        [RelayCommand]
        private async Task ViewWork()
        {
            await Navigation.PushModalAsync(_serviceProvider.GetRequiredService<WorkTabPage>());
        }


    }
}

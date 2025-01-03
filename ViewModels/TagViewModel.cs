using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using AppMMR.Entities;
using Microsoft.EntityFrameworkCore;
using AppMMR.Models;
using System.Diagnostics;

namespace AppMMR.ViewModels
{
    public partial class TagViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _dbContext;
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        [ObservableProperty]
        private ObservableCollection<TagModel> tags = new();

        [ObservableProperty]
        private ObservableCollection<TagModel> searchResults = new();

        [ObservableProperty]
        private string? searchText = string.Empty;

        public TagViewModel(IServiceProvider serviceProvider, AppDbContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
            LoadTags();
        }

        public void LoadTags()
        {
            try
            {
                var tagList = _dbContext.Tags
                    .AsNoTracking()
                    .OrderByDescending(t => t.DateModified)
                    .ToList();

                Tags.Clear();
                foreach (var tag in tagList)
                {
                    Tags.Add(tag);
                }
                SearchResults = new ObservableCollection<TagModel>(Tags);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载标签失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddTag()
        {
            try
            {
                var page = _serviceProvider.GetRequiredService<TagFormPage>();
                page.Disappearing += TagFormPage_Disappearing;
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        private void TagFormPage_Disappearing(object sender, EventArgs e)
        {
            if (sender is TagFormPage page)
            {
                page.Disappearing -= TagFormPage_Disappearing;
                LoadTags();
            }
        }

        [RelayCommand]
        private async Task UpdateTag(TagModel tag)
        {
            if (tag == null) return;

            try
            {
                var page = _serviceProvider.GetRequiredService<TagFormPage>();
                page.Disappearing += TagFormPage_Disappearing;
                var viewModel = page.BindingContext as TagFormViewModel;
                if (viewModel != null)
                {
                    viewModel.TagData = tag;
                }
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"导航失败: {ex.Message}");
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
                    SearchResults = new ObservableCollection<TagModel>(Tags);
                    return;
                }

                var results = Tags.Where(t =>
                    t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                SearchResults = new ObservableCollection<TagModel>(results);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"搜索失败: {ex.Message}");
                SearchResults = Tags;
            }
        }
    }
}

using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using AppMMR.Entities;
using Microsoft.EntityFrameworkCore;
using AppMMR.Models;

namespace AppMMR.ViewModels
{
    public partial class TagViewModel : ViewModelBase
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        private readonly IServiceProvider _serviceProvider;

        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<TagModel> tags;

        [ObservableProperty]
        private ObservableCollection<TagModel> searchResults = [];

        [ObservableProperty]
        private string? searchText = string.Empty;

        public TagViewModel(IServiceProvider serviceProvider, AppDbContext appDbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = appDbContext;
            LoadTags();
        }

        private void LoadTags()
        {
            try
            {
                var tagList = _dbContext.Tags.AsNoTracking().ToList();
                Tags = new ObservableCollection<TagModel>(tagList);
                SearchResults = Tags;
            }
            catch (Exception ex)
            {
                // 可以添加错误处理逻辑
                System.Diagnostics.Debug.WriteLine($"加载标签失败: {ex.Message}");
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
                    SearchResults = Tags;
                    return;
                }

                // 方案1：使用 EF.Functions.Like
                var results = _dbContext.Tags
                    .AsNoTracking()
                    .Where(t => EF.Functions.Like(t.Name, $"%{query}%"))
                    .ToList();

                // 或者方案2：先获取数据再在内存中过滤
                //var results = Tags.Where(t => 
                //    t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                //    .ToList();

                SearchResults = new ObservableCollection<TagModel>(results);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"搜索失败: {ex.Message}");
                // 发生错误时显示所有标签
                SearchResults = Tags;
            }
        }

        [RelayCommand]
        private async Task AddTag()
        {
            try
            {
                await Navigation.PushModalAsync(_serviceProvider.GetRequiredService<TagFormPage>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task UpdateTag(TagModel tag)
        {
            if (tag == null) return;

            try
            {
                var page = _serviceProvider.GetRequiredService<TagFormPage>();
                var viewModel = page.BindingContext as TagFormViewModel;
                if (viewModel != null)
                {
                    viewModel.TagData = tag;
                }
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
                // 可以添加用户提示
            }
        }
    }
}

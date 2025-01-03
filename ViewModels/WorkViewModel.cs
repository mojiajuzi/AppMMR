﻿using AppMMR.Entities;
using AppMMR.Models;
using AppMMR.Models.Enums;
using AppMMR.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AppMMR.ViewModels
{
    public partial class WorkViewModel : ViewModelBase, IQueryAttributable
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<WorkModel> works = new();

        [ObservableProperty]
        private ObservableCollection<WorkModel> searchResults = new();

        [ObservableProperty]
        private string? searchText = string.Empty;

        [ObservableProperty]
        private WorkStatusEnum? filterStatus;

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
                if (Works == null)
                {
                    Works = new ObservableCollection<WorkModel>();
                }

                var query = _dbContext.Works.AsNoTracking();

                if (FilterStatus.HasValue)
                {
                    query = query.Where(w => w.Status == FilterStatus.Value);
                }

                var works = query
                    .Include(w => w.WorkTags)
                    .ThenInclude(wt => wt.Tag)
                    .OrderByDescending(w => w.DateModified)
                    .ToList();

                Works.Clear();
                foreach (var work in works)
                {
                    Works.Add(work);
                }

                SearchResults = new ObservableCollection<WorkModel>(Works);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载项目列表失败: {ex.Message}");
                Works ??= new ObservableCollection<WorkModel>();
                SearchResults ??= new ObservableCollection<WorkModel>();
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
                    .OrderByDescending(w => w.DateModified)
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
        private async Task ViewWork(WorkModel work)
        {
            if (work == null) return;

            try
            {
                var page = _serviceProvider.GetRequiredService<WorkTabPage>();
                var viewModel = page.BindingContext as WorkTabViewModel;
                viewModel?.LoadWork(work);
                await Navigation.PushModalAsync(page);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("FilterStatus", out var status))
            {
                FilterStatus = (WorkStatusEnum)status;
                LoadWorks(); // 重新加载并筛选数据
            }
        }
    }
}

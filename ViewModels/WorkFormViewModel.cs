using AppMMR.Entities;
using AppMMR.Models;
using AppMMR.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppMMR.ViewModels
{
    public partial class WorkFormViewModel : ViewModelBase
    {
        [ObservableProperty]
        private WorkModel workData = new WorkModel
        {
            StartAt = DateTime.Today,
            EndAt = DateTime.Today.AddMonths(1)
        };

        [ObservableProperty]
        private ObservableCollection<TagModel> availableTags = [];

        [ObservableProperty]
        private ObservableCollection<TagModel> selectedTags = [];

        [ObservableProperty]
        private string? tagSearchText;

        [ObservableProperty]
        private ObservableCollection<WorkStatusEnum> statusList = [];

        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        private readonly AppDbContext _dbContext;

        public WorkFormViewModel(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
            InitializeStatusList();
            LoadTags();
        }

        private void InitializeStatusList()
        {
            StatusList = new ObservableCollection<WorkStatusEnum>(Enum.GetValues<WorkStatusEnum>());
        }

        partial void OnWorkDataChanged(WorkModel value)
        {
            if (value != null)
            {
                LoadTags();
            }
        }

        private void LoadTags()
        {
            try
            {
                var tags = _dbContext.Tags
                    .AsNoTracking()
                    .Where(t => t.Active)
                    .OrderBy(t => t.Name)
                    .ToList();

                AvailableTags = new ObservableCollection<TagModel>(tags);

                if (WorkData.Id > 0)
                {
                    var selectedTagIds = _dbContext.WorkTags
                        .AsNoTracking()
                        .Where(wt => wt.WorkId == WorkData.Id)
                        .Select(wt => wt.TagId)
                        .ToList();

                    SelectedTags = new ObservableCollection<TagModel>(
                        tags.Where(t => selectedTagIds.Contains(t.Id)));
                }
                else
                {
                    SelectedTags = new ObservableCollection<TagModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载标签失败: {ex.Message}");
            }
        }

        partial void OnTagSearchTextChanged(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LoadTags();
                return;
            }

            var filteredTags = _dbContext.Tags
                .AsNoTracking()
                .Where(t => t.Active && EF.Functions.Like(t.Name, $"%{value}%"))
                .OrderBy(t => t.Name)
                .ToList();

            AvailableTags = new ObservableCollection<TagModel>(filteredTags);
        }

        [RelayCommand]
        private void ToggleTag(TagModel tag)
        {
            if (tag == null) return;

            if (SelectedTags.Any(t => t.Id == tag.Id))
            {
                SelectedTags.Remove(tag);
            }
            else
            {
                SelectedTags.Add(tag);
            }
        }

        [RelayCommand]
        private async Task SaveWork()
        {
            try
            {
                // 验证数据
                if (!WorkData.Validate(out var result))
                {
                    var errorMessage = string.Join(Environment.NewLine, result.Select(r => r.ErrorMessage));
                    await Application.Current.MainPage.DisplayAlert("提示", errorMessage, "确定");
                    return;
                }

                // 验证起止时间
                if (WorkData.EndAt < WorkData.StartAt)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "结束时间不能早于开始时间", "确定");
                    return;
                }

                // 更新或新增
                if (WorkData.Id > 0)
                {
                    // 更新项目
                    var entity = await _dbContext.Works
                        .Include(w => w.WorkTags)
                        .FirstOrDefaultAsync(w => w.Id == WorkData.Id);

                    if (entity != null)
                    {
                        entity.Name = WorkData.Name;
                        entity.Description = WorkData.Description;
                        entity.StartAt = WorkData.StartAt;
                        entity.EndAt = WorkData.EndAt;
                        entity.Funds = WorkData.Funds;
                        entity.Status = WorkData.Status;
                        entity.DateModified = DateTime.Now;

                        // 更新标签关联
                        entity.WorkTags.Clear();
                        foreach (var tag in SelectedTags)
                        {
                            entity.WorkTags.Add(new WorkTagModel
                            {
                                WorkId = entity.Id,
                                TagId = tag.Id,
                                CreateTime = DateTime.Now,
                                DateModified = DateTime.Now
                            });
                        }

                        _dbContext.Works.Update(entity);
                    }
                }
                else
                {
                    // 新增项目
                    WorkData.DateCreated = DateTime.Now;
                    WorkData.DateModified = DateTime.Now;
                    await _dbContext.Works.AddAsync(WorkData);
                    await _dbContext.SaveChangesAsync(); // 先保存以获取 ID

                    // 添加标签关联
                    foreach (var tag in SelectedTags)
                    {
                        await _dbContext.WorkTags.AddAsync(new WorkTagModel
                        {
                            WorkId = WorkData.Id,
                            TagId = tag.Id,
                            CreateTime = DateTime.Now,
                            DateModified = DateTime.Now
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存项目失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "保存失败，请重试", "确定");
            }
        }
    }
}

using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace AppMMR.ViewModels
{
    public partial class TagFormViewModel : ViewModelBase
    {
        [ObservableProperty] private TagModel tagData = new TagModel();

        private INavigation Navigation => Application.Current?.MainPage?.Navigation;

        private readonly AppDbContext _dbContext;
        public TagFormViewModel(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        [RelayCommand]
        private async Task SaveTag()
        {
            try
            {
                // 验证数据
                if (!TagData.Validate(out var result))
                {
                    var errorMessage = string.Join(Environment.NewLine, result.Select(r => r.ErrorMessage));
                    await Application.Current.MainPage.DisplayAlert("提示", errorMessage, "确定");
                    return;
                }

                // 检查是否存在同名标签（排除当前编辑的标签）
                var existingTag = await _dbContext.Tags
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Name == TagData.Name && t.Id != TagData.Id);

                if (existingTag != null)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "已存在相同名称的标签", "确定");
                    return;
                }

                // 更新或新增
                if (TagData.Id > 0)
                {
                    // 更新
                    var entity = await _dbContext.Tags.FindAsync(TagData.Id);
                    if (entity != null)
                    {
                        entity.Name = TagData.Name;
                        entity.Active = TagData.Active;
                        entity.DateModified = DateTime.Now;
                        _dbContext.Tags.Update(entity);
                    }
                }
                else
                {
                    // 新增
                    TagData.DateCreated = DateTime.Now;
                    TagData.DateModified = DateTime.Now;
                    await _dbContext.Tags.AddAsync(TagData);
                }

                // 保存更改
                await _dbContext.SaveChangesAsync();

                // 返回上一页
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存标签失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "保存失败，请重试", "确定");
            }
        }
    }
}

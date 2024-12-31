using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace AppMMR.ViewModels
{
    public partial class ContactFormViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ContactModel contactData = new ContactModel();

        [ObservableProperty]
        private ObservableCollection<TagModel> availableTags = [];

        [ObservableProperty]
        private ObservableCollection<TagModel> selectedTags = [];

        [ObservableProperty]
        private string? tagSearchText;

        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        private readonly AppDbContext _dbContext;

        public ContactFormViewModel(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
            LoadTags();
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

                if (ContactData.Id > 0)
                {
                    // 加载已选择的标签
                    var selectedTagIds = _dbContext.ContactTags
                        .Where(ct => ct.ContactId == ContactData.Id)
                        .Select(ct => ct.TagId)
                        .ToList();

                    SelectedTags = new ObservableCollection<TagModel>(
                        tags.Where(t => selectedTagIds.Contains(t.Id)));
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
        private async Task SaveContact()
        {
            try
            {
                // 验证数据
                if (!ContactData.Validate(out var result))
                {
                    var errorMessage = string.Join(Environment.NewLine, result.Select(r => r.ErrorMessage));
                    await Application.Current.MainPage.DisplayAlert("提示", errorMessage, "确定");
                    return;
                }

                // 检查是否存在相同手机号（排除当前编辑的联系人）
                if (!string.IsNullOrEmpty(ContactData.Phone))
                {
                    var existingContact = await _dbContext.Contacts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Phone == ContactData.Phone && c.Id != ContactData.Id);

                    if (existingContact != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("提示", "已存在相同手机号的联系人", "确定");
                        return;
                    }
                }

                // 更新或新增
                if (ContactData.Id > 0)
                {
                    // 更新联系人
                    var entity = await _dbContext.Contacts
                        .Include(c => c.ContactTags)
                        .FirstOrDefaultAsync(c => c.Id == ContactData.Id);

                    if (entity != null)
                    {
                        entity.Name = ContactData.Name;
                        entity.Phone = ContactData.Phone;
                        entity.Email = ContactData.Email;
                        entity.Wechat = ContactData.Wechat;
                        entity.Active = ContactData.Active;
                        entity.DateModified = DateTime.Now;

                        // 更新标签关联
                        entity.ContactTags.Clear();
                        foreach (var tag in SelectedTags)
                        {
                            entity.ContactTags.Add(new ContactTagModel
                            {
                                ContactId = entity.Id,
                                TagId = tag.Id,
                                CreateTime = DateTime.Now,
                                DateModified = DateTime.Now
                            });
                        }

                        _dbContext.Contacts.Update(entity);
                    }
                }
                else
                {
                    // 新增联系人
                    ContactData.DateCreated = DateTime.Now;
                    ContactData.DateModified = DateTime.Now;
                    await _dbContext.Contacts.AddAsync(ContactData);
                    await _dbContext.SaveChangesAsync(); // 先保存以获取 ID

                    // 添加标签关联
                    foreach (var tag in SelectedTags)
                    {
                        await _dbContext.ContactTags.AddAsync(new ContactTagModel
                        {
                            ContactId = ContactData.Id,
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
                System.Diagnostics.Debug.WriteLine($"保存联系人失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("错误", "保存失败，请重试", "确定");
            }
        }

        [RelayCommand]
        private async Task PickContact()
        {
            try
            {
                var contact = await Contacts.Default.PickContactAsync();
                
                if (contact == null)
                    return;

                if (contact.Phones != null && contact.Phones.Any())
                {
                    // 获取第一个手机号
                    var phone = contact.Phones.First().PhoneNumber;
                    // 清理手机号格式（移除空格、括号等）
                    phone = new string(phone.Where(c => char.IsDigit(c)).ToArray());
                    ContactData.Phone = phone;

                    // 如果联系人有姓名且当前表单姓名为空，则同时填充姓名
                    if (!string.IsNullOrEmpty(contact.DisplayName) && 
                        string.IsNullOrEmpty(ContactData.Name))
                    {
                        ContactData.Name = contact.DisplayName;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "提示", 
                        "所选联系人没有电话号码", 
                        "确定");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"选择联系人失败: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "错误", 
                    "无法访问通讯录，请确保已授予权限", 
                    "确定");
            }
        }
    }
}

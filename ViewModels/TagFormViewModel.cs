using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _dbContext.Tags.Add(TagData);
            _dbContext.SaveChanges();
            // 保存标签数据
            await Navigation.PopModalAsync();
        }
    }
}

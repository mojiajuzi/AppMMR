using AppMMR.Entities;
using AppMMR.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMMR.ViewModels
{
    public partial class WorkDetailViewModel : ObservableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private WorkModel workData;

        public WorkDetailViewModel(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public void LoadWork(int workId)
        {
            try
            {
                var work = _dbContext.Works
                    .AsNoTracking()
                    .Include(w => w.WorkTags)
                        .ThenInclude(wt => wt.Tag)
                    .FirstOrDefault(w => w.Id == workId);

                if (work != null)
                {
                    WorkData = work;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载项目失败: {ex.Message}");
            }
        }
    }
}

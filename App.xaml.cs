using AppMMR.Pages;
using AppMMR.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace AppMMR
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            //MainPage = new NavigationPage(new AppShell());
            MainPage = new AppShell();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                var dbContext = _serviceProvider.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(dbContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"数据库初始化失败: {ex}");
                // 可以在这里添加用户提示或其他错误处理
            }
        }
    }
}

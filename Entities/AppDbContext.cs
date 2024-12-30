using AppMMR.Models;
using Microsoft.EntityFrameworkCore;

namespace AppMMR.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<TagModel> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 配置 TagModel 的表结构
            modelBuilder.Entity<TagModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            // 添加一些初始数据
            modelBuilder.Entity<TagModel>().HasData(
                new TagModel { Id = 1, Name = "默认标签" }
            );
        }
    }
}

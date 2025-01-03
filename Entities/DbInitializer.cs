using AppMMR.Models;
using AppMMR.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppMMR.Entities
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            try
            {
                context.Database.EnsureCreated();

                if (context.Works.Any() || context.Contacts.Any() || context.Tags.Any())
                {
                    return;
                }

                // 初始化标签
                var tags = new[]
                {
                    new TagModel
                    {
                        Name = "重要",
                        Active = true,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    },
                    new TagModel
                    {
                        Name = "紧急",
                        Active = true,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.Tags.AddRange(tags);

                // 初始化联系人
                var contacts = new[]
                {
                    new ContactModel
                    {
                        Name = "示例联系人",
                        Phone = "13800138000",
                        Email = "example@email.com",
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.Contacts.AddRange(contacts);

                // 初始化项目
                var works = new[]
                {
                    new WorkModel
                    {
                        Name = "示例项目",
                        Description = "这是一个示例项目",
                        Status = WorkStatusEnum.InProgress,
                        StartAt = DateTime.UtcNow,
                        EndAt = DateTime.UtcNow.AddMonths(1),
                        Funds = 10000M,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.Works.AddRange(works);

                // 保存基础数据
                context.SaveChanges();

                // 添加关联数据
                var workTags = new[]
                {
                    new WorkTagModel
                    {
                        WorkId = works[0].Id,
                        TagId = tags[0].Id,
                        CreateTime = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.WorkTags.AddRange(workTags);

                var workContacts = new[]
                {
                    new WorkContactModel
                    {
                        WorkId = works[0].Id,
                        ContactId = contacts[0].Id,
                        Amount = 0,
                        IsCome = false,
                        CreateTime = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.WorkContacts.AddRange(workContacts);

                var payments = new[]
                {
                    new WorkPaymentModel
                    {
                        WorkId = works[0].Id,
                        ContactId = contacts[0].Id,
                        Amount = 5000M,
                        IsIncome = true,
                        HasInvoice = false,
                        PaymentDate = DateTime.UtcNow,
                        Remark = "首付款",
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    }
                };

                context.WorkPayments.AddRange(payments);

                // 保存关联数据
                context.SaveChanges();

                Debug.WriteLine("数据库初始化成功");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"数据库初始化失败: {ex.Message}");
                throw;
            }
        }
    }
}
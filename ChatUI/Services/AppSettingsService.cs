using LocalChatApp.Data;
using LocalChatApp.Data.Enitites;
using LocalChatApp.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalChatApp.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly AppDbContext _db;

        public AppSettingsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AppSettingsItem>> GetAllAsync()
        {
           return await _db.AppSettings.ToListAsync();
        }

        public async Task SaveAsync(AppSettingsItem appSettingItem)
        {
            var item = _db.AppSettings.SingleOrDefault(x => x.Id == appSettingItem.Id);

            if (item == null)
            {
                _db.AppSettings.Add(appSettingItem);
            }
            else
            {
                item.Value = appSettingItem.Value;
            }

            await _db.SaveChangesAsync();  
        }
    }
}

using DbScriptManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Persistence.Interfaces
{
    public interface IVersionRepository
    {
        Task<List<DbVersion>> GetAllAsync();//TÜM VERSİYONLARI GETİR
        Task<DbVersion> GetByIdAsync (int  id);//TEK VERSİYON GETİR
        Task AddAsync(DbVersion version);// EKLE
        Task DeleteAsync (int id);//SİL
        

    }
}

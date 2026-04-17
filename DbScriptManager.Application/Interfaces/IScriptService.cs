using DbScriptManager.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Application.Interfaces
{
    public interface IScriptService
    {
        Task<List<ScriptDto>> GetAllScripts();
        Task<ScriptDto?> GetById(int id);
        Task CreateScript(CreateScriptDto dto);
        Task Delete(int id);
        Task<List<ScriptDto>> GetScriptsByVersion(int versionId);
        Task <ScriptDetailDto?> GetScriptDetail(int id);
        Task ExecuteScriptAsync(int id);
    }
}
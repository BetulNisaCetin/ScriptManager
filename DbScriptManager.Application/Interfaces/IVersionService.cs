using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptManager.Application.DTOs;

namespace DbScriptManager.Application.Interfaces
{
    public interface IVersionService
    {
        Task<List<VersionDto>> GetAllVersions();
        Task CreateVersion(CreateVersionDto dto);
        Task DeleteVersion(int id);
        Task DeleteVersions(List<int> ids);
        Task<VersionDetailDto> GetVersionDetail(int id);
       
    }
}

// DatabaseConfigController.cs
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbScriptManager.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DatabaseConfigController : Controller
    {
        private readonly IDatabaseConfigRepository _dbConfigRepository;

        public DatabaseConfigController(IDatabaseConfigRepository dbConfigRepository)
            => _dbConfigRepository = dbConfigRepository;

        [HttpGet]
        public async Task<IActionResult> Index()
            => View(await _dbConfigRepository.GetAllAsync());

        [HttpPost]
        [ValidateAntiForgeryToken]
       
public async Task<IActionResult> Create(string name, string description)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        TempData["ErrorMessage"] = "Database adı zorunludur";
        return RedirectToAction(nameof(Index));
    }

    try
    {
        await _dbConfigRepository.AddAsync(new DatabaseConfig
        {
            Name        = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            CreatedDate = DateTime.UtcNow
        });
        TempData["SuccessMessage"] = "Database eklendi";
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = ex.Message;
    }

    return RedirectToAction(nameof(Index));
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hasScripts = await _dbConfigRepository.HasScriptsAsync(id);
            if (hasScripts)
            {
                TempData["ErrorMessage"] = "Bu database'e ait scriptler var, silinemez";
                return RedirectToAction(nameof(Index));
            }

            await _dbConfigRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Database silindi";
            return RedirectToAction(nameof(Index));
        }
    }
}
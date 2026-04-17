using DbScriptManager.Application.DTOs;
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DbScriptManager.WebUI.Controllers;

[Authorize]
public class ScriptController : Controller
{
    private readonly IScriptService _scriptService;
    private readonly IVersionService _versionService;
    private readonly UserManager<AppUser> _userManager;

    public ScriptController(
        IScriptService scriptService,
        UserManager<AppUser> userManager,
        IVersionService versionService)
    {
        _scriptService = scriptService;
        _versionService = versionService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var scripts = await _scriptService.GetAllScripts();
        
        return View(scripts);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? versionId)
    {
        var versions = await _versionService.GetAllVersions();
        ViewBag.Versions = new SelectList(versions, "Id", "VersionName", versionId);

        var model = new CreateScriptDto();
        var user =await _userManager.GetUserAsync(User);
        if (user != null)
        {
            model.DeveloperName = user.FullName;
        }

        if (versionId.HasValue)
        {
            model.VersionId = versionId.Value;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateScriptDto dto)
    {
        if (!ModelState.IsValid)
        {
            var versions = await _versionService.GetAllVersions();
            ViewBag.Versions = new SelectList(versions, "Id", "VersionName", dto.VersionId);
            return View(dto);
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            dto.CreatedByUserId = user.Id;
            dto.DeveloperName = user.FullName;

            await _scriptService.CreateScript(dto);
            TempData["SuccessMessage"] = "Script başarıyla oluşturuldu";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var versions = await _versionService.GetAllVersions();
            ViewBag.Versions = new SelectList(versions, "Id", "VersionName", dto.VersionId);

            ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var script = await _scriptService.GetById(id);
        if (script == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && script.CreatedByUserId != user.Id)
            return Forbid();

        await _scriptService.Delete(id);
        TempData["SuccessMessage"] = "Script başarıyla silindi";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var script = await _scriptService.GetScriptDetail(id);
        if (script == null)
            return NotFound();

        return View(script);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Execute(int id)
    {
        var script = await _scriptService.GetById(id);
        if (script == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && script.CreatedByUserId != user.Id)
            return Forbid();

        try
        {
            await _scriptService.ExecuteScriptAsync(id);
            TempData["SuccessMessage"] = "Script başarıyla çalıştırıldı";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Detail), new { id });
    }
}
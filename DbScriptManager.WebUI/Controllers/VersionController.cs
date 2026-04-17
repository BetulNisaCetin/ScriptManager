using DbScriptManager.Application.DTOs;
using DbScriptManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbScriptManager.WebUI.Controllers
{
    [Authorize(Roles = "Admin,Developer")]
    public class VersionController : Controller
    {
        private readonly IVersionService _service;

        public VersionController(IVersionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var versions = await _service.GetAllVersions();
            return View(versions);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Developer")]
        public IActionResult Create()
        {
            return View(new CreateVersionDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVersionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _service.CreateVersion(dto);
            TempData["SuccessMessage"] = "Versiyon başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteVersion(id);
                TempData["SuccessMessage"] = "Versiyon başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<int> selectedIds)
        {
            try
            {
                if (selectedIds == null || !selectedIds.Any())
                {
                    TempData["ErrorMessage"] = "Lütfen silmek için en az bir versiyon seçin.";
                    return RedirectToAction("Index");
                }

                await _service.DeleteVersions(selectedIds);
                TempData["SuccessMessage"] = "Seçilen versiyonlar başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var model = await _service.GetVersionDetail(id);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
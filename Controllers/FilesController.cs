using KaymazLabs.AzureStorage.Data;
using KaymazLabs.AzureStorage.Models;
using KaymazLabs.AzureStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using KaymazLabs.AzureStorage.Security;

namespace KaymazLabs.AzureStorage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKey]
    public class FilesController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly AppDbContext _context;

        public FilesController(IBlobStorageService blobStorageService, AppDbContext context)
        {
            _blobStorageService = blobStorageService;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Dosya seçilmedi.");

            // Servisten iki bilgiyi de alıyoruz (tuple yakalama)
            var (uri, uniqueName) = await _blobStorageService.UploadFileAsync(file, "kaymazlabs-uploads");

            var fileRecord = new FileInfoModel
            {
                FileName = file.FileName,       // Orijinal ad (İndirirken lazım)
                StoredFileName = uniqueName,    // Azure adı (Silme/İndirme işlemlerinde lazım)
                AzureUrl = uri,                 // Tam link (Frontend'de göstermek için lazım)
                ContentType = file.ContentType
            };

            _context.Files.Add(fileRecord);
            await _context.SaveChangesAsync();

            return Ok(fileRecord);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            return Ok(await _context.Files.ToListAsync());
        }

        // --- YENİ: DOSYA İNDİRME METODU ---
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            // 1. Veritabanından kaydı bul
            var fileRecord = await _context.Files.FindAsync(id);
            if (fileRecord == null) return NotFound("Dosya kaydı bulunamadı.");

            // KRİTİK DÜZELTME: AzureUrl yerine StoredFileName gönderiyoruz
            var fileStream = await _blobStorageService.DownloadFileAsync(fileRecord.StoredFileName, "kaymazlabs-uploads");

            if (fileStream == null) return NotFound("Dosya Azure üzerinde fiziksel olarak bulunamadı.");

            // 3. Dosyayı kullanıcıya orijinal ismiyle (FileName) teslim et
            return File(fileStream, fileRecord.ContentType, fileRecord.FileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var fileRecord = await _context.Files.FindAsync(id);
            if (fileRecord == null) return NotFound();

            // 1. Önce Azure'dan fiziksel dosyayı siliyoruz
            await _blobStorageService.DeleteFileAsync(fileRecord.StoredFileName, "kaymazlabs-uploads");
    
            // 2. Sonra veritabanı kaydını siliyoruz
            _context.Files.Remove(fileRecord);
            await _context.SaveChangesAsync();

            return Ok("Dosya hem Azure'dan hem de veritabanından tamamen silindi.");
        }
        [HttpGet("test-cicd")]
        public IActionResult TestCiCd()
        {
            return Ok(new 
            { 
                Durum = "Başarılı", 
                Mesaj = "🚀 GitHub Actions Otomasyonu Çalışıyor!", 
                SunucuSaati = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            });
        }
    }
}
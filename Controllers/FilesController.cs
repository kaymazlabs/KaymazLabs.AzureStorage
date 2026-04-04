using KaymazLabs.AzureStorage.Data;
using KaymazLabs.AzureStorage.Models;
using KaymazLabs.AzureStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KaymazLabs.AzureStorage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly AppDbContext _context; // Veritabanı motorumuzu (DbContext) buraya tanımlıyoruz
        //test3
        // Constructor: Hem Azure servisini hem de Veritabanı motorunu içeri alıyoruz (Dependency Injection)
        public FilesController(IBlobStorageService blobStorageService, AppDbContext context)
        {
            _blobStorageService = blobStorageService;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Lütfen bir dosya seçin.");

            // 1. Dosyayı Azure'a gönder ve açık URL'yi al
            var fileUrl = await _blobStorageService.UploadFileAsync(file, "kaymazlabs-uploads");

            // 2. Veritabanına kaydedilecek "Kayıt Defteri" modelini oluştur
            var fileRecord = new FileInfoModel
            {
                FileName = file.FileName,
                AzureUrl = fileUrl,
                ContentType = file.ContentType
                // UploadDate yazmamıza gerek yok, modelde otomatik olarak şu anki zaman atanıyor
            };

            // 3. Oluşturduğumuz bu kaydı veritabanı tablomuza ekle ve kalıcı olarak kaydet
            _context.Files.Add(fileRecord);
            await _context.SaveChangesAsync();

            // 4. Kullanıcıya başarı mesajıyla birlikte veritabanı kaydını geri dön
            return Ok(new { Message = "Dosya Azure'a yüklendi ve veritabanına kaydedildi!", Data = fileRecord });
        }

        // YENİ ÖZELLİK: Tüm dosyaları listeleme metodu
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            // Veritabanındaki Files tablosuna git, içindeki her şeyi bir Liste (Array) olarak getir
            var files = await _context.Files.ToListAsync();
            return Ok(files);
        }
    }
}
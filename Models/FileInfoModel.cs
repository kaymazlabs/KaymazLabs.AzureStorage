using System;

namespace KaymazLabs.AzureStorage.Models
{
    public class FileInfoModel
    {
        public int Id { get; set; } // Birincil anahtar
        public string FileName { get; set; } // Orijinal dosya adı
        public string AzureUrl { get; set; } // Azure'dan gelen link
        public DateTime UploadDate { get; set; } = DateTime.Now; // Yükleme zamanı
        public string ContentType { get; set; } // image/png vb.
    }
}
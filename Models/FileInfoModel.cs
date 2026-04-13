using System;

namespace KaymazLabs.AzureStorage.Models
{
    public class FileInfoModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } // Orijinal ad: "rapor.pdf"
        public string StoredFileName { get; set; } // Azure'daki ad: "guid_rapor.pdf"
        public string AzureUrl { get; set; } // Tam erişim linki: "https://.../guid_rapor.pdf"
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
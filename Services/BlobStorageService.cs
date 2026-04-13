using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace KaymazLabs.AzureStorage.Services
{
    public interface IBlobStorageService
    {
        Task<(string uri, string uniqueName)> UploadFileAsync(IFormFile file, string containerName);
        // Yeni eklenen satır:
        Task<Stream> DownloadFileAsync(string fileName, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string containerName);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;

        // ARTIK IConfiguration'a ihtiyacımız yok, doğrudan sistemden çekiyoruz
        public BlobStorageService()
        {
            // .env dosyasından yüklediğimiz değişkeni sistemden yakala
            _connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Azure Connection String bulunamadı! .env dosyasını kontrol et.");
            }
        }

        public async Task<(string uri, string uniqueName)> UploadFileAsync(IFormFile file, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
                });
            }

            // Hem linki hem de unique ismi paketleyip geri gönderiyoruz
            return (blobClient.Uri.ToString(), uniqueFileName);
        }
        public async Task<Stream> DownloadFileAsync(string fileName, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                // Dosyayı sunucuya indirmeden, doğrudan kullanıcıya akıtıyoruz (Stream)
                return await blobClient.OpenReadAsync();
            }

            return null;
        }
        public async Task<bool> DeleteFileAsync(string fileName, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }
        
    }
}
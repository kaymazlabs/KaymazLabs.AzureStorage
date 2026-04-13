using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace KaymazLabs.AzureStorage.Security
{
    // Bu etiketin hem class'lara (Controller) hem de metodlara eklenebilmesini sağlıyoruz
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        // İsteğin başlığında (Header) arayacağımız anahtarın adı
        private const string ApiKeyHeaderName = "X-Api-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. İstek atan kişi Header içine anahtarı koymuş mu?
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Güvenlik İhlali: API Anahtarı eksik!" });
                return;
            }

            // 2. Bizim sistemimizdeki (.env içindeki) gerçek anahtarı al
            var expectedApiKey = Environment.GetEnvironmentVariable("MY_API_KEY");

            if (string.IsNullOrEmpty(expectedApiKey))
            {
                context.Result = new StatusCodeResult(500); // Sunucu hatası (şifre ayarlanmamış)
                return;
            }

            // 3. Gönderilen anahtar ile gerçek anahtar eşleşiyor mu?
            if (!expectedApiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Güvenlik İhlali: Geçersiz API Anahtarı!" });
                return;
            }

            // Her şey yolundaysa isteğin geçmesine izin ver
            await next();
        }
    }
}
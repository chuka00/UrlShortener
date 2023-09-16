using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.BLL.Interfaces;
using UrlShortener.DAL.Data;
using UrlShortener.DAL.Entities;

namespace UrlShortener.BLL.Service
{
    public class UrlService : IUrlService
    {
        private readonly AppDbContext _dbContext;
        public UrlService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string ShortenUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var inputUrl))
                return null; // Invalid URL

            // Check if the URL already exists in the database
            var existingUrl = _dbContext.Urls.FirstOrDefault(u => u.Url == url);
            if (existingUrl != null)
            {
               // return $"{yourBaseUrl}/{existingUrl.ShortUrl}"; // Return existing short URL
            }

            // Generate a random short URL
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var sUrl = new UrlManagement()
            {
                Url = url,
                ShortUrl = randomStr
            };

            _dbContext.Urls.Add(sUrl);
            _dbContext.SaveChanges();

            return sUrl.ShortUrl;

           // return $"{yourBaseUrl}/{sUrl.ShortUrl}"; // Return newly generated short URL
        }

        public string GetOriginalUrl(string shortUrl)
        {
            var urlMatch = _dbContext.Urls.FirstOrDefault(x => x.ShortUrl == shortUrl);

            if (urlMatch == null)
                return null; // Short URL not found in the database

            return urlMatch.Url; // Return the original URL
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.BLL.Interfaces
{
    public interface IUrlService
    {
        string ShortenUrl(string url);
        string GetOriginalUrl(string shortUrl);

    }
}

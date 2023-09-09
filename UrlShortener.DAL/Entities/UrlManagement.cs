using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.DAL.Entities
{
    public class UrlManagement
    {
        public int Id { get; set; }
        public string Url { get; set; } = "";
        public string ShortUrl { get; set; } = "";
    }
}

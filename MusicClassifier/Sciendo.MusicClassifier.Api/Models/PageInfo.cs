using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalNumberOfPages { get; set; }
    }
}

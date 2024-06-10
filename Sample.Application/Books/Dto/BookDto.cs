using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Application.Books.Dto
{
    public class BookDto
    {
        public long? Id { get; set; }
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
    }
}

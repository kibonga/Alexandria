using System;
using System.Collections.Generic;

namespace Alexandria.Api.Data
{
    public partial class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}

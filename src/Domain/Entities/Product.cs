using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

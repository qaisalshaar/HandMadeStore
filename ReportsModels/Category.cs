using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ReportsModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

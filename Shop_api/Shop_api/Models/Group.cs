using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_api.Models
{
    public class Group
    {
        // ид
        // название группы
        // картинка

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        // link
        public List<Product> Products { get; set; }

    }
}

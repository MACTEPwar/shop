using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_api.Models
{
    public enum MeasureList
    {
        Килограмм,
        Грамм,
        Тонна,
        Мелиметр,
        Сантиметр,
        Метр,
        Штука,
        Упаковка
    }

    public class Product
    {
        // ид
        // название
        // код
        // описание
        // количество
        // единица измерения
        // цена
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Title { get; set; }
        string Discription { get; set; }
        [Required]
        public double Count { get; set; }
        [Required]
        public MeasureList Measure { get; set; }
        [Required]
        public double Price { get; set; }

        public int GroupId { get; set; }
        // link
        public Group Group { get; set; }
    }
}

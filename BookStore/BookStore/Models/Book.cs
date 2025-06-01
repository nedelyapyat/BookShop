using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    public class Book
    {
        public int Код { get; set; }
        public string Шифр { get; set; }
        public string Тип { get; set; }
        public string Название { get; set; }
        public string Тема { get; set; }
        public string Издательство { get; set; }
        public int ГодИздания { get; set; }
        public decimal Цена { get; set; }
        public int Количество { get; set; }
    }
}

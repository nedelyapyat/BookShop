using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Классы
{
    public class Sale
    {
        public int Код { get; set; }
        public int Книга { get; set; }
        public int Читатель { get; set; }
        public DateTime ДатаПродажи { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class SaleView
    {
        public int Код { get; set; }
        public string НазваниеКниги { get; set; }
        public string ФИОЧитателя { get; set; }
        public DateTime ДатаПродажи { get; set; }
    }
}

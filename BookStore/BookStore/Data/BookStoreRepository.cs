using BookStore.Models;
using BookStore.Классы;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data
{
    internal class BookStoreRepository
    {
        private readonly string connectionString;

        public BookStoreRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Book> GetBooks()
        {
            var result = new List<Book>();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Код, Шифр, Тип, Название, Тема, Издательство, ГодИздания, Цена, Количество FROM Книги", conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Book
                    {
                        Код = reader.GetInt32(0),
                        Шифр = reader.GetString(1),
                        Тип = reader.GetString(2),
                        Название = reader.GetString(3),
                        Тема = reader.GetString(4),
                        Издательство = reader.GetString(5),
                        ГодИздания = reader.GetInt32(6),
                        Цена = reader.GetDecimal(7),
                        Количество = reader.GetInt32(8)
                    });
                }
            }
            return result;
        }

        public List<Reader> GetReaders()
        {
            var result = new List<Reader>();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Код, Читатель, ФИО, Адрес, Телефон FROM Читатели", conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Reader
                    {
                        Код = reader.GetInt32(0),
                        Читатель = reader.GetString(1),
                        ФИО = reader.GetString(2),
                        Адрес = reader.GetString(3),
                        Телефон = reader.GetString(4)
                    });
                }
            }
            return result;
        }

        // Для отображения всех продаж с названиями и ФИО
        public List<SaleView> GetSales()
        {
            var result = new List<SaleView>();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                @"SELECT Продажа.Код, Книги.Название, Читатели.ФИО, Продажа.ДатаПродажи 
              FROM Продажа
              JOIN Книги ON Продажа.Книга = Книги.Код
              JOIN Читатели ON Продажа.Читатель = Читатели.Код", conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new SaleView
                    {
                        Код = reader.GetInt32(0),
                        НазваниеКниги = reader.GetString(1),
                        ФИОЧитателя = reader.GetString(2),
                        ДатаПродажи = reader.GetDateTime(3)
                    });
                }
            }
            return result;
        }

        // Для добавления продажи
        public void AddSale(int bookId, int readerId, DateTime saleDate)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "INSERT INTO Продажа (Книга, Читатель, ДатаПродажи) VALUES (@book, @reader, @date)", conn))
            {
                cmd.Parameters.Add("@book", SqlDbType.Int).Value = bookId;
                cmd.Parameters.Add("@reader", SqlDbType.Int).Value = readerId;
                cmd.Parameters.Add("@date", SqlDbType.Date).Value = saleDate.Date;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

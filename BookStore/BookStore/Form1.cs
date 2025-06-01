using BookStore.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace BookStore
{
    public partial class Form1 : Form
    {
        private BookStoreRepository repo;
        private List<Book> books;
        private List<Reader> readers;

        public Form1()
        {
            InitializeComponent();
            // Инициализация репозитория с строкой подключения из App.config
            string connStr = ConfigurationManager.ConnectionStrings["BookStoreConnection"].ConnectionString;
            repo = new BookStoreRepository(connStr);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Загрузка книг
                books = repo.GetBooks();
                comboBox1.DataSource = books;
                comboBox1.DisplayMember = "Название";
                comboBox1.ValueMember = "Код";

                // Загрузка читателей
                readers = repo.GetReaders();
                comboBox2.DataSource = readers;
                comboBox2.DisplayMember = "ФИО";
                comboBox2.ValueMember = "Код";

                // Загрузка продаж
                LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void LoadSales()
        {
            try
            {
                dataGridView1.DataSource = repo.GetSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продаж: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
                {
                    MessageBox.Show("Пожалуйста, выберите книгу и читателя!");
                    return;
                }

                int bookId = (int)comboBox1.SelectedValue;
                int readerId = (int)comboBox2.SelectedValue;
                DateTime date = dateTimePicker1.Value.Date;
                repo.AddSale(bookId, readerId, date);
                MessageBox.Show("Продажа добавлена!");
                LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении продажи: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Можно добавить логику при изменении выбранной книги, если нужно
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Можно добавить логику при изменении выбранного читателя, если нужно
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Можно добавить логику при изменении даты, если нужно
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Можно добавить логику при клике по ячейке таблицы продаж, если нужно
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            // Можно добавить логику при переходе на вкладку 1, если нужно
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            // Можно добавить логику при переходе на вкладку 2, если нужно
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            // Можно добавить логику при переходе на вкладку 3, если нужно
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Можно добавить логику при клике по ячейке второй таблицы, если нужно
        }
    }
}

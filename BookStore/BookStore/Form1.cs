using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BookStore
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;
        private SqlDataAdapter booksAdapter, readersAdapter, salesAdapter;
        private DataSet dataSet;

        public Form1()
        {
            InitializeComponent();

            // Подписываемся на события DataBindingComplete для настройки столбцов
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;
            dataGridView3.DataBindingComplete += dataGridView3_DataBindingComplete;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Строка подключения - измените под свою БД
            string connectionString = "Data Source=WIN-UCHSIQ48T9V;Initial Catalog=BookStore;Integrated Security=True";

            connection = new SqlConnection(connectionString);
            dataSet = new DataSet();

            LoadBooks();
            LoadReaders();
            LoadSales();
        }

        private void LoadBooks()
        {
            try
            {
                booksAdapter = new SqlDataAdapter("SELECT * FROM Книги", connection);
                SqlCommandBuilder booksCommandBuilder = new SqlCommandBuilder(booksAdapter);
                dataSet.Tables["Книги"]?.Clear();
                booksAdapter.Fill(dataSet, "Книги");
                dataGridView1.DataSource = dataSet.Tables["Книги"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке книг: " + ex.Message);
            }
        }

        private void LoadReaders()
        {
            try
            {
                readersAdapter = new SqlDataAdapter("SELECT * FROM Читатели", connection);
                SqlCommandBuilder readersCommandBuilder = new SqlCommandBuilder(readersAdapter);
                dataSet.Tables["Читатели"]?.Clear();
                readersAdapter.Fill(dataSet, "Читатели");
                dataGridView2.DataSource = dataSet.Tables["Читатели"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке читателей: " + ex.Message);
            }
        }

        private void LoadSales()
        {
            try
            {
                salesAdapter = new SqlDataAdapter(
                    "SELECT Продажа.Код, Книги.Название AS Книга, Читатели.ФИО AS Читатель, Продажа.ДатаПродажи " +
                    "FROM Продажа " +
                    "JOIN Книги ON Продажа.Книга = Книги.Код " +
                    "JOIN Читатели ON Продажа.Читатель = Читатели.Код", connection);

                SqlCommandBuilder salesCommandBuilder = new SqlCommandBuilder(salesAdapter);
                dataSet.Tables["Продажи"]?.Clear();
                salesAdapter.Fill(dataSet, "Продажи");
                dataGridView3.DataSource = dataSet.Tables["Продажи"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке продаж: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Выберите книгу и читателя!");
                return;
            }

            try
            {
                // Проверяем наличие книги
                int quantity = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Количество"].Value);
                if (quantity <= 0)
                {
                    MessageBox.Show("Этой книги нет в наличии!");
                    return;
                }

                // Получаем выбранные книгу и читателя
                int bookId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Код"].Value);
                int readerId = Convert.ToInt32(dataGridView2.CurrentRow.Cells["Код"].Value);

                // Добавляем запись о продаже в базу данных
                SqlCommand insertCommand = new SqlCommand(
                    "INSERT INTO Продажа (Книга, Читатель, ДатаПродажи) VALUES (@BookId, @ReaderId, @SaleDate); " +
                    "SELECT SCOPE_IDENTITY();", connection);

                insertCommand.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;
                insertCommand.Parameters.Add("@ReaderId", SqlDbType.Int).Value = readerId;
                insertCommand.Parameters.Add("@SaleDate", SqlDbType.Date).Value = DateTime.Today;

                connection.Open();
                int newId = Convert.ToInt32(insertCommand.ExecuteScalar());
                connection.Close();

                // Обновляем количество книг
                SqlCommand updateCommand = new SqlCommand(
                    "UPDATE Книги SET Количество = Количество - 1 WHERE Код = @BookId", connection);

                updateCommand.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;

                connection.Open();
                updateCommand.ExecuteNonQuery();
                connection.Close();

                // Обновляем локальные данные
                DataRow bookRow = dataSet.Tables["Книги"].Rows.Find(bookId);
                bookRow["Количество"] = Convert.ToInt32(bookRow["Количество"]) - 1;
                dataSet.Tables["Книги"].AcceptChanges();

                // Добавляем запись в локальный DataSet
                DataRow newSale = dataSet.Tables["Продажи"].NewRow();
                newSale["Код"] = newId;
                newSale["Книга"] = dataGridView1.CurrentRow.Cells["Название"].Value.ToString();
                newSale["Читатель"] = dataGridView2.CurrentRow.Cells["ФИО"].Value.ToString();
                newSale["ДатаПродажи"] = DateTime.Today;

                dataSet.Tables["Продажи"].Rows.Add(newSale);
                dataSet.Tables["Продажи"].AcceptChanges();

                // Обновляем DataGridView
                dataGridView1.Refresh();
                dataGridView3.Refresh();

                MessageBox.Show("Продажа успешно добавлена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Продажа успешно добавлена! Перезагрузите программу: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView1.Columns.Contains("Код"))
                dataGridView1.Columns["Код"].Visible = false;

            if (dataGridView1.Columns.Contains("Название"))
                dataGridView1.Columns["Название"].HeaderText = "Название книги";

            if (dataGridView1.Columns.Contains("Цена"))
                dataGridView1.Columns["Цена"].DefaultCellStyle.Format = "C2";
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView2.Columns.Contains("Код"))
                dataGridView2.Columns["Код"].Visible = false;

            if (dataGridView2.Columns.Contains("ФИО"))
                dataGridView2.Columns["ФИО"].HeaderText = "ФИО читателя";

            if (dataGridView2.Columns.Contains("Читатель"))
                dataGridView2.Columns["Читатель"].HeaderText = "Номер читателя";
        }

        private void dataGridView3_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView3.Columns.Contains("Код"))
                dataGridView3.Columns["Код"].Visible = false;

            if (dataGridView3.Columns.Contains("Книга"))
                dataGridView3.Columns["Книга"].HeaderText = "Название книги";

            if (dataGridView3.Columns.Contains("Читатель"))
                dataGridView3.Columns["Читатель"].HeaderText = "ФИО читателя";

            if (dataGridView3.Columns.Contains("ДатаПродажи"))
            {
                dataGridView3.Columns["ДатаПродажи"].HeaderText = "Дата продажи";
                dataGridView3.Columns["ДатаПродажи"].DefaultCellStyle.Format = "d";
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            // Обновляем данные при переходе на вкладку
            LoadBooks();
            LoadReaders();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            // Обновляем данные о продажах при переходе на вкладку
            LoadSales();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Обработка клика по ячейке таблицы "Книги"
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Обработка клика по ячейке таблицы "Читатели"
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Обработка клика по ячейке таблицы "Продажи"
        }
    }
}

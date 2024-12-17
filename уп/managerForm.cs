using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace уп
{
    public partial class managerForm : Form
    {
        private SqlConnection connection;
        private DataTable requestsTable; // Таблица для хранения заявок
        private int totalRecords;
        private string connectionString = @"Server=DESKTOP-Q8GSFE6\SQLEXPRESS;Database=CarMechanic;Integrated Security=True";
        private const int InProgressStatusId = 2; // ID статуса "В процессе ремонта"

        public managerForm()
        {
            InitializeComponent();
            LoadRequests();
            LoadMechanics();
        }

        private void LoadRequests()
        {
            try
            {
                // Инициализация соединения
                connection = new SqlConnection(connectionString);
                connection.Open();

                // Запрос для получения всех заявок с присоединением к другим таблицам для отображения имен клиентов и механиков
                string query = @"
            SELECT 
            r.requestID,
            r.startDate,
            c.carID,
            r.problemDescription,
            s.statusName,
            r.complectionDate,
            u.userID AS masterID,
            u.fio AS masterName,
            uc.fio AS clientName
            FROM Requests r
            LEFT JOIN Cars c ON r.carID = c.carID
            LEFT JOIN Statuses s ON r.statusID = s.statusID
             LEFT JOIN Users u ON r.masterID = u.userID
             LEFT JOIN Users uc ON r.clientID = uc.userID";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                requestsTable = new DataTable();
                adapter.Fill(requestsTable);
                dataGridView1.DataSource = requestsTable;
                dataGridView1.Columns[0].HeaderText = "Номер заявки";
                dataGridView1.Columns[1].HeaderText = "Дата начала";
                dataGridView1.Columns[2].HeaderText = "Номер автомобиля";
                dataGridView1.Columns[3].HeaderText = "Проблема";
                dataGridView1.Columns[4].HeaderText = "Статус заявки";
                dataGridView1.Columns[5].HeaderText = "Дата завершения";
                dataGridView1.Columns[6].HeaderText = "Номер мастера";
                dataGridView1.Columns[7].HeaderText = "ФИО Мастера";
                dataGridView1.Columns[8].HeaderText = "ФИО Клиента";

                totalRecords = requestsTable.Rows.Count; // Сохраняем общее количество записей
                UpdateRecordCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявок: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void LoadMechanics()
        {
            // Загрузка всех автомехаников в ComboBox
            string query = @"SELECT userID, fio FROM Users WHERE roleID = (SELECT roleID FROM Roles WHERE roleID = 2)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt); // Заполнение DataTable данными

                comboBox1.DisplayMember = "fio";   // Отображаем ФИО автомехаников
                comboBox1.ValueMember = "userID";  // Используем userID как значение
                comboBox1.DataSource = dt;          // Устанавливаем источник данных
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Назначить выбранного автомеханика на выбранную заявку
            if (dataGridView1.SelectedRows.Count > 0 && comboBox1.SelectedValue != null)
            {
                int selectedRequestId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["requestID"].Value);
                int selectedMasterId = Convert.ToInt32(comboBox1.SelectedValue);

                AssignMechanicToRequest(selectedRequestId, selectedMasterId);
                MessageBox.Show("Механик успешно назначен на заявку!");

                // Обновляем данные заявок после изменения
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Выберите заявку и автомеханика.");
            }
        }

        private void AssignMechanicToRequest(int requestId, int mechanicId)
        {
            // SQL-запрос для назначения автомеханика на заявку
            string query = @"UPDATE Requests
                             SET masterID = @masterID
                             WHERE requestID = @requestID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@masterID", mechanicId);
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Изменить дату завершения заявки и статус на "В процессе"
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRequestId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["requestID"].Value);
                DateTime selectedDate = dateTimePicker1.Value;

                UpdateRequestComplectionDateAndStatus(selectedRequestId, selectedDate);
                MessageBox.Show("Дата завершения заявки успешно обновлена и статус изменен на 'В процессе'!");

                // Обновляем данные заявок после изменения
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Выберите заявку.");
            }
        }

        private void UpdateRequestComplectionDateAndStatus(int requestId, DateTime complectionDate)
        {
            // SQL-запрос для обновления даты завершения и статуса заявки
            string query = @"UPDATE Requests
                             SET complectionDate = @complectionDate, statusID = @statusID
                             WHERE requestID = @requestID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@complectionDate", complectionDate);
                    cmd.Parameters.AddWithValue("@statusID", InProgressStatusId); // Статус "В процессе"
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void UpdateRecordCount()
        {
            int filteredRecords = (dataGridView1.DataSource as DataTable)?.DefaultView.Count ?? 0;
            label2.Text = $"{filteredRecords} из {totalRecords}"; // Обновляем текст метки с количеством записей
        }
    }
}

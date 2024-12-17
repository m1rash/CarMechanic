﻿using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using CarMechanicLibrary;

namespace уп
{
    public partial class avtorizatia : Form
    {
        public int attemptCount = 0;
        public bool isBlocked = false;
        private System.Timers.Timer blockTimer;
        private int blockTime = 180000;
        private string captchaValue;
        private bool isPasswordVisible = false;

        public avtorizatia()
        {
            InitializeComponent();
            ServiceCalculator.CalculateDiscountedPrice(1000, 80);
            pictureBoxCaptcha.Visible = false;
            txtCaptcha.Visible = false;
            updatePictureBox.Visible = false;

            txtPassword.UseSystemPasswordChar = true;
            pictureBox1.Click += new EventHandler(TogglePasswordVisibility);

            blockTimer = new System.Timers.Timer(blockTime);
            blockTimer.Elapsed += UnblockUser;
            blockTimer.AutoReset = false;
        }

        public static class CaptchaGenerator
        {
            private static Random random = new Random();

            public static string Generate()
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                char[] captcha = new char[7];

                for (int i = 0; i < captcha.Length; i++)
                {
                    captcha[i] = chars[random.Next(chars.Length)];
                }

                return new string(captcha);
            }

            public static Bitmap RenderCaptchaImage(string captchaText, Size pictureBoxSize)
            {
                Bitmap bitmap = new Bitmap(pictureBoxSize.Width, pictureBoxSize.Height);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Gray);

                    int fontSize = Math.Min(pictureBoxSize.Width / captchaText.Length, pictureBoxSize.Height / 2);
                    Font font = new Font("Arial", fontSize, FontStyle.Bold);

                    for (int i = 0; i < captchaText.Length; i++)
                    {
                        int xOffset = i * fontSize + random.Next(-5, 5);
                        int yOffset = random.Next(0, pictureBoxSize.Height / 3);

                        PointF position = new PointF(xOffset, yOffset);
                        g.DrawString(captchaText[i].ToString(), font, Brushes.White, position);
                    }

                    for (int i = 0; i < 100; i++)
                    {
                        int x = random.Next(bitmap.Width);
                        int y = random.Next(bitmap.Height);
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                }

                return bitmap;
            }
        }
        
        public void GenerateCaptcha()
        {
            captchaValue = CaptchaGenerator.Generate();
            pictureBoxCaptcha.Image = CaptchaGenerator.RenderCaptchaImage(captchaValue, pictureBoxCaptcha.Size);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isBlocked)
            {
                MessageBox.Show("Вход заблокирован. Подождите 3 минуты.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string login = txtLogin.Text;
            string password = txtPassword.Text;

            if (attemptCount >= 1 && pictureBoxCaptcha.Visible)
            {
                if (txtCaptcha.Text != captchaValue)
                {
                    attemptCount++;
                    LogLoginAttempt(login, false);
                    MessageBox.Show("Неправильная CAPTCHA. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (attemptCount > 2)
                    {
                        BlockUser();
                        return;
                    }

                    return;
                }
            }

            if (ValidateUser(login, password))
            {
                attemptCount = 0;
                LogLoginAttempt(login, true);
            }
            else
            {
                attemptCount++;
                LogLoginAttempt(login, false);

                if (attemptCount == 1)
                {
                    MessageBox.Show("Неверный логин или пароль. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ShowCaptcha();
                }
                else if (attemptCount >= 2)
                {
                    MessageBox.Show("Неверный логин или пароль. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (attemptCount > 2)
                {
                    BlockUser();
                }
            }
        }

        public bool ValidateUser(string login, string password)
        {
            using (SqlConnection conn = new SqlConnection("Server=DESKTOP-Q8GSFE6\\SQLEXPRESS;Database=CarMechanic;Integrated Security=True"))
            {
                conn.Open();
                string query = @"
                    SELECT r.RoleName 
                    FROM Users u 
                    INNER JOIN Roles r ON u.RoleID = r.RoleID 
                    WHERE u.Login = @login AND u.Password = @password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    string role = reader.GetString(0);
                    OpenUserForm(role);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void LogLoginAttempt(string login, bool isSuccess)
        {
            using (SqlConnection conn = new SqlConnection("Server=DESKTOP-Q8GSFE6\\SQLEXPRESS;Database=CarMechanic;Integrated Security=True"))
            {
                conn.Open();
                string query = "INSERT INTO LoginHistory (Login, LoginTime, Success) VALUES (@login, @loginTime, @success)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@loginTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@success", isSuccess);
                cmd.ExecuteNonQuery();
            }
        }

        private void OpenUserForm(string role)
        {
            this.Hide();

            pictureBoxCaptcha.Visible = false;
            txtCaptcha.Visible = false;
            updatePictureBox.Visible = false;

            switch (role)
            {
                case "Заказчик":
                    int clientID = GetClientID();
                    clientForm clientForm = new clientForm(clientID);
                    clientForm.Show();
                    break;
                case "Оператор":
                    showRequestAdmin adminForm = new showRequestAdmin();
                    adminForm.Show();
                    break;
                case "Автомеханик":
                    int mechanicID = GetMechanicID();
                    requestFormMaster masterForm = new requestFormMaster(mechanicID);
                    masterForm.Show();
                    break;
                case "Менедежр":
                    managerForm managerForm = new managerForm(); 
                    managerForm.Show(); 
                    break;
                default:
                    MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                    break;
            }
        }

        private int GetClientID()
        {
            using (SqlConnection conn = new SqlConnection("Server=DESKTOP-Q8GSFE6\\SQLEXPRESS;Database=CarMechanic;Integrated Security=True"))
            {
                conn.Open();
                string query = "SELECT UserID FROM Users WHERE Login = @login";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", txtLogin.Text);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("Не удалось получить clientID");
                }
            }
        }

        private int GetMechanicID()
        {
            using (SqlConnection conn = new SqlConnection("Server=DESKTOP-Q8GSFE6\\SQLEXPRESS;Database=CarMechanic;Integrated Security=True"))
            {
                conn.Open();
                string query = "SELECT UserID FROM Users WHERE Login = @login";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", txtLogin.Text);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("Не удалось получить идентификатор автомеханика");
                }
            }
        }

        private void ShowCaptcha()
        {
            pictureBoxCaptcha.Visible = true;
            txtCaptcha.Visible = true;
            updatePictureBox.Visible = true;

            GenerateCaptcha();
        }

        private void BlockUser()
        {
            isBlocked = true;
            MessageBox.Show("Доступ заблокирован на 3 минуты из-за слишком большого количества неудачных попыток.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            blockTimer.Start();
        }

        public void UnblockUser(Object source, ElapsedEventArgs e)
        {
            isBlocked = false;
            attemptCount = 0;
            blockTimer.Stop();
        }

        public void TogglePasswordVisibility(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !isPasswordVisible;
            isPasswordVisible = !isPasswordVisible;
        }

        public void TogglePasswordVisibility()
        {
            txtPassword.UseSystemPasswordChar = !isPasswordVisible;
            isPasswordVisible = !isPasswordVisible;
        }

        private void updatePictureBox_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            historyLoginForm historyForm = new historyLoginForm();
            historyForm.Show();
        }
    }
}

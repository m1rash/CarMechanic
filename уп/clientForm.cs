using QRCoder;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace уп
{
    public partial class clientForm : Form
    {
        private int clientID; // Переменная для хранения идентификатора клиента

        // Конструктор, который принимает clientID
        public clientForm(int clientID)
        {
            InitializeComponent();
            this.clientID = clientID; // Сохраняем clientID для использования внутри формы
        }

        // Открываем форму для добавления заявки, текущая остаётся открытой
        private void button1_Click(object sender, EventArgs e)
        {
            addRequest addRequestForm = new addRequest(clientID); // Передаем clientID в форму создания заявки
            addRequestForm.Show();
        }

        // Открываем форму просмотра заявок, закрываем текущую
        private void button2_Click(object sender, EventArgs e)
        {
            showRequestClient showRequestClientForm = new showRequestClient(clientID); // Передаем clientID в форму просмотра заявок
            showRequestClientForm.Show();
            this.Close(); // Закрываем текущую форму
        }

        // Открываем форму авторизации, закрываем текущую
        private void button3_Click(object sender, EventArgs e)
        {
            avtorizatia avtorizatiaForm = new avtorizatia();
            avtorizatiaForm.Show();
            this.Close();
        }

        private void GenerateQRCode(string url)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);


            int qrSize = Math.Min(pictureBox1.Width, pictureBox1.Height); 
            Bitmap qrCodeImage = qrCode.GetGraphic(20); 


            Bitmap resizedQRCode = new Bitmap(qrCodeImage, new Size(pictureBox1.Width, pictureBox1.Height));
            pictureBox1.Image = resizedQRCode;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string url = "https://docs.google.com/forms/d/e/1FAIpQLScJWRRmSbUQdjT7-4LrYjtTCt-kIilPvcoe35lytzqLzN3EkQ/viewform?usp=header";
            GenerateQRCode(url);
        }
    }
}
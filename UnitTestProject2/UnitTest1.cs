using Microsoft.VisualStudio.TestTools.UnitTesting;
using уп;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        private avtorizatia form;

        [TestInitialize]
        public void Setup()
        {
            form = new avtorizatia();
        }

        [TestMethod]
        public void Test_InvalidPassword_ShouldFailLogin()
        {
            string login = "login1"; 
            string invalidPassword = "wrongpass";

            bool result = form.ValidateUser(login, invalidPassword);

            Assert.IsFalse(result, "Вход должен завершиться неудачей с неверным паролем.");
        }

        [TestMethod]
        public void Test_InvalidLogin_ShouldFailLogin()
        {
            string invalidLogin = "invalidLogin";
            string password = "pass1"; 

            bool result = form.ValidateUser(invalidLogin, password); 

            Assert.IsFalse(result, "Вход должен завершиться неудачей с неверным логином.");
        }

        [TestMethod]
        public void Test_ValidUser()
        {
            string login = "login1";
            string password = "pass1";

            bool result = form.ValidateUser(login, password);
            string userRole = "Менеджер";

            Assert.IsTrue(result, "Пользователь должен успешно войти.");
            Assert.AreEqual("Менеджер", userRole, "Роль должна быть 'Менеджер'.");
        }

        [TestMethod]
        public void Test_GenerateCaptcha()
        {
            form.GenerateCaptcha();

            Assert.IsNotNull(form.pictureBoxCaptcha.Image, "Изображение CAPTCHA должно быть сгенерировано и не быть нулевым после вызова GenerateCaptcha.");
        }

        [TestMethod]
        public void Test_PasswordVisibilityToggle()
        {
            bool initialVisibility = form.txtPassword.UseSystemPasswordChar;

            form.TogglePasswordVisibility();

            Assert.AreNotEqual(initialVisibility, !form.txtPassword.UseSystemPasswordChar, "Видимость пароля должна переключаться.");
        }
    }
}
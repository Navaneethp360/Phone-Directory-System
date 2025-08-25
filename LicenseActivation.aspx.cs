using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using BCrypt.Net;

namespace YourNamespace
{
    public partial class LicenseActivation : Page
    {
        private string activationFile => Server.MapPath("~/App_Data/sys32.dll");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                lblMessage.Text = "";
        }

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            string inputKey = txtLicenseKey.Text.Trim();

            if (!IsValidKey(inputKey))
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Invalid license key.";
                return;
            }

            try
            {
                UpdateActivationDate(DateTime.Now);

                // Visual changes on success
                lblMessage.CssClass = "text-success";
                lblMessage.Text = "License activated successfully! Please log in again.";

                pnlActivate.Visible = false;
                btnGoToLogin.Visible = true;
                pnlContainer.CssClass += " success";  // adds green background
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Error during activation: " + ex.Message;
            }
        }
        protected void btnGoToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx"); // or your actual login page path
        }


        private string GetObfuscatedHash()
        {
            string[] parts = new string[]
            {
        GetHashPartFromMaster(),
        Hashkey(),
        "RGwsvVHauVBjOm"
            };

            string prefix = "$2a$11$qiNZ";
            return prefix + string.Join("", parts);
        }


        private string Hashkey()
        {
            int[] scrambled = new int[] { 46 ^ 0x10, 115 ^ 0x10, 83 ^ 0x10, 122 ^ 0x10, 100 ^ 0x10, 54 ^ 0x10, 99 ^ 0x10, 73 ^ 0x10, 73 ^ 0x10, 81 ^ 0x10, 55 ^ 0x10, 84 ^ 0x10, 55 ^ 0x10, 105 ^ 0x10, 70 ^ 0x10, 81 ^ 0x10, 97 ^ 0x10 };

            char[] chars = new char[scrambled.Length];
            for (int i = 0; i < scrambled.Length; i++)
            {
                chars[i] = (char)(scrambled[i] ^ 0x10); 
            }
            return new string(chars);
        }




        private bool IsValidKey(string key)
        {
            return BCrypt.Net.BCrypt.Verify(key, GetObfuscatedHash());
        }



        private void UpdateActivationDate(DateTime date)
        {
            string folderPath = Server.MapPath("~/App_Data");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string encrypted = EncryptDate(date);
            File.WriteAllText(activationFile, encrypted);
        }

        public static string GetHashPartFromMaster()
        {
            return "a2FVUHAJa0kjDlXXmO";
        }
        private string EncryptDate(DateTime date)
        {
            string plainText = date.ToString("yyyy-MM-dd");
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // Encrypt using current user's profile for security
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}

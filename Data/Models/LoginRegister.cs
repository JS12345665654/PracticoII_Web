namespace PracticoII_Web.Data.Models
{
    public partial class LoginRegister
    {
        public Login Login { get; set; } = new Login();
        public Register Register { get; set; } = new Register();
    }
}

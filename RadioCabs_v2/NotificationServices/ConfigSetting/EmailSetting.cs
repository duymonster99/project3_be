namespace NotificationServices.ConfigSetting;

public class EmailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSSL { get; set; }
    public string FromMail { get; set; }
    public string Password { get; set; }
}
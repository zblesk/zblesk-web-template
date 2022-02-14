using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace zblesk_web.Configs;

public class Config
{
    public string Urls { get; set; }
    private string _baseUrl;
    private string _key;
    public static readonly string[] SupportedLanguages = new[] { "sk", "en", "cs" };

    public string BaseUrl { get => (_baseUrl ?? "").Trim().TrimEnd('/'); set => _baseUrl = value; }
    public string MailerType { get; set; }
    public int DefaultPasswordMinLength { get; set; }
    public string PasswordGenerationChars { get; set; }
    public string DataFolder { get; set; }
    public string DefaultCulture { get; set; }
    public CultureInfo DefaultCultureInfo => new(DefaultCulture);
    public SymmetricSecurityKey SecurityKey { get; private set; }
    public SigningCredentials SigningCreds { get; private set; }

    public string SymmetricSecurityKey
    {
        get => _key;
        set
        {
            _key = value;
            SecurityKey = new SymmetricSecurityKey(
                Encoding.Default.GetBytes(_key));
            SigningCreds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }
    }

}

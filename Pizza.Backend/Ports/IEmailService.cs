using System.Threading.Tasks;

namespace Pizza.Backend.Ports
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
    }
}

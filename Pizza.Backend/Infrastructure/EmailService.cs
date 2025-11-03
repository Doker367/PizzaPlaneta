using Microsoft.Extensions.Configuration;
using Pizza.Backend.Ports;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Pizza.Backend.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var apiKey = _configuration["SENDGRID_API_KEY"];
            var senderEmail = _configuration["SENDGRID_SENDER_EMAIL"];
            var senderName = "Pizza Planeta";

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(senderEmail))
            {
                // Si no hay configuración de SendGrid, no se intenta enviar el correo.
                // Esto evita errores en entornos de desarrollo que no tengan las claves.
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(toEmail);
            var subject = "Restablece tu contraseña de Pizza Planeta";
            
            // Aquí puedes crear un HTML más elaborado para tu correo
            var plainTextContent = $"Para restablecer tu contraseña, usa el siguiente token: {resetToken}";
            var htmlContent = $"<strong>Hola,</strong><br><p>Hemos recibido una solicitud para restablecer tu contraseña.</p>" +
                              "<p>Por favor, haz clic en el siguiente enlace para continuar:</p>" +
                              // IMPORTANTE: La URL debe apuntar a tu frontend
                              $"<a href='http://localhost:3000/reset-password?token={resetToken}&email={toEmail}'>Restablecer Contraseña</a><br>" +
                              "<p>Si no solicitaste esto, puedes ignorar este correo.</p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}

﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MotasAlcoafinal.Services
{
    /// <summary>
    /// Serviço para envio de emails
    /// </summary>
    public class EmailService
    {
        /// <summary>
        /// Envia um email assíncrono
        /// </summary>
        /// <param name="toEmail">Endereço de email do destinatário</param>
        /// <param name="subject">Assunto do email</param>
        /// <param name="htmlMessage">Conteúdo HTML do email</param>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Motas Alcoa", "mmotasalcoa@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("mmotasalcoa@gmail.com", "gshq auak spzg ixkj");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
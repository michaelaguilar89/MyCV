using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MICV.Dto;
using MICV.Models;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;


namespace MICV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:UserName").Value));
            email.To.Add(MailboxAddress.Parse(request.Para));
            email.To.Add(MailboxAddress.Parse(request.CC));
            email.Subject = request.Asunto;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Contenido
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true; // Solo para pruebas

            await smtp.ConnectAsync(
                _config.GetSection("Email:Host").Value,
               Convert.ToInt32(_config.GetSection("Email:Port").Value),
               SecureSocketOptions.Auto

                );


            await smtp.AuthenticateAsync(_config.GetSection("Email:UserName").Value, _config.GetSection("Email:PassWord").Value);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);


        }

        private string GenerateContactEmailBody(ContactDto model)
        {
            StringBuilder emailBody = new StringBuilder();

            // Encabezado del correo
            emailBody.AppendLine("<h2> MVC - PortFolio </h2>");
            emailBody.AppendLine("<p></p>");
            emailBody.AppendLine("<p>Contact Email</p>");

            // Informaci n del cliente
            emailBody.AppendLine("<br />");
            emailBody.AppendLine("<p>---------------------------</p>");
            emailBody.AppendLine("<p><strong>Client:</strong> " + model.Name + "</p>");
            emailBody.AppendLine("<p><strong>Email:</strong> " + model.Email + "</p>");
            emailBody.AppendLine("<p><strong>Address:</strong> " + model.Address + "</p>");
            emailBody.AppendLine("<p><strong>Phone Number:</strong> " + model.PhoneNumber + "</p>");
            emailBody.AppendLine("<p><strong>Messages:</strong> " + model.Message + "</p>");
            emailBody.AppendLine("<p>---------------------------</p>");

            return emailBody.ToString();
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                string emailBody = GenerateContactEmailBody(dto);
                EmailDto email = new EmailDto();
                email.Para = "aguilarartaviamichael@gmail.com";
                email.CC= dto.Email;
                email.Asunto = "Contact Form - MVC PortFolio -Michael Aguilar -by : " + dto.Name;
                email.Contenido = emailBody;
                await SendEmail(email);


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {

                Console.WriteLine("Error : " + e.Message);
                return View(dto);
            }

        }
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

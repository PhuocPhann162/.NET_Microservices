using FucoMicro.Services.EmailAPI.Data;
using FucoMicro.Services.EmailAPI.Models;
using FucoMicro.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FucoMicro.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;

        public EmailService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<!DOCTYPE html>");
            message.AppendLine("<html>");
            message.AppendLine("<head>");
            message.AppendLine("<style>");
            message.AppendLine("table {font-family: Arial, sans-serif; border-collapse: collapse; width: 100%;}");
            message.AppendLine("td, th {border: 1px solid #dddddd; text-align: left; padding: 8px;}");
            message.AppendLine("tr:nth-child(even) {background-color: #f2f2f2;}");
            message.AppendLine("</style>");
            message.AppendLine("</head>");
            message.AppendLine("<body>");

            message.AppendLine("<h2>Đơn hàng được tạo bởi Phước Phan</h2>");
            message.AppendLine($"<p>Ngày tạo: {DateTime.Now}</p>");
            message.AppendLine("<table>");
            message.AppendLine("<tr><th>Sản phẩm</th><th>Số lượng</th><th>Giá</th><th>Tổng</th></tr>");

            foreach (var item in cartDto.CartDetails)
            {
                message.AppendLine("<tr>");
                message.AppendLine($"<td>{item.Product.Name}</td>");
                message.AppendLine($"<td>{item.Product.Count}</td>");
                message.AppendLine($"<td>{item.Product.Price:C}</td>");
                message.AppendLine($"<td>{item.Product.Count * item.Product.Price:C}</td>");
                message.AppendLine("</tr>");
            }

            message.AppendLine("<tr>");
            message.AppendLine("<td colspan='3'><strong>Tổng cộng</strong></td>");
            message.AppendLine($"<td><strong>{cartDto.CartHeader.CartTotal:C}</strong></td>");
            message.AppendLine("</tr>");
            message.AppendLine("</table>");

            message.AppendLine("<p>Cảm ơn bạn đã mua hàng!</p>");

            message.AppendLine("</body>");
            message.AppendLine("</html>");


            // Send email (implementation depends on your email service)
            // For example: await _emailService.SendEmailAsync(cartDto.CustomerEmail, "Đơn hàng của bạn", message.ToString());

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

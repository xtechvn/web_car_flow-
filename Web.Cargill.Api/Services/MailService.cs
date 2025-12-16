using System.Net.Mail;
using System.Net;
using Web.Cargill.Api.Model;
using Utilities;
using Entities.ViewModels.Car;

namespace Web.Cargill.Api.Services
{
    public class MailService
    {
        private IConfiguration configuration;
        public MailService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task<bool> SendDailyVehicleReportMail(
    TotalVehicleInspection s,
    List<TotalWeightByHourModel> byWeightGroup,
    List<TotalWeightByHourModel> byTrough,
    DateTime reportDate)
        {
            try
            {
                var config = configuration.GetSection("MAIL_CONFIG");

                var minutePerTon = CalcMinutePerTon(s);
                var avgTonPerTrip = s.TotalCarCompleted > 0
                    ? Math.Round(s.TotalWeightTroughType / s.TotalCarCompleted, 2)
                    : 0;

                var message = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = $"Báo cáo xuất hàng ngày {reportDate:dd/MM/yyyy} – Nhà máy Hà Nam",
                    From = new MailAddress(config["FROM_MAIL"])
                };

                message.Body = $@"
<html>
<body style='font-family:Arial;font-size:14px'>
<b>Báo cáo xuất hàng ngày {reportDate:dd/MM/yyyy} – Nhà máy Hà Nam</b><br/>
Kính gửi: Ban Lãnh đạo Công ty<br/>
Đơn vị báo cáo: Hệ thống vận hành xuất hàng<br/>
Thời điểm chốt số liệu: {DateTime.Now:HH:mm:ss} – ngày {reportDate:dd/MM/yyyy}

<h3>I. CHỈ TIÊU CHÍNH</h3>
<table border='1' cellpadding='6' cellspacing='0' width='100%'>
<tr><td>Tổng xe đăng ký</td><td>{s.TotalCar} xe</td></tr>
<tr><td>Số chuyến đã xuất</td><td>{s.TotalCarCompleted} chuyến</td></tr>
<tr><td>Tổng khối lượng đã xuất</td><td>{s.TotalWeightTroughType} tấn</td></tr>
<tr><td>Tổng thời gian làm việc</td><td>{s.TotalTimeWorkInHour} giờ</td></tr>
<tr><td><b>Năng suất trung bình</b></td><td><b>{s.AverageProductivity} tấn/giờ</b></td></tr>
<tr><td>Trung bình khối lượng / chuyến</td><td>{avgTonPerTrip} tấn</td></tr>
<tr><td>Thời gian trung bình / chuyến</td><td>{Math.Round(s.AvgTimePerCompletedCar_Hour * 60, 2)} phút</td></tr>
<tr><td>Phút / tấn (bình quân)</td><td>{minutePerTon} phút</td></tr>
<tr><td>Xe đến lấy sau 16h</td><td>{s.TotalCarArriving16h} xe</td></tr>
</table>

<h3>III. HIỆU SUẤT THEO NHÓM XE</h3>
<table border='1' cellpadding='6' cellspacing='0' width='100%'>
<tr>
<th>Nhóm xe</th><th>Sản lượng (tấn)</th><th>Phút / tấn</th><th>Phút / xe</th>
</tr>
{BuildWeightGroupRows(byWeightGroup)}
</table>

<h3>IV. HIỆU SUẤT THEO MÁNG XUẤT</h3>
<table border='1' cellpadding='6' cellspacing='0' width='100%'>
<tr>
<th>Máng</th><th>Sản lượng (tấn)</th><th>Thời gian (giờ)</th><th>Năng suất (tấn/giờ)</th>
</tr>
{BuildTroughRows(byTrough)}
</table>

<p><b>Nhận định & Đề xuất:</b></p>
<ul>
<li>Máng có năng suất cao nhất cần ưu tiên khai thác.</li>
<li>Theo dõi máng hiệu suất thấp để điều chỉnh nhân lực/luồng xe.</li>
</ul>

Trân trọng,<br/>
<b>Hệ thống Báo cáo Vận hành</b>
</body>
</html>";

                AddMailList(message, config);

                var smtp = new SmtpClient(config["HOST"], int.Parse(config["PORT"]))
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        config["USERNAME"], config["PASSWORD"])
                };

                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendDailyVehicleReportMail: " + ex);
                return false;
            }
        }
        private string BuildWeightGroupRows(List<TotalWeightByHourModel> list)
        {
            if (list == null || !list.Any()) return "";

            return string.Join("", list.Select(x => $@"
<tr>
<td>{x.WeightGroup}</td>
<td>{x.SanLuong}</td>
<td>{x.SoPhut_Tren_Tan}</td>
<td>{x.SoPhut_Tren_Xe}</td>
</tr>"));
        }

        // ================= HELPER =================

        private void AddMailList(MailMessage message, IConfigurationSection config)
        {
            if (!string.IsNullOrEmpty(config["TO_MAIL"]))
            {
                foreach (var mail in config["TO_MAIL"]
                    .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(mail.Trim());
                }
            }

            if (!string.IsNullOrEmpty(config["BCC_MAIL"]))
            {
                foreach (var mail in config["BCC_MAIL"]
                    .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Bcc.Add(mail.Trim());
                }
            }
        }

        private string BuildTroughRows(List<TotalWeightByHourModel> list)
        {
            if (list == null || !list.Any()) return "";

            return string.Join("", list.Select(x => $@"
<tr>
<td>{x.TroughType}</td>
<td>{x.SanLuong}</td>
<td>{x.TongGio}</td>
<td>{x.Tan_Moi_Gio}</td>
</tr>"));
        }


        private double CalcMinutePerTon(TotalVehicleInspection r)
        {
            if (r.TotalTimeWorkInHour <= 0 || r.TotalWeightTroughType <= 0)
                return 0;

            return Math.Round(
                (r.TotalTimeWorkInHour * 60) / r.TotalWeightTroughType,
                2
            );
        }









    }
}

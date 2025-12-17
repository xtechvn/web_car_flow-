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

                if (config == null)
                    throw new Exception("MAIL_CONFIG not found");

                var message = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = $"Báo cáo xuất hàng ngày {reportDate:dd/MM/yyyy} – Nhà máy Hà Nam",
                    From = new MailAddress(config["FROM_MAIL"])
                };

                message.Body = $@"
<html>
<body style='margin:0;padding:0;
font-family:Arial,Helvetica,sans-serif;
font-size:14px;color:#00264D;background-color:#f6f6f6;'>

<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td align='center'>

<table width='100%' cellpadding='0' cellspacing='0'
style='max-width:800px;margin:0 auto;background:#fff;border:1px solid #E3EBF3;'>

<!-- TITLE -->
<tr>
<td style='padding:16px;text-align:center;font-size:18px;font-weight:bold;color:#070BA0;'>
BÁO CÁO XUẤT HÀNG NGÀY {reportDate:dd/MM/yyyy} – NHÀ MÁY HÀ NAM
</td>
</tr>

<!-- META -->
<tr>
<td style='padding:8px 16px;font-size:13px;line-height:1.6;'>
<b>Kính gửi:</b> Ban Lãnh đạo Công ty<br/>
<b>Đơn vị báo cáo:</b> Hệ thống vận hành xuất hàng<br/>
<b>Thời điểm chốt số liệu:</b> {DateTime.Now:HH:mm:ss} – ngày {reportDate:dd/MM/yyyy}
</td>
</tr>

<!-- I. CHỈ TIÊU CHÍNH -->
<tr>
<td style='padding:12px 16px;font-weight:bold;font-size:15px;'>
I. CHỈ TIÊU CHÍNH
</td>
</tr>

<tr>
<td style='padding:0 16px 24px 16px;'>
<table width='100%' cellpadding='0' cellspacing='0'
style='border-collapse:collapse;border:1px solid #E3EBF3;'>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Tổng xe đăng ký</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{s.TotalCar} xe</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Số chuyến đã xuất</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{s.TotalCarCompleted} chuyến</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Tổng khối lượng đã xuất</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{s.TotalWeightTroughType:N0} tấn</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Tổng thời gian làm việc</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{s.TotalTimeWorkInHour} giờ</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Năng suất trung bình</td><td style='padding:8px 12px;border:1px solid #E3EBF3;font-weight:bold;color:#D4380D;'>{s.AverageProductivity} tấn/giờ</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>TB khối lượng / chuyến</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{avgTonPerTrip} tấn</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>TB thời gian / chuyến</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{Math.Round(s.AvgTimePerCompletedCar_Hour * 60, 2)} phút</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Phút / tấn</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{minutePerTon}</td></tr>
<tr><td style='padding:8px 12px;font-weight:bold;border:1px solid #E3EBF3;'>Xe đến sau 16h</td><td style='padding:8px 12px;border:1px solid #E3EBF3;'>{s.TotalCarArriving16h}</td></tr>
</table>
</td>
</tr>

<!-- III -->
<tr>
<td style='padding:16px;font-weight:bold;font-size:15px;'>
II. HIỆU SUẤT THEO NHÓM XE
</td>
</tr>

<tr>
<td style='padding:0 16px 32px 16px;'>
<table width='100%' cellpadding='0' cellspacing='0'
style='border-collapse:collapse;
       border:1px solid #D9D9D9;
       table-layout:fixed;'>

<tr style='background:#F5F7FA;font-weight:bold;'>
  <td style='width:40%;padding:10px;border:1px solid #D9D9D9;'>
    Nhóm xe
  </td>
  <td style='width:20%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Sản lượng (tấn)
  </td>
  <td style='width:20%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Phút / tấn
  </td>
  <td style='width:20%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Phút / xe
  </td>
</tr>

{BuildWeightGroupRows(byWeightGroup)}

</table>
</td>
</tr>


<!-- IV -->
<tr>
<td style='padding:16px;font-weight:bold;font-size:15px;'>
III. HIỆU SUẤT THEO MÁNG XUẤT
</td>
</tr>

<tr>
<td style='padding:0 16px 32px 16px;'>
<table width='100%' cellpadding='0' cellspacing='0'
style='border-collapse:collapse;
       border:1px solid #D9D9D9;
       table-layout:fixed;'>

<tr style='background:#F5F7FA;font-weight:bold;'>
  <td style='width:25%;padding:10px;border:1px solid #D9D9D9;'>
    Máng
  </td>
  <td style='width:25%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Sản lượng (tấn)
  </td>
  <td style='width:25%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Thời gian (giờ)
  </td>
  <td style='width:25%;padding:10px;border:1px solid #D9D9D9;text-align:right;'>
    Năng suất (tấn/giờ)
  </td>
</tr>

{BuildTroughRows(byTrough)}

</table>
</td>
</tr>


<tr>
<td style='padding:16px;font-size:13px;'>
<b>Nhận định & Đề xuất:</b>
<ul style='margin:6px 0 0 18px;padding:0;'>
<li>Máng có năng suất cao nhất cần ưu tiên khai thác.</li>
<li>Theo dõi máng hiệu suất thấp để điều chỉnh nhân lực / luồng xe.</li>
</ul>
<br/>
<b>Hệ thống Báo cáo Vận hành</b>
</td>
</tr>

</table>
</td>
</tr>
</table>
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
                LogHelper.InsertLogTelegram(
                    "SendDailyVehicleReportMail ERROR: " + ex
                );
                return false; // ✅ QUAN TRỌNG
            }

        }

        private string BuildWeightGroupRows(List<TotalWeightByHourModel> list)
        {
            if (list == null || !list.Any()) return "";

            return string.Join("", list.Select(x => $@"
<tr>
  <td style='width:40%;padding:8px 10px;border:1px solid #D9D9D9;'>
    {x.WeightGroup}
  </td>
  <td style='width:20%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.SanLuong:N2}
  </td>
  <td style='width:20%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.SoPhut_Tren_Tan}
  </td>
  <td style='width:20%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.SoPhut_Tren_Xe}
  </td>
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
  <td style='width:25%;padding:8px 10px;border:1px solid #D9D9D9;'>
    {x.TroughType}
  </td>
  <td style='width:25%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.SanLuong:N2}
  </td>
  <td style='width:25%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.TongGio}
  </td>
  <td style='width:25%;padding:8px 10px;border:1px solid #D9D9D9;text-align:right;'>
    {x.Tan_Moi_Gio}
  </td>
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

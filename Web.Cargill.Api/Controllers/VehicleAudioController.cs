using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using System;

using Microsoft.EntityFrameworkCore;
using B2B.Utilities.Common;
using Web.Cargill.Api.Model;
using NAudio.Wave;
using NAudio.Lame;                 // NuGet: NAudio


namespace Web.Cargill.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleAudioController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public VehicleAudioController(AppDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _config = config;
        }

        [HttpPost("upload-audio")]
        public async Task<IActionResult> UploadAudio([FromForm] int booking_id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Status = 1, Msg = "File không hợp lệ." });

            try
            {
                var booking = _db.VehicleInspection.FirstOrDefault(b => b.Id == booking_id);
                if (booking == null)
                    return NotFound(new { Status = 1, Msg = $"Không tìm thấy booking_id = {booking_id}" });

                // Lấy biển số xe, loại bỏ toàn bộ ký tự đặc biệt, khoảng trắng
                string vehicleNumber = booking.VehicleNumber ?? "unknown";
                vehicleNumber = new string(vehicleNumber
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                    .ToLower(); // viết thường cho nhất quán

                // Lấy số thứ tự (RecordNumber) nếu có, mặc định = 1
                int recordNumber = booking.RecordNumber ?? 1;

                // Tạo tên file: audio + bookingId + vehicleNumber + recordNumber (viết liền)
                string customFileName = $"audio{booking_id}_{vehicleNumber}_{recordNumber}";

                // Gọi helper upload
                string audioUrl = await UpLoadHelper.UploadFileOrImage(file, booking_id, 999, customFileName);

                if (string.IsNullOrEmpty(audioUrl))
                    return StatusCode(500, new { Status = 1, Msg = "Upload thất bại từ UpLoadHelper" });

                // Cập nhật DB
                booking.AudioPath = audioUrl;
                await _db.SaveChangesAsync();

                return Ok(new { Status = 0, Url = audioUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Msg = ex.Message, StackTrace = ex.ToString() });
            }
        }

        /// <summary>
        /// Fallback: nhận link Zalo (.wav) -> download -> convert MP3 (in-memory) -> upload -> update DB
        /// form-data: booking_id (Text), link_audio_zalo_ai (Text)
        /// </summary>
        [HttpPost("download-zalo-audio")]
        public async Task<IActionResult> DownloadZaloAudio(
    [FromForm] int booking_id,
    [FromForm] string link_audio_zalo_ai,
    CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(link_audio_zalo_ai))
                return BadRequest(new { Status = 1, Msg = "Link audio không hợp lệ." });

            try
            {
                // 1) Download WAV về MemoryStream
                var wavStream = await DownloadWavToStreamAsync(link_audio_zalo_ai, maxBytes: 20 * 1024 * 1024, ct: ct);
                if (wavStream == null || wavStream.Length == 0)
                    return StatusCode(502, new { Status = 1, Msg = "Không tải được file từ Zalo AI" });

                // 2) Convert WAV -> MP3 (in-memory)
                var mp3Stream = ConvertWavStreamToMp3Stream(wavStream);

                // 3) Lấy thông tin booking để đặt tên file
                var booking = await _db.VehicleInspection.FirstOrDefaultAsync(b => b.Id == booking_id);
                if (booking == null)
                    return NotFound(new { Status = 1, Msg = $"Không tìm thấy booking_id = {booking_id}" });

                string vehicleNumber = booking.VehicleNumber ?? "unknown";
                vehicleNumber = new string(vehicleNumber.Where(char.IsLetterOrDigit).ToArray()).ToLower();

                int recordNumber = booking.RecordNumber ?? 1;

                // Format tên file: audio + bookingId + vehicleNumber + recordNumber (viết liền)
                string customFileName = $"audio{booking_id}_{vehicleNumber}_{recordNumber}";

                // 4) Tạo IFormFile và upload
                mp3Stream.Position = 0;
                var formFile = new FormFile(mp3Stream, 0, mp3Stream.Length, "file", $"{customFileName}.mp3")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "audio/mpeg"
                };

                string? audioUrl = await UpLoadHelper.UploadFileOrImage(formFile, booking_id, 999, customFileName);
                if (string.IsNullOrEmpty(audioUrl))
                    return StatusCode(500, new { Status = 1, Msg = "Upload thất bại sau khi convert" });

                // 5) Update DB
                booking.AudioPath = audioUrl;
                await _db.SaveChangesAsync();

                return Ok(new { Status = 0, Url = audioUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Msg = ex.Message });
            }
        }


        // ===== Helpers =====

        private async Task<MemoryStream> DownloadWavToStreamAsync(string url, long maxBytes, CancellationToken ct)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

            // GET file
            using var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Tải WAV thất bại: HTTP {(int)resp.StatusCode}");

            var contentLength = resp.Content.Headers.ContentLength; // có thể null
            if (contentLength.HasValue && contentLength.Value > maxBytes)
                throw new Exception($"File quá lớn (> {maxBytes / (1024 * 1024)}MB)");

            // tạo stream với capacity hợp lý
            var capacity = contentLength.HasValue ? (int)Math.Min(contentLength.Value, maxBytes) : 0;
            var ms = capacity > 0 ? new MemoryStream(capacity) : new MemoryStream();

            await using (var s = await resp.Content.ReadAsStreamAsync(ct))
            {
                await s.CopyToAsync(ms, 81920, ct);
            }
            ms.Position = 0;
            return ms;
        }

        private MemoryStream ConvertWavStreamToMp3Stream(Stream wavStream)
        {
            wavStream.Position = 0;
            using var reader = new WaveFileReader(wavStream);

            WaveStream pcmStream = reader;
            if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
            }

            var mp3 = new MemoryStream();
            using (var lame = new LameMP3FileWriter(mp3, pcmStream.WaveFormat, LAMEPreset.VBR_90))
            {
                pcmStream.CopyTo(lame);
            }

            mp3.Position = 0;
            if (pcmStream != reader) pcmStream.Dispose();
            return mp3;
        }



    }
}

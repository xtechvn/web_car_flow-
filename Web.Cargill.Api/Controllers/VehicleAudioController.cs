using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using System;

using Microsoft.EntityFrameworkCore;
using B2B.Utilities.Common;
using Web.Cargill.Api.Model;

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
                // Gọi helper để upload
                string audioUrl = await UpLoadHelper.UploadFileOrImage(file, booking_id, 3);

                if (string.IsNullOrEmpty(audioUrl))
                    return StatusCode(500, new { Status = 1, Msg = "Upload thất bại từ UpLoadHelper" });

                // Update DB theo booking_id
                var booking = _db.VehicleInspection.FirstOrDefault(b => b.Id == booking_id);
                if (booking == null)
                    return NotFound(new { Status = 1, Msg = $"Không tìm thấy booking_id = {booking_id}" });

                booking.AudioPath = audioUrl;
                await _db.SaveChangesAsync();

                return Ok(new { Status = 0, Url = audioUrl });
            }
            catch (Exception ex)
            {
                // Trả cả message để dễ trace
                return StatusCode(500, new { Status = 1, Msg = ex.Message, StackTrace = ex.ToString() });
            }
        }



    }
}

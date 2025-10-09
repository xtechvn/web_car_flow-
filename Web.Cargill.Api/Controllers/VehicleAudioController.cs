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
                return BadRequest("File không hợp lệ.");

            try
            {
                // Gọi helper để upload
                string audioUrl = await UpLoadHelper.UploadFileOrImage(file, booking_id, 3);
                // 3 = type tự quy định, ví dụ: 3 = audio

                if (string.IsNullOrEmpty(audioUrl))
                    return StatusCode(500, "Upload thất bại");

                // Update DB theo booking_id
                var booking = _db.VehicleInspection.FirstOrDefault(b => b.Id == booking_id);
                if (booking == null)
                    return NotFound("Không tìm thấy booking.");

                booking.AudioPath = audioUrl;
                await _db.SaveChangesAsync();

                return Ok(new { Status = 0, url = audioUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1 });
            }
        }


    }
}

using App_Push_Consummer.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using StackExchange.Redis;
using System.Text.RegularExpressions;
using System.Configuration;
using App_Push_Consummer.Interfaces;
using System.Linq;
using DnsClient.Protocol;
using App_Push_Consummer.Utilities;

namespace App_Push_Consummer.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService
    {

        private readonly SheetsService _sheetsService;

        private readonly string _spreadsheetId;
        private readonly string _sheetName;

        public GoogleSheetsService()
        {
            // Get configuration
            _spreadsheetId = ConfigurationManager.AppSettings["SpreadsheetId"]
                ?? "1mocqFI7Gue7E47K3LjhPTCYbEt7Rl-Gw1MrchDHk_dA";
            _sheetName = ConfigurationManager.AppSettings["SheetName"] ?? "Sheet1";

            // Initialize Google Sheets service
            _sheetsService = InitializeSheetsService();
        }
        private SheetsService InitializeSheetsService()
        {
            try
            {
                var serviceAccountFile = ConfigurationManager.AppSettings["ServiceAccountFile"];
                if (!string.IsNullOrEmpty(serviceAccountFile) && File.Exists(serviceAccountFile))
                {
                    var credential = GoogleCredential.FromFile(serviceAccountFile)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);

                    return new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Car Registration API"
                    });
                }

                var serviceAccountJson = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"disco-horizon-462708-i0\",\n  \"private_key_id\": \"517279e6f03423aa2195d3796f5d3e820641dbc9\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCuDYxex3lOC4FO\\nx7aiAb2TrgRsfNbR0nAKUlhCDn/sGayGrgFqLeGS+ue7wvU3sOCJa/4g3I9e+IsL\\nLW0xz7Xab4L66taskD8HmmyzLkNcgxQIfa9nAkrxxSEmiGAhgG71pdEX2NKrJciQ\\nH4KN5Q6CJ0H0Z7sfin7i2ZWa6D0L6ib2oL2yboy3/sgcLpzGmii4oxlsQ8//Kbrd\\nniUNpgH7yJWMNzGHzH9DLnvqh9s6XqJAGkmYuEnXVUH12QYwkkFYp3r2mAc8Hjxa\\ns0xF7c2HUy5Y3elJgccP/trtBDz7cSkoycBkTRUYqOOf3SrjyTCm/j1SnPLIGZmh\\npwtZj9+pAgMBAAECggEAAIK1KfUDV9WETlcbI8wGApgk1q2iErD/l6Qosp7oxKhJ\\nn4aEpgtW6U+3nM8kWYK5EeYhc3a6K/DmNYTvWFFP6wFfPL2yHgT5TlwnW/ozg+K1\\njRXTTSXOUvm9UGffglGmYa2YGK4P5iUg+r1A7IoiugKD+MPSInRNTXyOagsq3K1U\\nrUU5b0s3xmoujFUY6ee7F+IjqVh2ukPezMNnFK1uVXBwtQ1guqnJcKvg3k5PP+8a\\nfs8juLn3fomO3AhTYMzwvR+xW1kqV8HCLEkwepMm7g1floRewc8AS1ZdxJtxVT+U\\nfQJllb3GRkM3Km/r41+Fg1yLx7mV0y6Y9hK+3vW1IQKBgQDY5/U1tiU73ASDIWEX\\nrXgrU8P01BvTZ8LlWEvQQaWigGBCexJd/94woOKanexQ48+MnJvlGloTv0DcQMt2\\nJkdIzB7DmcvZTHvfEmP8YPRkp13CelXxLmPdHgiq65aUEAQ7Pfe6gHdbz4OggYJK\\njdNPM+4FgFnlgpDTDTFpSwK48QKBgQDNbFiy58a6NkA6TtUuinWMuMkcn/qdaP+M\\ncBRJXyCmOssAL1MOu4JPWvbwCMq88o0uY7rwfXOOTpsGtc4ZBt9kUrNabb7xHnAw\\nnWHvVactWLr6KdKBsD674xmq4YW1UL4st1+2edO0k2VGPoe3qaN5ExwDhzSrtX3u\\noip41i/SOQKBgB3xT2ldvqAXzEup1PRmzvr0Mk3e5gR2A3KTkMur9EiNfjxPLwbo\\n5mxCGWYMvO2htSoNCHxE+gBV24dMLood2KNVAj4wQfK6WzM9H65cWAB5Fjldl/WW\\nWNTSa5HkucGwwFTJRiRpzZBQAjSrDChskaoSWh5KTJ6hOorX/GUzpKmRAoGBAIjJ\\ntOas3+/vYCVziRFMsanbAlBFVySqXkCuAVQ6PAt06uhcmvocclFVSUndEONwwAI7\\n9qddYi1IuoJlXa/cm7S6PSPiIFt+4UX+BtDQQFo504fxgXNKYPvL5bOcKOTrtzcf\\nhGSGCysbWzzDNqxeEbT1vJm81p3gZNMauR1twrpBAoGBAKkA4EqQRcyZ7VEI+iPQ\\nCeCm8+YxaFfXGde5rA5Akt34CL3baq+TIEdaH0GGZl+kzHfsN+hj1bthxGoGvIo9\\nvKcJGlpEF4aSFFTtAX5n0KrifTO8n7gWXGL4fmqsJEJCtlqfFXyK+UonjzrmH3ed\\nZWfZBzzlxWtM8p4Y/Egs4mpQ\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"google-sheet-bot@disco-horizon-462708-i0.iam.gserviceaccount.com\",\n  \"client_id\": \"114568941997422987816\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/google-sheet-bot%40disco-horizon-462708-i0.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
;
                if (!string.IsNullOrEmpty(serviceAccountJson))
                {
                    var credential = GoogleCredential.FromJson(serviceAccountJson)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);

                    return new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Car Registration API"
                    });
                }

                throw new InvalidOperationException("Google Sheets credentials not configured properly");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InitializeSheetsService - GoogleSheetsService: " + ex.Message);
                throw;
            }

        }
        public async Task<bool> SaveRegistrationAsync(RegistrationRecord record)
        {
            try
            {
                // Updated to include Zalo Status column
                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        record.Name,
                        record.PlateNumber,
                        record.GPLX,
                        record.Referee,
                        record.PhoneNumber,
                        record.QueueNumber,
                        record.RegistrationTime.ToString("yyyy-MM-dd HH:mm:ss").ToString(),
                        record.ZaloStatus,
                        record.Camp
                    }
                };

                var valueRange = new ValueRange
                {
                    Values = values
                };

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(
                    valueRange,
                    _spreadsheetId,
                    $"{_sheetName}!A:E");

                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

                var appendResponse = await appendRequest.ExecuteAsync();


                // 2. Lấy dòng vừa chèn
                //var updatedRange = appendResponse.Updates.UpdatedRange; // ví dụ: "Sheet1!A10:E10"
                //var startRow = int.Parse(Regex.Match(updatedRange, @"[A-Z]+(\d+)").Groups[1].Value) - 1;

                //// 3. Lấy SheetId (không phải tên)
                //var spreadsheet = await _sheetsService.Spreadsheets.Get(_spreadsheetId).ExecuteAsync();
                //var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == _sheetName);
                //var sheetId = sheet?.Properties.SheetId;

                //if (sheetId == null)
                //{
                //    throw new Exception("Không tìm thấy SheetId.");
                //}
                //// 4. Gửi BatchUpdate để định dạng dòng
                //var formatRequest = new BatchUpdateSpreadsheetRequest
                //{
                //    Requests = new List<Request>
                //    {
                //        new Request
                //        {
                //            RepeatCell = new RepeatCellRequest
                //            {
                //                Range = new GridRange
                //                {
                //                    SheetId = sheetId,
                //                    StartRowIndex = startRow,
                //                    EndRowIndex = startRow + 1,
                //                    StartColumnIndex = 0,
                //                    EndColumnIndex = 9 // Cột A đến I (0 đến 8)
                //                },
                //                Cell = new CellData
                //                {
                //                    UserEnteredFormat = new CellFormat
                //                    {
                //                        BackgroundColor = new Color
                //                        {
                //                            Red = 1.0f, Green = 1.0f, Blue = 1.0f // Trắng
                //                        },
                //                        TextFormat = new TextFormat
                //                        {
                //                            ForegroundColor = new Color
                //                            {
                //                               Red = 0.0f, Green = 0.0f, Blue = 0.0f // Đen
                //                            },
                //                            Bold = true
                //                        }
                //                    }
                //                },
                //                Fields = "userEnteredFormat(backgroundColor,textFormat)"
                //            }
                //        }
                //    }
                //};

                //var batchRequest = _sheetsService.Spreadsheets.BatchUpdate(formatRequest, _spreadsheetId);
                //await batchRequest.ExecuteAsync();
                if (appendResponse.Updates.UpdatedRows.HasValue && appendResponse.Updates.UpdatedRows.Value > 0)
                {
                    //_logger.LogInformation($"Successfully saved registration to Google Sheets: {record.PhoneNumber} - {record.PlateNumber} - Queue: {record.QueueNumber} - Zalo: {record.ZaloStatus}- Camp: {record.Camp}");

                    //var today = DateTime.Today.ToString("yyyy-MM");
                    //var cacheKey = $"daily_count_{today}";
                    //if (_cache.TryGetValue(cacheKey, out int currentCount))
                    //{
                    //    _cache.Set(cacheKey, currentCount + 1);
                    //}

                    return true;
                }
                //_logger.LogWarning("No rows were updated when saving to Google Sheets");
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveRegistrationAsync - GoogleSheetsService: " + ex.Message);
                //_logger.LogError(ex, "Error saving registration to Google Sheets");
                return false;
            }
        }
        public async Task<int> GetDailyQueueCountRedis()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["Redis_Host"] + ":" + ConfigurationManager.AppSettings["Redis_Port"]);
                var db = redis.GetDatabase();
                // Tính effective date dựa trên giờ địa phương (UTC+7)
                DateTime now = DateTime.Now; // Sử dụng giờ hệ thống (giả định đã cấu hình đúng timezone)
                DateTime effectiveDate = now.Hour < 18 ? now.Date.AddDays(-1) : now.Date;

                string key = $"counter:daily_car_count_:{effectiveDate:yyyyMMdd}";
                long nextNumber = db.StringIncrement(key);

                // Đặt TTL nếu là lần đầu tăng
                if (nextNumber == 1)
                {
                    // Mục tiêu: 18 hôm nay
                    DateTime expireAt = new DateTime(now.Year, now.Month, now.Day, 17, 59, 0);

                    // Nếu đã quá 18 hôm nay → chuyển sang 18 ngày mai
                    if (now > expireAt)
                    {
                        expireAt = expireAt.AddDays(1);
                    }

                    TimeSpan ttl = expireAt - now;
                    db.KeyExpire(key, ttl);
                }


                Console.WriteLine($"Số thứ tự tiếp theo: {nextNumber}");
                return (int)nextNumber;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDailyQueueCountRedis - GoogleSheetsService: " + ex.Message);
                // _logger.LogError(ex, "Error getting daily queue count from Google Sheets");

                throw;
            }
        }
        public async Task<bool> SaveRegistrationEX(List<RegistrationRecordMongo> record)
        {
            try
            {
                // Updated to include Zalo Status column
                var values = new List<IList<object>>();
                foreach (var item in record)
                {
                    values.Add(new List<object>
                    {
                        item.Name,
                        item.PlateNumber,
                        item.GPLX,
                        item.Referee,
                        item.PhoneNumber,
                        item.QueueNumber,
                        item.CreatedTime,
                        item.ZaloStatus,
                        item.Camp
                    });
                }
                var valueRange = new ValueRange
                {
                    Values = values
                };

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(
                    valueRange,
                    _spreadsheetId,
                    $"{_sheetName}!A:E");

                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

                var appendResponse = await appendRequest.ExecuteAsync();

                if (appendResponse.Updates.UpdatedRows.HasValue && appendResponse.Updates.UpdatedRows.Value > 0)
                {

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SaveRegistrationEX - GoogleSheetsService: " + ex.Message);
                return false;
            }
        }
    }
}

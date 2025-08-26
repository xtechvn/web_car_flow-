using MongoDB.Bson;
using MongoDB.Driver;
using XTECH_FRONTEND.IRepositories;
using XTECH_FRONTEND.Model;

namespace XTECH_FRONTEND.Services
{
    public class MongoService : IMongoService
    {

        private readonly IConfiguration _configuration;
        public MongoService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<long> Insert(RegistrationRecord model)
        {
            try
            {
                string url = "mongodb://" + _configuration["MongoServer:user"] + ":" + _configuration["MongoServer:pwd"] + "@" + _configuration["MongoServer:Host"] + ":" + _configuration["MongoServer:Port"] + "/" + _configuration["MongoServer:catalog_log"];
                var client = new MongoClient(url);

                IMongoDatabase db = client.GetDatabase(_configuration["MongoServer:catalog_log"]);
                RegistrationRecord log = new RegistrationRecord()
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    PhoneNumber = model.PhoneNumber,
                    PlateNumber = model.PlateNumber.ToUpper(),
                    Name = model.Name,
                    Referee = model.Referee.ToUpper(),
                    GPLX = model.GPLX.ToUpper(),
                    QueueNumber = model.QueueNumber,
                    RegistrationTime = model.RegistrationTime,
                    ZaloStatus = model.ZaloStatus,
                    Camp = model.Camp

                };
                IMongoCollection<RegistrationRecord> affCollection = db.GetCollection<RegistrationRecord>(_configuration["MongoServer:Data_Car"]);


                await affCollection.InsertOneAsync(log);


                return model.QueueNumber;
            }
            catch (Exception ex)
            {
                //LogHelper.InsertLogTelegram("PushLog - LogActionMongoService: " + ex.Message);
            }
            return 0;
        }
    }
}

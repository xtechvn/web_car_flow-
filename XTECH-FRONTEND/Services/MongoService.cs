using Microsoft.AspNetCore.Razor.TagHelpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Newtonsoft.Json;
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
        public async Task<int> CheckPlateNumber(string PlateNumber)
        {
            var list_data = new List<RegistrationRecord>();
            try
            {
                var db = GetDatabase();

                var todayStart = DateTime.Today;
                var cutoffTime = todayStart.AddHours(18);
                var collection = db.GetCollection<RegistrationRecord>(_configuration["MongoServer:Data_Car"]);
                var filter = Builders<RegistrationRecord>.Filter.Empty;
                filter &= Builders<RegistrationRecord>.Filter.Eq(n => n.PlateNumber, PlateNumber);
                filter &= Builders<RegistrationRecord>.Filter.Gte("RegistrationTime", cutoffTime.AddDays(-1));
                filter &= Builders<RegistrationRecord>.Filter.Lte("RegistrationTime", cutoffTime);
                list_data = collection.Find(filter).ToList();
                if (list_data != null && list_data.Count > 0)
                {
                    return  1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                //LogHelper.InsertLogTelegram("GetListLogActions - CommentClientMongoService. " + JsonConvert.SerializeObject(ex));
            }
            return 0;
        }
        public static IMongoDatabase GetDatabase()
        {
            var host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["Host"];
            var port = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["Port"]);
            var catalog_log = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["catalog_log"];
            var user = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["user"];
            var pwd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MongoServer")["pwd"];
            var cred = MongoCredential.CreateCredential(catalog_log, user, pwd);
            var client = new MongoClient(
                    new MongoClientSettings()
                    {
                        Server = new MongoServerAddress(host, port),
                        ClusterConfigurator = cb =>
                        {
                            //var textWriter = TextWriter.Synchronized(new StreamWriter("mylogfile.txt"));
                            cb.Subscribe<CommandStartedEvent>(e =>
                            {
                                //log.Debug(e.Command.ToString());
                                //LogHelper.InsertLogTelegram(e.Command.ToString());
                            });
                        },
                        Credential = cred
                    });

            //them tinh nang bo qua cac truong co trong db nhung khong co trong mo ta class
            var pack = new ConventionPack();
            pack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);
            var db = client.GetDatabase(catalog_log);
            return db;
        }
    }
}

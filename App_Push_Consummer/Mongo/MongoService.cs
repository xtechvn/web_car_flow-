using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model;
using App_Push_Consummer.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Configuration;

namespace App_Push_Consummer.Mongo
{
    public class MongoService : IMongoService
    {

        public MongoService()
        {


        }
        public async Task<long> Insert(RegistrationRecord model)
        {
            try
            {
                string url = "mongodb://" + ConfigurationManager.AppSettings["MongoServer_user"] + ":" + ConfigurationManager.AppSettings["MongoServer_pwd"] + "@" + ConfigurationManager.AppSettings["MongoServer_Host"] + ":" + ConfigurationManager.AppSettings["MongoServer_Port"];

                var client = new MongoClient(url);

                IMongoDatabase db = client.GetDatabase(ConfigurationManager.AppSettings["MongoServer_catalog_log"]);
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
                IMongoCollection<RegistrationRecord> affCollection = db.GetCollection<RegistrationRecord>(ConfigurationManager.AppSettings["MongoServer_Data_Car"]);
                await affCollection.InsertOneAsync(log);
                return model.QueueNumber;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Insert - MongoService: " + ex.Message);
            }
            return 0;
        }

        public List<RegistrationRecordMongo> GetList()
        {
            var list = new List<RegistrationRecordMongo>();
            try
            {

                string url = "mongodb://" + ConfigurationManager.AppSettings["MongoServer_user"] + ":" + ConfigurationManager.AppSettings["MongoServer_pwd"] + "@" + ConfigurationManager.AppSettings["MongoServer_Host"] + ":" + ConfigurationManager.AppSettings["MongoServer_Port"];

                var client = new MongoClient(url);
                var db = client.GetDatabase(ConfigurationManager.AppSettings["MongoServer_catalog_log"]);
                var collection = db.GetCollection<RegistrationRecordMongo>(ConfigurationManager.AppSettings["MongoServer_Data_Car"]);

                var now = DateTime.Now;
                var DateTime_Lte = new DateTime(now.Year, now.Month, now.Day, 18, 29, 59);
                var DateTime_Gte = new DateTime(now.Year, now.Month, now.Day, 17, 55, 0);
                var filter = Builders<RegistrationRecordMongo>.Filter.Empty;

                filter &= Builders<RegistrationRecordMongo>.Filter.Eq(n => n.Type, 0);
                filter &= Builders<RegistrationRecordMongo>.Filter.Gte("RegistrationTime", DateTime_Gte);
                filter &= Builders<RegistrationRecordMongo>.Filter.Lte("RegistrationTime", DateTime_Lte);


                var S = Builders<RegistrationRecordMongo>.Sort.Ascending("QueueNumber");
                list = collection.Find(filter).Sort(S).ToList();
                return list;

            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("GetList - MongoService: " + ex.Message);
            }
            return list;
        }
        public async Task<long> update(RegistrationRecordMongo item, string id)
        {
            try
            {
                string url = "mongodb://" + ConfigurationManager.AppSettings["MongoServer_user"] + ":" + ConfigurationManager.AppSettings["MongoServer_pwd"] + "@" + ConfigurationManager.AppSettings["MongoServer_Host"] + ":" + ConfigurationManager.AppSettings["MongoServer_Port"];

                var client = new MongoClient(url);
                var db = client.GetDatabase(ConfigurationManager.AppSettings["MongoServer_catalog_log"]);
                var collection = db.GetCollection<RegistrationRecordMongo>(ConfigurationManager.AppSettings["MongoServer_Data_Car"]);
                if (id != null && id.Trim() != "")
                {
                    var filter = Builders<RegistrationRecordMongo>.Filter;
                    var filterDefinition = filter.Empty;
                    filterDefinition &= Builders<RegistrationRecordMongo>.Filter.Eq(x => x._id, id);
     
                    await collection.ReplaceOneAsync(filterDefinition, item);
                    return item.QueueNumber;
                }
                return -1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertBooking - BookingHotelDAL - Cannot Excute: " + ex.ToString());
                return 0;
            }
        }
    }
}

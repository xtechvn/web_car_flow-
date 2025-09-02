using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Engines;
using App_Push_Consummer.GoogleSheets;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Mongo;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Configuration;
using System.Text;

namespace App_Push_Consummer
{
    /// <summary>
    /// App: Xử lý các tiến trình được push về từ FRONT END.
    /// /// </summary>

    class Program
    {
        private static string queue_checkout_order = ConfigurationManager.AppSettings["queue_name"];
        public static string QUEUE_HOST = ConfigurationManager.AppSettings["QUEUE_HOST"];
        public static string QUEUE_V_HOST = ConfigurationManager.AppSettings["QUEUE_V_HOST"];
        public static string QUEUE_USERNAME = ConfigurationManager.AppSettings["QUEUE_USERNAME"];
        public static string QUEUE_PASSWORD = ConfigurationManager.AppSettings["QUEUE_PASSWORD"];
        public static string QUEUE_PORT = ConfigurationManager.AppSettings["QUEUE_PORT"];
        public static string QUEUE_KEY_API = ConfigurationManager.AppSettings["QUEUE_KEY_API"];
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        static void Main(string[] args)
        {
            try
            {


                #region READ QUEUE
                var factory = new ConnectionFactory()
                {
                    HostName = QUEUE_HOST,
                    UserName = QUEUE_USERNAME,
                    Password = QUEUE_PASSWORD,
                    VirtualHost = QUEUE_V_HOST,
                    Port = Protocols.DefaultProtocol.DefaultPort
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    try
                    {
                        channel.QueueDeclare(queue: queue_checkout_order,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        Console.WriteLine(" [*] Waiting for messages.");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (sender, ea) =>
                        {
                            try
                            {

                                var body = ea.Body.ToArray();
                                var message = Encoding.UTF8.GetString(body);

                                using (var serviceProvider = new ServiceCollection()
                                   .AddSingleton<IFactory, Factory>()
                                   .AddSingleton<IGoogleSheetsService, GoogleSheetsService>()
                                   .AddSingleton<IMongoService, MongoService>()
                                   .BuildServiceProvider())
                                {
                                    var _factory = serviceProvider.GetService<IFactory>();
                                    _factory.DoSomeRealWork(message);
                                }

                                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("error queue: " + ex.ToString());
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "error queue = " + ex.ToString());
                            }
                        };

                        channel.BasicConsume(queue: queue_checkout_order, autoAck: false, consumer: consumer);

                        // ========== Background Task chạy CheckEx() lúc 18:00 ==========
                        Task.Run(async () =>
                        {
                            while (true)
                            {
                                var now = DateTime.Now;

                                if (now.Hour == 20 && now.Minute == 30)
                                {
                                    try
                                    {
                                        using (var serviceProvider = new ServiceCollection()
                                          .AddSingleton<IFactory, Factory>()
                                          .AddSingleton<IGoogleSheetsService, GoogleSheetsService>()
                                          .AddSingleton<IMongoService, MongoService>()
                                          .BuildServiceProvider())
                                        {
                                            var _factory = serviceProvider.GetService<IFactory>();
                                            _factory.UpdateEx();
                                        }
                                        Console.WriteLine($"[+] factory.CheckEx() executed at {now:HH:mm:ss}");
                                        await Task.Delay(TimeSpan.FromSeconds(61));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error running factory.CheckEx: {ex}");
                                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "error CheckEx = " + ex.ToString());
                                    }

                                    // Chờ thêm 1 phút để tránh bị gọi lại nhiều lần trong 1 phút
                                }
                                Console.WriteLine($"check time: {DateTime.Now:HH:mm:ss}");
                                await Task.Delay(TimeSpan.FromSeconds(30)); // check lại mỗi 30s
                            }
                        });
                        // ===============================================================

                        Console.ReadLine();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "error queue = " + ex.ToString());
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Main (JOB APP_PUSH) => error queue = " + ex.ToString());
                Console.WriteLine(" [x] Received message: {0}", ex.ToString());
            }
        }

    }
}
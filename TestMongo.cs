using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TestMongo
{

    class TestPayLoad
   {
       public int Test { get; set; }
       public int Time { get; set; } = (int)Math.Truncate(DateTime.UtcNow.TimeOfDay.TotalSeconds);
   }

    class Program
   {
       static void Main()
       {
           const string connectionString = "mongodb://localhost:27017";
           var client = new MongoClient(connectionString);
           var db = client.GetDatabase("SizeBasedtimeSeriesBase");
           var collection = db.GetCollection<BsonDocument>("SizeBasedtimeSeries");

            for (var i = 0; i < 100; i++)
           {
               UpdateWithNewTest(collection, "toto", new TestPayLoad() { Test = i });
               // Thread.Sleep(100); // to go slower
           }
       }


        private static void UpdateWithNewTest(IMongoCollection<BsonDocument> collection, string deviceId, TestPayLoad test)
       {
           var day = "ISODate(\"" + DateTime.UtcNow.Date.ToString("yyyy-MM-dd") + "\")";

            var filter = "{'deviceid': '" + deviceId + "', nsamples: {$lt: 50}, day: " + day + "}";

          var update = Builders<BsonDocument>.Update
               .Inc("nsamples", 1)
               .Min("first", "test.time")
               .Max("latest", "test.time")
               .Push("tests", test);

            Console.WriteLine(value: "Update Renderer : " + update.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry));

            collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });


        }
   }
}


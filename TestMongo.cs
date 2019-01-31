using System;
using MongoDB.Bson;
using MongoDB.Driver;
/* Original code created by Ric Itto. 
 * 
 * This code is a C# implementation of "sized-based bucketing" as discussed
 * in the blog post "Time-Series Data and MongoDB: Part 2 - Schema Design"
 * (https://www.mongodb.com/blog/post/time-series-data-and-mongodb-part-2-schema-design-best-practices) 
 * by Robert Walters. September 13, 2018.
 *
 * This code changed by Bob Cochran (r2cochran2@gmail.com, BobCochran on GitHub) 
 * to do the following:
 *
 * 1. Shorten database and collection names.
 * 2. Reduce the number of samples bucketed within the document, 
 *    so that output collection documents can be studied more easily.
 *
 */
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

           // Set up database connection. 
           const string connectionString = "mongodb://localhost:27017";
           var client = new MongoClient(connectionString);
           var db = client.GetDatabase("itto");

           // Work with this collection.
           var collection = db.GetCollection<BsonDocument>("szbase2");

           /* The below code will generate 0 to n test samples for adding
            * to the document that is generated. So suppose that the value
            * of the field 'nsamples' is set to 3, and the for loop below loops
            * until i < 4. This will cause two documents to be written to the
            * collection: Test samples 0, 1, 2 are written to the first document.
            * Because the insert is coded with upsert=true, a second document 
            * is then created and it contains test sample 3.
            */ 
            for (var i = 0; i < 4; i++)
           {
               UpdateWithNewTest(collection, "toto", new TestPayLoad() { Test = i });
               // Thread.Sleep(100); // to go slower
           }
       }


        /* If no collection exists and upsert=true, this will cause a new collection
         * and a new document to be created. This code actually seems to assume this
         * use case: it has no provision for updating an existing document inside 
         * a collection.
         */
        private static void UpdateWithNewTest(IMongoCollection<BsonDocument> collection, string deviceId, TestPayLoad test)
       {

        /* Implement suggestion from Robert Walters: let the driver handle the date natively */
 
           var myday = new BsonDocument { { "day", DateTime.UtcNow.Date } };

            var filter = "{'deviceid': '" + deviceId + "', nsamples: {$lt: 3}, " + myday + "}";

          var update = Builders<BsonDocument>.Update
               .Inc("nsamples", 1)
               .Min("first", "test.time")
               .Max("latest", "test.time")
               .Push("tests", test);

            Console.WriteLine(value: "Update Renderer : " + update.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry));

            /* IMongoCollection(TDocument) UpdateOne method
             * (IClientSessionHandle, FilterDefinition(TDocument), 
             * UpdateDefinition(TDocument),
             * UpdateOptions, CancellationToken)
             */ 
            collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });


        }
   }
}


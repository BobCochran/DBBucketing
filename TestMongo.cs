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
 * This implementation of TestMongo.cs results because of a post made by Ric Itto to the 
 * 'mongodb-users' mailing list on Google Groups. Mr. Itto implemented size-based
 * bucketing based on the Walters article. However, Itto had trouble posting correct
 * values to the "first" and "latest" fields in his code, and he requested help
 * with solving this problem.
 *
 * This code changed by Bob Cochran (r2cochran2@gmail.com, BobCochran on GitHub) 
 * to do the following:
 *
 * 1. Shorten database and collection names.
 * 2. Reduce the number of samples bucketed within the document, 
 *    so that output collection documents can be studied more easily.
 * 3. Convert the field "day" to use a true Date() object.
 * 4. Correct the values of the "first" and "latest" fields.
 * 5. Use epoch time for the number of seconds.
 *
 */
namespace TestMongo
{

    class TestPayLoad
   {
       public int Test { get; set; }
       /* Implement suggestion from Robert Walters: use epoch time for the test samples */
       public int Time { get; set; } = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
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
           var collection = db.GetCollection<BsonDocument>("szbase9");

           // Implement use of epoch time now in seconds.

           int minTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds; 

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
               UpdateWithNewTest(collection, "toto", new TestPayLoad() { Test = i }, minTime);
               // Thread.Sleep(100); // to go slower
           }
       }


        /* If no collection exists and upsert=true, this will cause a new collection
         * and a new document to be created. This code actually seems to assume this
         * use case: it has no provision for updating an existing document inside 
         * a collection.
         */
        private static void UpdateWithNewTest(IMongoCollection<BsonDocument> collection, string deviceId, TestPayLoad test, int theMinTime)
       {

        /* Implement suggestion from Robert Walters: let the driver handle the date natively */
 
           Console.WriteLine("\nThe deviceid is " + deviceId);

           var myday = new BsonDocument { { "day", DateTime.UtcNow.Date } };

           Console.WriteLine("The date is " + DateTime.UtcNow );

           Console.WriteLine("The time in seconds for today is " + theMinTime );

            var data1 = Builders<BsonDocument>.Update.Set( "deviceid", deviceId)
                  .Inc( "nsamples", 1 )
                  .Min( "first", theMinTime )
                  .Max( "latest", theMinTime )
            /* Use a full UTC date and time, including the correct time value */
                  .Set( "day", DateTime.UtcNow)
                  .Push( "tests", test);

            var builder = Builders<BsonDocument>.Filter;

            var filter3 = builder.Eq("deviceid", deviceId) & builder.Lt("nsamples", 3);
 
            /* IMongoCollection(TDocument) UpdateOne method
             * (IClientSessionHandle, FilterDefinition(TDocument), 
             * UpdateDefinition(TDocument),
             * UpdateOptions, CancellationToken)
             */ 
            collection.UpdateOne(filter3, data1, new UpdateOptions() { IsUpsert = true });


        }
   }
}


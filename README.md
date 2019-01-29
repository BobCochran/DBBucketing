# TestMongo 

This tool populates a MongoDB database with collection that implements size-based bucketing according to this blog post by [Robert Walters:](https://www.mongodb.com/blog/post/time-series-data-and-mongodb-part-2-schema-design-best-practices)

Example code provided is written in C#. Example code was originally written by Ric Itto. It is modified in this repository.

## Dependencies

This code is tested with MongoDB .NET Driver version 2.7.3.
MongoDB Enterprise database server and client version 4.0.5.
Microsoft .NET Core version 2.2.103.

This code was tested on a Ubuntu 18.04.1 LTS system.

Install Microsoft .NET Core in the latest version available.

Install MongoDB server and client utilities in the latest version available.

Install the MongoDB .NET Driver in the latest version available.
 
## Usage:

MongoDB server must be running.

Review the C# code elements and change the database and collection names to suit need.

Execute 'dotnet build' to build (compile) the project and all dependencies.

To run the code, execute 'dotnet run'.

## References

1. [Time-Series Data and MongoDB: Part 2 - Schema Design](https://www.mongodb.com/blog/post/time-series-data-and-mongodb-part-2-schema-design-best-practices) by Robert Walters. September 13, 2018.
2. Email thread started by Ric Itto on the 'MongoDB Users' Google Group.
3. Suggestions offered by Robert Walters in the above-referenced email thread.

## Credits

Ric Itto for the original email thread and source code that inspired this project.
Robert Walters for helpful advice while investigating the issues posed by Ric Itto.


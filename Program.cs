using System;
using MONGO_DB_SPACE;
using MongoDB.Bson;

namespace MongoDbApi
{
    class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string ObjType  {get; set;} = typeof(Person).Name;
        public int Age { get; set; }
        public Company Company { get; set; }
    }

    class Person2
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string ObjType {get; set;} = typeof(Person2).Name;
        public int Age { get; set; }
        public int Amount { get; set; }
        public Company Company { get; set; }

        public void Info()
        { Console.WriteLine("Person2 info");}
    }

    class Company
    {
        public string Name { get; set; }
    }

    class Program
    {
        static MONGO_DB_CLIENT db = null;
        static void Main(string[] args)
        {
            string connectionString = "mongodb://localhost";
            MONGO_DB_SETTINGS dbSettings;
            dbSettings.dbName = "test";
            dbSettings.collectionName = "colect1";

            db = new MONGO_DB_CLIENT();
            if (db.Connect(connectionString, dbSettings))
            {
                Console.WriteLine("Connected to: {0}", connectionString);

                Person per1 = new Person
                {
                    Name = "Add",
                };
                Person2 per2 = new Person2
                {
                    Name = "Add",
                    Age = 10,
                };

                db.InsertObj<Person2>(per2);

                Person per = new Person();

                

                if (db.FindByName<Person>(ref per, "Add") == eRESULT.SUCCESS)
                {
                    Console.WriteLine("Field already present!");
                    per.Name = "Dan";
                    per.Age = 12;

                    
                } else {
                    db.InsertObj<Person>(per1);
                    Console.WriteLine("Field were added!");
                }

            } else {
                Console.WriteLine("Can not connect to: {0}", connectionString);
            }

            //db.Init();
        }
    }
}
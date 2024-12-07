

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var baseUrl = "http://localhost:6000/api";

var healthClient = new MongoDB.Client.HealthClient($"{baseUrl}/health");

System.Console.WriteLine(await healthClient.GetHealthAsync());

var mongoDBClient = new MongoDB.Client.MongoDB_REST_Client($"{baseUrl}/mongodb");
var data = new Dictionary<string, object>();

var dbName = "test";
var collectionName = "source";

var id = await mongoDBClient.CreateAsync(dbName, collectionName, data);
System.Console.WriteLine(id);
var result = await mongoDBClient.ReadAsync(dbName, collectionName, id);
System.Console.WriteLine(result);
System.Console.WriteLine(await mongoDBClient.UpdateAsync(dbName, collectionName, id, data));

System.Console.WriteLine(await mongoDBClient.DeleteAsync(dbName, collectionName, id));

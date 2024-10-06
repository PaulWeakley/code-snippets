using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.CRUD.Tests
{
    public class MongoDBCrudClientTests
    {
        private readonly Mock<IMongoCollection<BsonDocument>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDBClientBuilder> _mockMongoDBClientBuilder;
        private readonly MongoDBCrudClient _mongoDBCrudClient;
        

        public MongoDBCrudClientTests()
        {
            _mockCollection = new Mock<IMongoCollection<BsonDocument>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
            _mockMongoDBClientBuilder = new Mock<IMongoDBClientBuilder>();
            
            _mockCollection.Setup(collection => collection.InsertOneAsync(It.IsAny<BsonDocument>(), null, default))
                           .Callback<BsonDocument, InsertOneOptions, System.Threading.CancellationToken>((doc, options, token) => doc.Add("_id", ObjectId.GenerateNewId()))
                           .Returns(Task.CompletedTask);
            _mockCollection.Setup(collection => collection.FindOneAndUpdateAsync(
                            It.IsAny<FilterDefinition<BsonDocument>>(),
                            It.IsAny<UpdateDefinition<BsonDocument>>(),
                            It.IsAny<FindOneAndUpdateOptions<BsonDocument>>(),
                            default))
                            .ReturnsAsync(new BsonDocument { { "_id", ObjectId.GenerateNewId() } });
            _mockCollection.Setup(collection => collection.DeleteOneAsync(
                            It.IsAny<FilterDefinition<BsonDocument>>(),
                            default))
                            .ReturnsAsync(new DeleteResult.Acknowledged(1));
            _mockDatabase.Setup(db => db.GetCollection<BsonDocument>(It.IsAny<string>(), null))
                         .Returns(_mockCollection.Object);
            _mockClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
                       .Returns(_mockDatabase.Object);
            _mockMongoDBClientBuilder.Setup(builder => builder.Build())
                                     .Returns(_mockClient.Object);
            
            _mongoDBCrudClient = new MongoDBCrudClient(_mockMongoDBClientBuilder.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertDocument()
        {
            var document = new BsonDocument { { "name", "test" } };

            await _mongoDBCrudClient.CreateAsync("test", "users", document, CancellationToken.None);

            _mockCollection.Verify(
                collection => collection.InsertOneAsync(document, null, default),
                Times.Once);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnDocument()
        {
            var document = new BsonDocument { { "_id", ObjectId.GenerateNewId() }, { "name", "test" } };
            var mockCursor = new Mock<IAsyncCursor<BsonDocument>>();
            mockCursor.SetupSequence(cursor => cursor.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                      .Returns(true)
                      .Returns(false);
            mockCursor.Setup(cursor => cursor.Current).Returns(new[] { document });

            _mockCollection.Setup(collection => collection.FindAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                default))
                .ReturnsAsync(mockCursor.Object);

            var result = await _mongoDBCrudClient.ReadAsync("test", "users", document["_id"].AsObjectId, CancellationToken.None);

            Assert.Equal(document, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDocument()
        {
            var document = new BsonDocument { { "_id", ObjectId.GenerateNewId() }, { "name", "test" } };
            var update = Builders<BsonDocument>.Update.Set("name", "updatedTest").ToBsonDocument();

            var updatedDocument = await _mongoDBCrudClient.UpdateAsync("test", "users", document["_id"].AsObjectId, update, default);

            _mockCollection.Verify(
                collection => collection.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument>>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteDocument()
        {
            var documentId = ObjectId.GenerateNewId();

            await _mongoDBCrudClient.DeleteAsync("test", "users", documentId, CancellationToken.None);

            _mockCollection.Verify(
                collection => collection.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    default),
                Times.Once);
        }
    }
}
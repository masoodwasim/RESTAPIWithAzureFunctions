using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using SampleFunctionsApp.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SampleFunctionsApp.Functions
{
    public class CreateBook
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<BookModel> _books;

        public CreateBook(
            MongoClient mongoClient,
            ILogger<CreateBook> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase(_config["Database"]);
            _books = database.GetCollection<BookModel>(_config["Collection"]);
        }

        [FunctionName(nameof(CreateBook))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CreateBook")] HttpRequest req)
        {
            IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<BookModel>(requestBody);

            var book = new BookModel
            {
                Title= input.Title,
                Author=input.Author,
                Publisher=input.Publisher,
                Category= input.Category, 
                Price = input.Price,
                PublishDate = input.PublishDate 
            };

            try
            {
                _books.InsertOne(book);
                returnValue = new OkObjectResult(book);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return returnValue;
        }
    }
}

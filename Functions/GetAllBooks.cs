using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using SampleFunctionsApp.Models;


namespace SampleFunctionsApp.Functions
{
    public class GetAllBooks
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<BookModel> _books;

        public GetAllBooks(
            MongoClient mongoClient,
            ILogger<GetAllBooks> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config; 

            var database = _mongoClient.GetDatabase(_config["Database"]);
            _books = database.GetCollection<BookModel>(_config["Collection"]);
        }

        [FunctionName(nameof(GetAllBooks))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Books")] HttpRequest req)
        {
           
            IActionResult returnValue = null;

            try
            {
                var result = await _books.Find(album => true).ToListAsync();

                if (result == null)
                {
                    _logger.LogInformation($"There are no books in the collection");
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(result);
                }
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

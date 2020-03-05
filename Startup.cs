
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SampleFunctionsApp;
using System.Linq;

[assembly: WebJobsStartup(typeof(Startup))]
namespace SampleFunctionsApp
{
    public class Startup : IWebJobsStartup
    { 
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });


            var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(config["MONGO_CONNECTION_STRING"]);

                return client;
            });
        }
    }
}

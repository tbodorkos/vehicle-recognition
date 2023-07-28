using Microsoft.ML;
using Functions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(new MLContext());
            builder.Services.AddSingleton<ITFModelScorer, TFModelScorer>();
        }
    }
}

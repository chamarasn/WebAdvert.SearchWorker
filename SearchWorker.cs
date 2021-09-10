using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using System;
using Nest;
using Newtonsoft.Json;
using AdvertApi.Models.Messages;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace WebAdvert.SearchWorker
{
    public class SearchWorker
    {
        public SearchWorker() : this(ElasticSearchHelper.GETInstance(ConfigurationHelper.Instance))
        {
        }
        private readonly IElasticClient _client;
        public SearchWorker(IElasticClient client)
        {
            _client = client;
        }

        public async Task Function(SNSEvent snsEvent, ILambdaContext context)
        {
            foreach(var record in snsEvent.Records)
            {
                context.Logger.LogLine(record.Sns.Message);

                var message = JsonConvert.DeserializeObject<AdvertConfirmedMessage>(record.Sns.Message);
                var advertDocument = MappingHelper.Map(message);

                var con = ConfigurationHelper.Instance;
                var url = con.GetSection("ES").GetValue<string>("url");

                //_client.Indices
                //    .Create("advert", s => s
                //        .Settings(se => se
                //            .NumberOfReplicas(1)
                //            .NumberOfShards(1)
                //            .Setting("merge.policy.merge_factor", "10")));


               // var resp =  _client.IndexDocument(advertDocument);

                

                await _client.IndexDocumentAsync(advertDocument);
            }


        }
    }
}

using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaftCore;
using RaftCore.Connections;
using RaftCore.Connections.Implementations;
using System.IO;

namespace NodeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Configure();
            var conf = Configure();
            var node = StartNode(conf);
            StartKestrel(conf, node);
            Logger.Log($"Node {conf.CurrentNode.Id} started");
        }

        private static Configuration Configure()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("Nodes.json");
            var root = builder.Build();
            Configuration configuration = new Configuration();
            root.Bind(configuration);
            return configuration;
        }

        private static void StartKestrel(Configuration conf, RaftNode node)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.Add( ServiceDescriptor.Singleton(typeof(RaftNode),node));
                }
                )
                .UseUrls( conf.CurrentNode.BaseUrl)
                .Build();

            host.Run();
        }

        private static RaftNode StartNode(Configuration conf)
        {
            var id = conf.CurrentNode.Id;
            var node = new RaftNode(id, new RaftCore.StateMachine.Implementations.NumeralStateMachine());
            var clusterConfiguration = new RaftCluster();
            foreach (var anotherNode in conf.Nodes)
            {
                clusterConfiguration.AddNode(new APIRaftConnector(anotherNode.Id, anotherNode.BaseUrl+"/RaftApi"));
            }
            node.Configure(clusterConfiguration);
            node.Run();
            return node;
        }
    }
}

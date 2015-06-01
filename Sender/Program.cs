using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Transports.SQLServer;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NServiceBus.Persistence;

class Program
{
    static void Main()
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
        Random random = new Random();
        BusConfiguration busConfiguration = new BusConfiguration();

        Configuration hibernateConfig = new Configuration();
        hibernateConfig.DataBaseIntegration(x =>
        {
            x.ConnectionStringName = "NServiceBus/Persistence";
            x.Dialect<MsSql2012Dialect>();
        });
        hibernateConfig.SetProperty("default_schema", "dbo");

        #region SenderConfiguration
        //sql transport is used 
        busConfiguration.UseTransport<SqlServerTransport>()
            .DefaultSchema("dbo")
            .UseSpecificConnectionInformation(
                EndpointConnectionInfo.For("receiver")
                    .UseSchema("dbo"));
        //data is persisted using the nhibernate 
        busConfiguration.UsePersistence<NHibernatePersistence>()
            .UseConfiguration(hibernateConfig);

        #endregion

        IBus bus = Bus.Create(busConfiguration).Start();
        while (true)
        {
            Console.WriteLine("Press <enter> to send a message");
            Console.ReadLine();

            //generate a random order id 
            string orderId = new string(Enumerable.Range(0, 4).Select(x => letters[random.Next(letters.Length)]).ToArray());
            //publish a message to receiver
            bus.Publish(new OrderSubmitted
            {
                OrderId = orderId,
                Value = random.Next(100)
            });
        }
    }
}
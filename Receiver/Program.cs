using System;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBus.Transports.SQLServer;

class Program
{
    static void Main()
    {
        Configuration hibernateConfig = new Configuration();
        hibernateConfig.DataBaseIntegration(x =>
        {
            x.ConnectionStringName = "NServiceBus/Persistence";
            x.Dialect<MsSql2012Dialect>();
        });

        #region NHibernate
        //use the default schema 
        hibernateConfig.SetProperty("default_schema", "dbo");

        #endregion

        ModelMapper mapper = new ModelMapper();
        mapper.AddMapping<OrderMap>();
        hibernateConfig.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

        new SchemaExport(hibernateConfig).Execute(false, true, false);

        #region ReceiverConfiguration

        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.UseTransport<SqlServerTransport>()
            .DefaultSchema("dbo")
            .UseSpecificConnectionInformation(endpoint =>
            {
                if (endpoint == "error")
                {
                    return ConnectionInfo.Create().UseSchema("dbo");
                }
                if (endpoint == "audit")
                {
                    return ConnectionInfo.Create().UseSchema("dbo");
                }
                string schema = endpoint.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();
                return ConnectionInfo.Create().UseSchema("dbo");
            });
        busConfiguration.UsePersistence<NHibernatePersistence>()
            .UseConfiguration(hibernateConfig)
            .RegisterManagedSessionInTheContainer();
        #endregion

        using (Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }
    }
}
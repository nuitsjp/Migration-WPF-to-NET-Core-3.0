using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Windows;
using AdventureWorks.EmployeeManager.DatabaseAccesses;
using AdventureWorks.EmployeeManager.Presentation.ViewModels;
using AdventureWorks.EmployeeManager.Services;
using AdventureWorks.EmployeeManager.Services.Imple;
using AutoMapper;
using RedSheeps.SimpleInjector.DynamicProxy;
using SimpleInjector;

namespace AdventureWorks.EmployeeManager.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static Container Container { get; set; } = new Container();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitMapper();
            BuildContainer();
        }

        private static void InitMapper()
        {
            Mapper.Initialize(config =>
            {
                CreateToWayMap<ManagedEmployee, ManagedEmployeeViewModel>(config);
                CreateToWayMap<ManagedEmployee, DatabaseAccesses.ManagedEmployee>(config);
                config.CreateMap<DatabaseAccesses.Gender, Gender>();
                config.CreateMap<MaritalStatu, MaritalStatus>();
            });
        }

        private static void CreateToWayMap<TLeft, TRight>(IProfileExpression config)
        {
            config.CreateMap<TLeft, TRight>();
            config.CreateMap<TRight, TLeft>();
        }

        private static void BuildContainer()
        {
            Container.Intercept<IHumanResourcesService>(
                typeof(AntiDeadlockInterceptor), typeof(TransactionInterceptor));

            // ViewModel
            Container.Register<MainWindowViewModel>();

            // Services
            TransactionContext.SetOpenConnection(OpenConnection);
            Container.Register<ITransactionContext, TransactionContext>(Lifestyle.Singleton);
            Container.Register<IHumanResourcesService, HumanResourcesService>();

            // DatabaseAccesses
            Container.Register<GenderDao>();
            Container.Register<MaritalStatusDao>();
            Container.Register<BusinessEntityDao>();
            Container.Register<PersonDao>();
            Container.Register<EmployeeDao>();
            Container.Register<ManagedEmployeeDao>();

            Container.Verify();
        }

        private static IDbConnection OpenConnection()
        {
            var settings = ConfigurationManager.ConnectionStrings["AdventureWorks2017"];
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            var connection = factory.CreateConnection();
            // ReSharper disable once PossibleNullReferenceException
            connection.ConnectionString = settings.ConnectionString;
            connection.Open();

            return connection;
        }
    }
}

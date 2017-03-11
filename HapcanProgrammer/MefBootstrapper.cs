using Caliburn.Micro;
using HapcanProgrammer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition.Registration;
using System.Reflection;
using System.IO;

namespace HapcanProgrammer
{
    public class MefBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;

        public MefBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new CompositionContainer( new AggregateCatalog( AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>() ));

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());

            assemblies.Add(Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onixarts.Hapcan.dll")));

            foreach (var path in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugins\"), "*.dll"))
                assemblies.Add( Assembly.LoadFrom(path));

            return assemblies;
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}

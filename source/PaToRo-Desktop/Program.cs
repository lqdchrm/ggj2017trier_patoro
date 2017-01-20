using System;
using System.Threading.Tasks;

namespace PaToRo_Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {

            //var team1 = AppDomain.CreateDomain("Team1");
            //var t = team1.CreateInstanceAndUnwrap(typeof(PaToRoGame).Assembly.GetName().FullName, typeof(PaToRoGame).FullName);

            var expander = new AppDomainExpander<StartHook>();
            Task.WaitAll(
                Task.Run(() => expander.Create("Team1")),
                Task.Run(() => expander.Create("Team2"))
            );

        }
    }


    internal class StartHook : DomainLifetimeHook
    {
        internal override void Start()
        {
            using (var game = new PaToRoGame())
                game.Run();
        }

        internal override void Stop()
        {

        }
    }

    abstract class DomainLifetimeHook { internal abstract void Start(); internal abstract void Stop(); }

    class AppDomainExpander<T> where T : DomainLifetimeHook, new()
    {
        public void Create() { Create(Guid.NewGuid().ToString()); }
        public void Create(string domainName)
        {
            AppDomain dmn = AppDomain.CreateDomain(domainName);
            string typename = typeof(DomainCommunicator<T>).FullName;
            string assemblyName = typeof(DomainCommunicator<T>).Assembly.FullName;
            var inner = (DomainCommunicator<T>)dmn.CreateInstanceAndUnwrap(assemblyName, typename);
            inner.Create();
        }
    }


    class DomainCommunicator<T> : MarshalByRefObject where T : DomainLifetimeHook, new()
    {
        T domainHook;
        public void Create()
        {
            domainHook = new T();
            // Attaching the handler is enough to keep a reference to ourselves
            // which in turn keeps T alive...
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(OnDomainUnload);
            domainHook.Start();
        }

        void OnDomainUnload(object sender, EventArgs e)
        {
            domainHook.Stop();
            // ...until the Domain dies: dereference myself to be more explicit
            AppDomain.CurrentDomain.DomainUnload -= new EventHandler(OnDomainUnload);
        }
    }
}

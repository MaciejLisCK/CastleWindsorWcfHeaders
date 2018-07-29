using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CastleWindsorWcfHeaders.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace CastleWindsorWcfHeaders.Client
{
    class Program
    {
        public static string ServiceUrl = "http://localhost:60260/Service1.svc";

        public static WindsorContainer Container;
        static void Main(string[] args)
        {
            Container = new WindsorContainer();
            Container.AddFacility<WcfFacility>();
            Container.Register(Component
                .For<IService1>()
                .AsWcfClient(
                    WcfEndpoint.BoundTo(new BasicHttpBinding())
                    .At(ServiceUrl)
                    .AddExtensions(new LangCodeHeaderBehavior())
                ));

            var service1 = Container.Resolve<IService1>();

            var result = service1.GetData(5);

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }

    internal class LangCodeHeaderBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new LangCodeHeaderInspector());
        }
    }

    internal class LangCodeHeaderInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var langCodeHeader = MessageHeader.CreateHeader("LangCode", String.Empty, "PL");
            request.Headers.Add(langCodeHeader);

            return null;
        }
    }
}

using Castle.Facilities.WcfIntegration;
using Castle.Facilities.WcfIntegration.Behaviors;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CastleWindsorWcfHeaders.Service;
using System;
using System.Collections;
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
using System.Xml;

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
            Container.Register(
                Component.For<MessageLifecycleBehavior>(),
                Component.For<IService1>()
                .AsWcfClient(
                    WcfEndpoint.BoundTo(new BasicHttpBinding())
                    .At(ServiceUrl)
                    #region Select only one option
                    //.AddExtensions(new AddLangCodeHeaderBehavior()) // Option 1
                    .AddExtensions(new AddLangCodeHeader()) // Option 2
                    #endregion
                ));

            var service1 = Container.Resolve<IService1>();

            var result = service1.GetData(5);

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }

    #region Option 1. With Wcf behaviour
    internal class AddLangCodeHeaderBehavior : IEndpointBehavior
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
    #endregion

    #region Option 2. With Castle AbstractMessageAction
    public class AddLangCodeHeader : AbstractMessageAction
    {
        public AddLangCodeHeader()
            : base(MessageLifecycle.OutgoingMessages)
        {

        }

        public override bool Perform(ref Message message, MessageLifecycle lifecycle, IDictionary state)
        {
            MessageHeader header = MessageHeader.CreateHeader("LangCode", String.Empty, "PL");
            message.Headers.Add(header);

            return true;
        }
    }
    #endregion
}

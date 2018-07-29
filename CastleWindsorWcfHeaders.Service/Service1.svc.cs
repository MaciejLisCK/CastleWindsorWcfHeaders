using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CastleWindsorWcfHeaders.Service
{
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            var langCode = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("LangCode", String.Empty);

            return $"Value: {value}, LangCode: {langCode}";
        }
    }
}

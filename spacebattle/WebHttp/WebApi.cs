// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreWCF;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public string PostOrder(OrderContract input){
            IoC.Resolve<ICommand>("Server.Commands.SendCommand",
                IoC.Resolve<Guid>("GetThreadByGameId",input.GameID),
                    IoC.Resolve<ICommand>("GetOrder",input)).Execute();
        var response = "Code 202 - Accepted";
        return response;
        }

    }
}

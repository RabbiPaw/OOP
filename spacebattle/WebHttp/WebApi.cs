// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Hwdtech;
using System;
using CoreWCF;
using System.Collections.Generic;

namespace SpaceBattle.Lib.WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public string PostOrder(OrderContract input)
        {
            try
            {
                var ServerThreadId = (Guid)IoC.Resolve<object>("Server.Commands.TryGetServerIdByGameId", input.GameId);

                IoC.Resolve<ICommand>("Server.Commands.SendCommand"
                    , ServerThreadId,
                       IoC.Resolve<ICommand>("CreateOrderCmd", input)).Execute();
                return "Code 202 - Accepted";
            }
            catch (Exception)
            {
                return "Code 400 - Not found your game";
            }
        }
    }
}

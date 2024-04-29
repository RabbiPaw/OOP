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
            try{
            var ThreadId = IoC.Resolve<List<Guid>>("Server.Command.GetThreadId", input.GameId)[0];
            var response = IoC.Resolve<string>("Server.Command.CreatResponse",input.GameId,input.OrderType,input.ObjectId);
            
            IoC.Resolve<ICommand>("Server.Commands.SendCommand",ThreadId,IoC.Resolve<ICommand>("CreatOrderCmd", input)).Execute();

            return response;
            }
            catch (KeyNotFoundException)
            {
                var response = "Code 400 - Entered GameId don't exist";
                return response;
            }
            catch (ArgumentException){
                var response = "Code 400 - Don't have OrderType";
                return response;
            }
            catch(IndexOutOfRangeException){
                var response = "Code 400 - Entered ObjectId don't exist";
                return response;
            }
        }
    }
}
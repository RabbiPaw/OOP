// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Hwdtech;
using System;
using CoreWCF;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Linq;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public string PostOrder(OrderContract input){
            try{
                if (input.OrderType == null){
                    throw new ArgumentException();
                    }
                if (input.ObjectId == null){
                    throw new IndexOutOfRangeException();
                }
                IoC.Resolve<ICommand>("Server.Commands.SendCommand",
                    IoC.Resolve<List<Guid>>("Server.Command.GetThreadId", input.GameId)[0],
                        IoC.Resolve<ICommand>("CreatOrderCmd",input)).Execute();
                var response = "Code 202 - Accepted " + input.GameId;
                return response;
                    
                }
            catch(KeyNotFoundException){
                var response = "Code 400 - Entered GameId don't exist";
                return response;
                }
            catch(ArgumentException)
            {
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
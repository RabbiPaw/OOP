// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using CoreWCF.OpenApi.Attributes;
using System.Diagnostics.CodeAnalysis;

[assembly: InternalsVisibleTo("SpaceBattle.Lib.Tests")]
namespace WebHttp
{
    [DataContract(Name = "OrderContract", Namespace = "http://order.com")]
    [ExcludeFromCodeCoverage]
    internal class OrderContract
    {
        [DataMember(Name = "OrderType", Order = 1)]
        [OpenApiProperty(Description = "OrderType")]
        public required string OrderType { get; set; }

        [DataMember(Name = "GameId", Order = 2)]
        [OpenApiProperty(Description = "GameId")]
        public required Guid GameId { get; set; }

        [DataMember(Name = "ObjectId", Order = 3)]
        [OpenApiProperty(Description = "ObjectId")]
        public required object ObjectId { get; set; }

        [DataMember(Name = "Properties", Order = 4)]
        [OpenApiProperty(Description = "Properties")]
        public Dictionary<string, object> Properties { get; set; }
    };
}

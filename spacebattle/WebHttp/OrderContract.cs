// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace WebHttp
{
    [DataContract(Name = "OrderContract", Namespace = "http://example.com")]
    internal class ExampleContract
    {
        [DataMember(Name = "OrderType", Order = 1)]
        [OpenApiProperty(Description = "OrderType")]
        public required string OrderType { get; set; }

        [DataMember(Name = "GameID", Order = 2)]
        [OpenApiProperty(Description = "GameID")]
        public required Guid GameID { get; set; }

        [DataMember(Name = "ObjectID", Order = 3)]
        [OpenApiProperty(Description = "ObjectID")]
        public required string ObjectID { get; set; }


        [DataMember(Name = "Properties", Order = 4)]
        [OpenApiProperty(Description = "Properties")]
        public IDictionary<string, object> Properties { get; set; }
    }
}

using Common.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.MapEvents.Messages
{
    [ProtoContract(SkipConstructor = true)]
    internal class NetworkMapEventSidesArrayUpdated : ICommand
    {
        [ProtoMember(1)]
        public string MapEventId { get; }
        [ProtoMember(2)]
        public string MapEventSideId { get; }
        [ProtoMember(3)]
        public int Index { get; }

        public NetworkMapEventSidesArrayUpdated(string mapEventId, string mapEventSideId, int index)
        {
            MapEventId = mapEventId;
            MapEventSideId = mapEventSideId;
            Index = index;
        }
    }
}

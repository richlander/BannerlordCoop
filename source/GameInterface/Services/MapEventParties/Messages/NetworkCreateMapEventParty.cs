using Common.Messaging;
using ProtoBuf;

namespace GameInterface.Services.MapEventParties.Messages
{
    [ProtoContract(SkipConstructor = true)]
    internal record NetworkCreateMapEventParty : ICommand
    {
        [ProtoMember(1)]
        public string Id { get; }

        public NetworkCreateMapEventParty(string id)
        {
            Id = id;
        }
    }
}

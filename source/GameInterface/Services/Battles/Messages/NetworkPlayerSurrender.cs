using Common.Messaging;
using ProtoBuf;

namespace GameInterface.Services.Battles.Messages
{
    [ProtoContract(SkipConstructor = true)]
    internal class NetworkPlayerSurrender : ICommand
    {
        [ProtoMember(1)]
        public string PlayerPartyId { get; }
        [ProtoMember(2)]
        public string CaptorPartyId { get; }
        [ProtoMember(3)]
        public string CharacterId { get; }

        public NetworkPlayerSurrender(string playerPartyId, string captorPartyId, string characterId)
        {
            PlayerPartyId = playerPartyId;
            CaptorPartyId = captorPartyId;
            CharacterId = characterId;
        }
    }
}

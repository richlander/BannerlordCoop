using Common.Messaging;

namespace GameInterface.Services.Battles.Messages
{
    internal class PlayerSurrender : IEvent
    {
        public string PlayerPartyId { get; }
        public string CaptorPartyId { get; }
        public string CharacterId { get; }

        public PlayerSurrender(string playerPartyId, string captorPartyId, string characterId)
        {
            PlayerPartyId = playerPartyId;
            CaptorPartyId = captorPartyId;
            CharacterId = characterId;
        }
    }
}

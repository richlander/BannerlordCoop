using Common.Logging;
using Common.Messaging;
using Common.Network;
using GameInterface.Services.Battles.Messages;
using GameInterface.Services.MobileParties.Patches;
using GameInterface.Services.ObjectManager;
using Serilog;
using System;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Battles.Handlers
{
    internal class PlayerSurrenderHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly INetwork network;

        private static readonly ILogger Logger = LogManager.GetLogger<PlayerSurrenderHandler>();

        public PlayerSurrenderHandler(IMessageBroker messageBroker, IObjectManager objectManager, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            this.network = network;
            messageBroker.Subscribe<PlayerSurrender>(Handle);
            messageBroker.Subscribe<NetworkPlayerSurrender>(Handle);
        }
        public void Dispose()
        {
            messageBroker.Unsubscribe<PlayerSurrender>(Handle);
            messageBroker.Unsubscribe<NetworkPlayerSurrender>(Handle);
        }

        private void Handle(MessagePayload<PlayerSurrender> payload)
        {
            var obj = payload.What;
            NetworkPlayerSurrender message = new NetworkPlayerSurrender(obj.PlayerPartyId, obj.CaptorPartyId, obj.CharacterId);
            network.SendAll(message);
        }

        private void Handle(MessagePayload<NetworkPlayerSurrender> payload)
        {
            var obj = payload.What;
            objectManager.TryGetObject(obj.CaptorPartyId, out MobileParty captorParty);
            PlayerSurrenderPatch.RunStartPlayerCaptivity(captorParty.Party);
        }
    }
}

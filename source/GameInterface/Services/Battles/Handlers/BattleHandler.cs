using Common.Logging;
using Common.Messaging;
using Common.Network;
using GameInterface.Services.Battles.Messages;
using GameInterface.Services.MobileParties.Handlers;
using GameInterface.Services.MobileParties.Patches;
using GameInterface.Services.ObjectManager;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Battles.Handlers
{
    internal class BattleHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly INetwork network;

        private static readonly ILogger Logger = LogManager.GetLogger<BattleHandler>();

        public BattleHandler(IMessageBroker messageBroker, IObjectManager objectManager, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            this.network = network;
            messageBroker.Subscribe<BattleStarted>(Handle);
            messageBroker.Subscribe<NetworkStartBattle>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<BattleStarted>(Handle);
            messageBroker.Unsubscribe<NetworkStartBattle>(Handle);
        }

        private void Handle(MessagePayload<BattleStarted> payload)
        {
            network.SendAll(new NetworkStartBattle(payload.What.AttackerId, payload.What.DefenderId));
        }

        private void Handle(MessagePayload<NetworkStartBattle> payload)
        {
            objectManager.TryGetObject(payload.What.AttackerId, out MobileParty attacker);
            objectManager.TryGetObject(payload.What.DefenderId, out MobileParty defender);
            EncounterManagerPatches.OverrideOnPartyInteraction(attacker, defender);
        }
    }
}

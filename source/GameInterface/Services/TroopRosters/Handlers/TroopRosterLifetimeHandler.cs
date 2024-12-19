using Common;
using Common.Logging;
using Common.Messaging;
using Common.Network;
using Common.Util;
using GameInterface.Services.Buildings.Handlers;
using GameInterface.Services.ObjectManager;
using GameInterface.Services.TroopRosters.Messages;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;

namespace GameInterface.Services.TroopRosters.Handlers
{
    internal class TroopRosterLifetimeHandler : IHandler
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TroopRosterLifetimeHandler>();
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly INetwork network;
        private static readonly ConstructorInfo TroopRoster_ctor = AccessTools.Constructor(typeof(TroopRoster));

        public TroopRosterLifetimeHandler(IMessageBroker messageBroker, IObjectManager objectManager, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            this.network = network;
            messageBroker.Subscribe<TroopRosterCreated>(Handle);
            messageBroker.Subscribe<NetworkCreateTroopRoster>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<TroopRosterCreated>(Handle);
            messageBroker.Unsubscribe<NetworkCreateTroopRoster>(Handle);
        }

        private void Handle(MessagePayload<TroopRosterCreated> obj)
        {
            var payload = obj.What;

            if (objectManager.AddNewObject(payload.TroopRoster, out string rosterId) == false) return;
            objectManager.TryGetId(payload.PartyBase, out string partyId);

            var message = new NetworkCreateTroopRoster(rosterId, partyId);
            network.SendAll(message);
        }

        private void Handle(MessagePayload<NetworkCreateTroopRoster> obj)
        {
            var payload = obj.What;

            var troopRoster = ObjectHelper.SkipConstructor<TroopRoster>();
            GameLoopRunner.RunOnMainThread(() =>
            {
                using (new AllowedThread())
                {
                    TroopRoster_ctor.Invoke(troopRoster, Array.Empty<object>());
                }
            });
            if (objectManager.AddExisting(payload.TroopRosterId, troopRoster) == false)
            {
                Logger.Error("Failed to add existing TroopRoster, {id}", payload.TroopRosterId);
                return;
            }
        }
    }
}

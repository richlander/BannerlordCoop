using Common.Logging;
using Common.Messaging;
using Common.Network;
using Common.Util;
using GameInterface.Services.MapEvents.Handlers;
using GameInterface.Services.MapEventSides.Messages;
using GameInterface.Services.ObjectManager;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Siege;

namespace GameInterface.Services.MapEventSides.Handlers
{
    internal class MapEventSideDataHandler : IHandler
    {
        private static readonly ILogger Logger = LogManager.GetLogger<MapEventSideDataHandler>();

        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;
        private readonly IObjectManager objectManager;

        public MapEventSideDataHandler(IMessageBroker messageBroker, INetwork network, IObjectManager objectManager)
        {
            this.messageBroker = messageBroker;
            this.network = network;
            this.objectManager = objectManager;

            messageBroker.Subscribe<MapEventPartyAdded>(Handle);
            messageBroker.Subscribe<MapEventPartyRemoved>(Handle);
            messageBroker.Subscribe<NetworkAddMapEventParty>(Handle);
            messageBroker.Subscribe<NetworkRemoveMapEventParty>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<MapEventPartyAdded>(Handle);
            messageBroker.Unsubscribe<MapEventPartyRemoved>(Handle);
            messageBroker.Unsubscribe<NetworkAddMapEventParty>(Handle);
            messageBroker.Unsubscribe<NetworkRemoveMapEventParty>(Handle);
        }

        private void Handle(MessagePayload<MapEventPartyRemoved> payload)
        {
            var data = payload.What;

            if (objectManager.TryGetId(data.MapEventSide, out string sideId) == false) return;
            if (objectManager.TryGetId(data.MapEventParty, out string partyId) == false) return;

            network.SendAll(new NetworkRemoveMapEventParty(sideId, partyId));
        }

        private void Handle(MessagePayload<MapEventPartyAdded> payload)
        {
            var data = payload.What;

            if (objectManager.TryGetId(data.MapEventSide, out string sideId) == false) return;
            if (objectManager.TryGetId(data.MapEventParty, out string partyId) == false) return;

            network.SendAll(new NetworkAddMapEventParty(sideId, partyId));
        }

        private void Handle(MessagePayload<NetworkRemoveMapEventParty> payload)
        {
            var data = payload.What;

            if (objectManager.TryGetObject<MapEventParty>(data.PartyId, out var party) == false)
            {
                Logger.Error("Unable to find {type} with id: {id}", typeof(MapEventParty), data.PartyId);
                return;
            }
            if (objectManager.TryGetObject<MapEventSide>(data.SideId, out var side) == false)
            {
                Logger.Error("Unable to find {type} with id: {id}", typeof(MapEventSide), data.SideId);
                return;
            }

            using(new AllowedThread())
            {
                side._battleParties.Remove(party);
            }
        }

        private void Handle(MessagePayload<NetworkAddMapEventParty> payload)
        {
            var data = payload.What;

            if (objectManager.TryGetObject<MapEventParty>(data.PartyId, out var party) == false)
            {
                Logger.Error("Unable to find {type} with id: {id}", typeof(MapEventParty), data.PartyId);
                return;
            }
            if (objectManager.TryGetObject<MapEventSide>(data.SideId, out var side) == false)
            {
                Logger.Error("Unable to find {type} with id: {id}", typeof(MapEventSide), data.SideId);
                return;
            }

            using (new AllowedThread())
            {
                side._battleParties.Add(party);
            }
        }
    }
}

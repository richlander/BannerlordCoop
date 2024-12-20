using Common;
using Common.Messaging;
using Common.Network;
using Common.Util;
using GameInterface.Services.MapEvents.Messages;
using GameInterface.Services.ObjectManager;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.MapEvents.Handlers
{
    internal class MapEventDataHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly INetwork network;
        private readonly IObjectManager objectManager;

        public MapEventDataHandler(IMessageBroker messageBroker, INetwork network, IObjectManager objectManager)
        {
            this.messageBroker = messageBroker;
            this.network = network;
            this.objectManager = objectManager;

            messageBroker.Subscribe<MapEventSidesArrayUpdated>(Handle);
            messageBroker.Subscribe<NetworkMapEventSidesArrayUpdated>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<MapEventSidesArrayUpdated>(Handle);
            messageBroker.Unsubscribe<NetworkMapEventSidesArrayUpdated>(Handle);
        }

        private void Handle(MessagePayload<MapEventSidesArrayUpdated> payload)
        {
            var data = payload.What;
            if (objectManager.TryGetId(data.Instance, out string MapEventId) == false) return;
            if (objectManager.TryGetId(data.Value, out string MapEventSideId) == false) return;

            var message = new NetworkMapEventSidesArrayUpdated(MapEventId, MapEventSideId, data.Index);

            network.SendAll(message);
        }
        private void Handle(MessagePayload<NetworkMapEventSidesArrayUpdated> payload)
        {
            var payloadData = payload.What;

            if (objectManager.TryGetObject<MapEvent>(payloadData.MapEventId, out var mapEvent) == false) return;
            if (objectManager.TryGetObject<MapEventSide>(payloadData.MapEventSideId, out var mapEventSide) == false) return;

            GameLoopRunner.RunOnMainThread(() =>
            {
                using (new AllowedThread())
                {
                    mapEvent._sides[payloadData.Index] = mapEventSide;
                }
            });
        }
    }
}

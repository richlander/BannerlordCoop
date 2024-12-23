using Common;
using Common.Logging;
using Common.Messaging;
using Common.Network;
using Common.Util;
using GameInterface.Services.Equipments.Messages.Events;
using GameInterface.Services.ObjectManager;
using GameInterface.Services.Equipments.Messages;
using Serilog;
using TaleWorlds.Core;
using System;
using GameInterface.Services.Equipments.Data;
using HarmonyLib;
using System.Reflection;
using System.Diagnostics;
using GameInterface.Services.Heroes.Messages.Collections;
using TaleWorlds.CampaignSystem;


namespace GameInterface.Services.Heroes.Handlers
{
    /// <summary>
    /// Handles all changes to Equipments on client.
    /// </summary>
    public class HeroCollectionHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly INetwork network;
        private readonly ILogger Logger = LogManager.GetLogger<HeroCollectionHandler>();

        public HeroCollectionHandler(IMessageBroker messageBroker, IObjectManager objectManager, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            this.network = network;
            messageBroker.Subscribe<VolunteerTypesArrayUpdated>(Handle);
            messageBroker.Subscribe<NetworkUpdateArray>(Handle);

            messageBroker.Subscribe<ChildrenListUpdated>(Handle);
            messageBroker.Subscribe<NetworkUpdateChildrenList>(Handle);
        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<VolunteerTypesArrayUpdated>(Handle);
            messageBroker.Unsubscribe<NetworkUpdateArray>(Handle);

            messageBroker.Unsubscribe<ChildrenListUpdated>(Handle);
            messageBroker.Unsubscribe<NetworkUpdateChildrenList>(Handle);
        }

        private void Handle(MessagePayload<VolunteerTypesArrayUpdated> payload)
        {
            var data = payload.What;

            if (!TryGetId(data.Instance, out string HeroId)) return;
            if (!TryGetId(data.Value, out string CharacterObjectId)) return;

            network.SendAll(new NetworkUpdateArray(HeroId, CharacterObjectId, data.Index));
        }

        private void Handle(MessagePayload<NetworkUpdateArray> payload)
        {
            var data = payload.What;

            if (!objectManager.TryGetObject(data.HeroId, out Hero hero)) return;
            if (!objectManager.TryGetObject(data.ValueId, out CharacterObject characterObject)) return;

            hero.VolunteerTypes[data.Index] = characterObject;
        }

        private void Handle(MessagePayload<ChildrenListUpdated> payload)
        {
            var data = payload.What;

            if (!TryGetId(data.Instance, out string HeroId)) return;
            if (!TryGetId(data.Value, out string ChildId)) return;

            network.SendAll(new NetworkUpdateChildrenList(HeroId, ChildId));
        }

        private void Handle(MessagePayload<NetworkUpdateChildrenList> payload)
        {
            var data = payload.What;

            if (!objectManager.TryGetObject(data.HeroId, out Hero hero)) return;
            if (!objectManager.TryGetObject(data.ValueId, out Hero child)) return;

            hero._children.Add(child);
        }

        private bool TryGetId(object value, out string id)
        {
            id = null;
            if (value == null) return false;

            if (!objectManager.TryGetId(value, out id))
            {
                Logger.Error("Unable to get ID for instance of type {type}", value.GetType());
                return false;
            }

            return true;
        }
    }
}


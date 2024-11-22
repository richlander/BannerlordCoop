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
using GameInterface.Services.MapEvents.Messages;


namespace GameInterface.Services.Equipments.Handlers
{
    /// <summary>
    /// Handles all changes to Equipments on client.
    /// </summary>
    public class ItemSlotHandler : IHandler
    {
        private readonly IMessageBroker messageBroker;
        private readonly IObjectManager objectManager;
        private readonly INetwork network;
        private readonly ILogger Logger = LogManager.GetLogger<EquipmentHandler>();
        private static readonly ConstructorInfo Equipment_ctor = AccessTools.Constructor(typeof(Equipment));
        private static readonly ConstructorInfo EquipmentParam_ctor = AccessTools.Constructor(typeof(Equipment), new Type[] { typeof(Equipment) });


        public ItemSlotHandler(IMessageBroker messageBroker, IObjectManager objectManager, INetwork network)
        {
            this.messageBroker = messageBroker;
            this.objectManager = objectManager;
            this.network = network;
            messageBroker.Subscribe<ItemSlotsArrayUpdated>(Handle);

        }

        public void Dispose()
        {
            messageBroker.Unsubscribe<ItemSlotsArrayUpdated>(Handle);

        }

        private void Handle(MessagePayload<ItemSlotsArrayUpdated> payload)
        {

        }
    }
}


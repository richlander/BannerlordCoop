﻿using Common.Messaging;
using Common.Network;
using HarmonyLib;
using Missions.Services.Agents.Messages;
using Missions.Services.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Missions.Services.Agents.Handlers
{
    public class WeaponPickupHandler
    {

        public WeaponPickupHandler()
        {
            NetworkMessageBroker.Instance.Subscribe<WeaponPickedup>(WeaponPickupSend);
            NetworkMessageBroker.Instance.Subscribe<NetworkWeaponPickedup>(WeaponPickupReceive);
        }
        ~WeaponPickupHandler()
        {
            NetworkMessageBroker.Instance.Unsubscribe<WeaponPickedup>(WeaponPickupSend);
            NetworkMessageBroker.Instance.Unsubscribe<NetworkWeaponPickedup>(WeaponPickupReceive);
        }

        private static MethodInfo WeaponEquippedMethod = typeof(Agent).GetMethod("WeaponEquipped", BindingFlags.NonPublic | BindingFlags.Instance);

        public void WeaponPickupSend(MessagePayload<WeaponPickedup> obj)
        {
            Agent agent = obj.Who as Agent;

            NetworkAgentRegistry.Instance.TryGetAgentId(agent, out Guid agentId);

            NetworkWeaponPickedup message = new NetworkWeaponPickedup(agentId, obj.What.EquipmentIndex, obj.What.WeaponObject, obj.What.WeaponModifier, obj.What.Banner);

            NetworkMessageBroker.Instance.PublishNetworkEvent(message);
        }
        public void WeaponPickupReceive(MessagePayload<NetworkWeaponPickedup> obj)
        {
            //ItemObject - ItemModifier - Banner creates MissionWeapon
            MissionWeapon missionWeapon = new MissionWeapon(obj.What.ItemObject, obj.What.ItemModifier, obj.What.Banner);

            NetworkAgentRegistry.Instance.TryGetAgent(obj.What.AgentId, out Agent agent);
            
            agent.Equipment[obj.What.EquipmentIndex] = missionWeapon;
            WeaponEquippedMethod.Invoke(agent, new object[]
                {
                obj.What.EquipmentIndex,
                missionWeapon.GetWeaponData(true),
                missionWeapon.GetWeaponStatsData(),
                missionWeapon.GetAmmoWeaponData(true),
                missionWeapon.GetAmmoWeaponStatsData(),
                null,
                false,
                false
                });
        }
    }
}

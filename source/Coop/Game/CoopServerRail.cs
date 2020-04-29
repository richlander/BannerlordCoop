﻿using System;
using System.Collections.Generic;
using Coop.Common;
using Coop.Game.Persistence;
using Coop.Multiplayer;
using Coop.Multiplayer.Network;
using Coop.Network;
using RailgunNet;
using RailgunNet.Connection.Server;

namespace Coop.Game
{
    public class CoopServerRail : IUpdateable
    {
        public CoopServerRail(Server server)
        {
            m_Server = server;
            m_Instance = new RailServer(Registry.Get(Component.Server));
            RailGameConfigurator.SetInstanceAs(Component.Server);
            RailPopulator.Populate(m_Instance.StartRoom());
            m_Server.Updateables.Add(this);
        }

        ~CoopServerRail()
        {
            m_Server.Updateables.Remove(this);
        }

        public void Update(TimeSpan frameTime)
        {
            m_Instance.Update();
        }
        public void ClientJoined(ConnectionServer connection)
        {
            var peer = new RailNetPeerWrapper(connection.Network);
            m_RailConnections.Add(connection, peer);
            m_Instance.AddClient(peer, ""); // TODO: Name
        }
        public void Disconnected(ConnectionServer connection)
        {
            if (m_RailConnections.ContainsKey(connection))
            {
                m_Instance.RemoveClient(m_RailConnections[connection]);
            }
        }

        private readonly Dictionary<ConnectionServer, RailNetPeerWrapper> m_RailConnections = new Dictionary<ConnectionServer, RailNetPeerWrapper>();
        private readonly Server m_Server;
        private readonly RailServer m_Instance;
    }
}

﻿using System;
using System.Diagnostics;
using Coop.Network;
using LiteNetLib;
using RailgunNet.Connection.Traffic;

namespace Coop.Multiplayer.Network
{
    public class RailNetPeerWrapper  : IRailNetPeer, IGameStatePersistence
    {
        private readonly INetworkConnection m_Connection;
        public event EventHandler OnDisconnected;
        public RailNetPeerWrapper(INetworkConnection connection)
        {
            m_Connection = connection;
        }

        [Conditional("DEBUG")]private static void AssertIsPersistencePayload(ArraySegment<byte> buffer)
        {
            Protocol.EPacket eType = PacketReader.DecodePacketType(buffer.Array[buffer.Offset]);
            if (eType != Protocol.EPacket.Persistence)
            {
                throw new ArgumentException(nameof(buffer));
            }
        }
        public void Receive(ArraySegment<byte> buffer)
        {
            AssertIsPersistencePayload(buffer);
            PayloadReceived?.Invoke(this, new ArraySegment<byte>(buffer.Array, buffer.Offset + 1, buffer.Count - 1));
        }

        public void Disconnect()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        #region IRailNetPeer
        public object PlayerData { get; set; }

        public float? Ping => m_Connection.Latency;

        public event RailNetPeerEvent PayloadReceived;

        public void SendPayload(ArraySegment<byte> buffer)
        {
            // TODO: Remove this copy
            byte[] toSend = new byte[buffer.Count + 1];
            toSend[0] = PacketWriter.EncodePacketType(Protocol.EPacket.Persistence);
            Array.Copy(buffer.Array, 0, toSend, 1, buffer.Count);
            m_Connection.SendRaw(new ArraySegment<byte>(toSend));
        }
        #endregion
    }
}

﻿using Common.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coop.Core.Client.Services.MobileParties.Messages
{
    /// <summary>
    /// Request to recruit troops from client
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public record NetworkTroopIndexAddRequest : ICommand
    {
        [ProtoMember(1)]
        public string PartyId;
        [ProtoMember(2)]
        public bool IsPrisonerRoster;
        [ProtoMember(3)]
        public int Index;
        [ProtoMember(4)]
        public int CountChange;
        [ProtoMember(5)]
        public int WoundedCountChange;
        [ProtoMember(6)]
        public int XpChange;
        [ProtoMember(7)]
        public bool RemoveDepleted;

        public NetworkTroopIndexAddRequest(string partyId, bool isPrisonerRoster, int index, int countChange, int woundedCountChange, int xpChange, bool removeDepleted)
        {
            PartyId = partyId;
            IsPrisonerRoster = isPrisonerRoster;
            Index = index;
            CountChange = countChange;
            WoundedCountChange = woundedCountChange;
            XpChange = xpChange;
            RemoveDepleted = removeDepleted;
        }
    }
}

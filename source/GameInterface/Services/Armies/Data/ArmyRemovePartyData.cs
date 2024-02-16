﻿using ProtoBuf;

namespace GameInterface.Services.Armies.Data;
[ProtoContract(SkipConstructor = true)]
public record ArmyRemovePartyData
{
    public ArmyRemovePartyData(string armyStringId, string partyStringId)
    {
        ArmyStringId = armyStringId;
        PartyStringId = partyStringId;
    }

    [ProtoMember(1)]
    public string ArmyStringId { get; set; }
    [ProtoMember(2)]
    public string PartyStringId { get; set; }

}

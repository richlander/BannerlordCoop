﻿using Common.Messaging;
using GameInterface.Services.Towns.Data;
using System.Collections.Generic;

namespace GameInterface.Services.Towns.Messages
{
    /// <summary>
    /// Used by the town auditor debug command to send all sync value from client to server only
    /// </summary>
    public record class TownAuditorSent : ICommand
    {

        public TownAuditorData[] Datas { get; }

        public TownAuditorSent(List<TownAuditorData> townAuditorDatas)
        {
            Datas = townAuditorDatas.ToArray();
        }
    }
}

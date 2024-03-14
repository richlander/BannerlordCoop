﻿using ProtoBuf.Meta;
using TaleWorlds.Library;

namespace GameInterface.Surrogates;

/// <summary>
/// Collection of ProtoBuf surrogates
/// </summary>
public interface ISurrogateCollection { }

/// <inheritdoc cref="ISurrogateCollection"/>
internal class SurrogateCollection : ISurrogateCollection
{
    public SurrogateCollection()
    {
        RuntimeTypeModel.Default.Add(typeof(Vec2), false).SetSurrogate(typeof(Vec2Surrogate));
    }
}

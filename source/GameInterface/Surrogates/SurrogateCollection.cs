using ProtoBuf.Meta;
using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace GameInterface.Surrogates;

public interface ISurrogateCollection { }

internal class SurrogateCollection : ISurrogateCollection
{
    public SurrogateCollection()
    {
        AddSurrogate<Vec2, Vec2Surrogate>();
        AddSurrogate<TextObject, TextObjectSurrogate>();
    }

    private void AddSurrogate<T, TSurrogate>()
    {
        try
        {
            RuntimeTypeModel.Default.SetSurrogate<T, TSurrogate>();
        }
        catch (InvalidOperationException) { }
        catch (ArgumentException) { }
    }
}

using ProtoBuf.Meta;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
        AddSurrogate<Banner, BannerSurrogate>();
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

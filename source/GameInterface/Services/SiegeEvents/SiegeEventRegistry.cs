using GameInterface.Services.Registry;
using System.Linq;
using System.Threading;
using TaleWorlds.CampaignSystem.Siege;

namespace GameInterface.Services.SiegeEvents;


/// <summary>
/// Registry manager for SiegeEvent
/// </summary>
internal class SiegeEventRegistry : RegistryBase<SiegeEvent>
{
	private const string SiegeEventIdPrefix = "CoopSiegeEvent";
	private static int InstanceCounter = 0;

	public SiegeEventRegistry (IRegistryCollection collection) : base(collection)
	{
	}

	public override void RegisterAll()
	{
		// Implement RegisterAll if needed
	}

	protected override string GetNewId(SiegeEvent obj)
	{
		return $"{SiegeEventIdPrefix}_{Interlocked.Increment(ref InstanceCounter)}";
	}
}
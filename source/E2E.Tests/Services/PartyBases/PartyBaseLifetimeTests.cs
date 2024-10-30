using E2E.Tests.Environment;
using E2E.Tests.Util;
using HarmonyLib;
using System.Runtime.InteropServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using Xunit.Abstractions;

namespace E2E.Tests.Services.PartyBases;

public class PartyBasePropertyTests : IDisposable
{
    E2ETestEnvironment TestEnvironment { get; }

    public PartyBasePropertyTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

    [Fact]
    public void Server_MobileParty_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironment.Server;

        // Act
        string? partyBaseId = null;
        string? partyId = null;
        string? settlementId = null;
        string? itemRosterId = null;
        string? mapEventSideId = null;
        string? memberRosterId = null;
        string? prisonRosterId = null;
        string? heroId = null;

        var cachedPartyLimit = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._cachedPartyMemberSizeLimit));
        var cachedPrisonerLimit = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._cachedPrisonerSizeLimit));
        var cachedStrength = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._cachedTotalStrength));
        var customOwner = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._customOwner));
        var index = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._index));
        var lastEatingTime = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._lastEatingTime));
        var lastRosterVerNo = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._lastMemberRosterVersionNo));
        var lastMenPerTierVerNo = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._lastNumberOfMenPerTierVersionNo));
        var mapEventSideField = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._mapEventSide));
        var numberMenHorseField = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._numberOfMenWithHorse));
        var partySizeLastCheckField = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._partyMemberSizeLastCheckVersion));
        var prisonerSizeLastCheckField = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._prisonerSizeLastCheckVersion));
        var remainingFoodPercentageField = AccessTools.Field(typeof(PartyBase), nameof(PartyBase._remainingFoodPercentage));


        // Get field intercept to use on the server to simulate the field changing
        var mapEventSideIntercept = TestEnvironment.GetIntercept(mapEventSideField);
        var numberMenHorseIntercept = TestEnvironment.GetIntercept(numberMenHorseField);
        var partySizeLastCheckIntercept = TestEnvironment.GetIntercept(partySizeLastCheckField);
        var prisonerSizeLastCheckIntercept = TestEnvironment.GetIntercept(prisonerSizeLastCheckField);
        var indexIntercept = TestEnvironment.GetIntercept(index);
        var lastEatingTimeIntercept = TestEnvironment.GetIntercept(lastEatingTime);
        var lastRosterVerNoIntercept = TestEnvironment.GetIntercept(lastRosterVerNo);
        var lastMenPerTierVerNoIntercept = TestEnvironment.GetIntercept(lastMenPerTierVerNo);
        var cachedPartyLimitIntercept = TestEnvironment.GetIntercept(cachedPartyLimit);
        var cachedPrisonerLimitIntercept = TestEnvironment.GetIntercept(cachedPrisonerLimit);
        var cachedStrengthIntercept = TestEnvironment.GetIntercept(cachedStrength);
        var customOwnerIntercept = TestEnvironment.GetIntercept(customOwner);
        var remainingFoodPercentageIntercept = TestEnvironment.GetIntercept(remainingFoodPercentageField);

        server.Call(() =>
        {
            var party = GameObjectCreator.CreateInitializedObject<MobileParty>();
            var itemRoster = GameObjectCreator.CreateInitializedObject<ItemRoster>();
            var mapEventSide = GameObjectCreator.CreateInitializedObject<MapEventSide>();
            var settlement = GameObjectCreator.CreateInitializedObject<Settlement>();
            var troopRoster = GameObjectCreator.CreateInitializedObject<TroopRoster>();
            var hero = GameObjectCreator.CreateInitializedObject<Hero>();

            partyId = party.StringId;
            var partyBase = new PartyBase(default(MobileParty));

            // Simulate the field changing
            remainingFoodPercentageIntercept.Invoke(null, new object[] { partyBase, 5 });
            mapEventSideIntercept.Invoke(null, new object[] { partyBase, mapEventSide });
            numberMenHorseIntercept.Invoke(null, new object[] { partyBase, 5 });
            partySizeLastCheckIntercept.Invoke(null, new object[] { partyBase, 5 });
            prisonerSizeLastCheckIntercept.Invoke(null, new object[] { partyBase, 5 });
            indexIntercept.Invoke(null, new object[] { partyBase, 5 });
            lastEatingTimeIntercept.Invoke(null, new object[] { partyBase, new CampaignTime(5) });
            lastRosterVerNoIntercept.Invoke(null, new object[] { partyBase, 5 });
            lastMenPerTierVerNoIntercept.Invoke(null, new object[] { partyBase, 5 });
            cachedPartyLimitIntercept.Invoke(null, new object[] { partyBase, 5 });
            cachedPrisonerLimitIntercept.Invoke(null, new object[] { partyBase, 5 });
            cachedStrengthIntercept.Invoke(null, new object[] { partyBase, 5f });
            customOwnerIntercept.Invoke(null, new object[] { partyBase, hero });


            partyBase.MobileParty = party;
            partyBase.Settlement = settlement;
            partyBase.IsVisualDirty = true;
            partyBase.ItemRoster = itemRoster;
            partyBase.LevelMaskIsDirty = true;
            //partyBase.MapEventSide = mapEventSide;
            partyBase.MemberRoster = troopRoster;
            partyBase.PrisonRoster = troopRoster;
            partyBase.RandomValue = 5;

            Assert.True(server.ObjectManager.TryGetId(party.Party, out partyBaseId));
            Assert.True(server.ObjectManager.TryGetId(party, out settlementId));
            Assert.True(server.ObjectManager.TryGetId(party.ItemRoster, out itemRosterId));
            //Assert.True(server.ObjectManager.TryGetId(party.MapEventSide, out mapEventSideId));
            Assert.True(server.ObjectManager.TryGetId(party.MemberRoster, out memberRosterId));
            Assert.True(server.ObjectManager.TryGetId(party.PrisonRoster, out prisonRosterId));
            Assert.True(server.ObjectManager.TryGetId(hero, out heroId));
        });

        // Assert
        Assert.NotNull(partyBaseId);
        Assert.NotNull(partyId);
        Assert.NotNull(settlementId);
        Assert.NotNull(itemRosterId);
        //Assert.NotNull(mapEventSideId);
        Assert.NotNull(memberRosterId);
        Assert.NotNull(prisonRosterId);
        Assert.NotNull(heroId);

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<MobileParty>(partyId, out var clientParty));
            Assert.True(client.ObjectManager.TryGetObject<PartyBase>(partyBaseId, out var clientPartyBase));
            //Assert.True(client.ObjectManager.TryGetObject<Settlement>(settlementId, out var clientSettlement));
            Assert.True(client.ObjectManager.TryGetObject<ItemRoster>(itemRosterId, out var clientItemRoster));
            //Assert.True(client.ObjectManager.TryGetObject<MapEventSide>(mapEventSideId, out var clientMapEventSide));
            Assert.True(client.ObjectManager.TryGetObject<TroopRoster>(memberRosterId, out var clientMemberRoster));
            Assert.True(client.ObjectManager.TryGetObject<TroopRoster>(prisonRosterId, out var clientPrisonRoster));
            Assert.True(client.ObjectManager.TryGetObject<Hero>(heroId, out var clientHero));


            Assert.Equal(clientParty, clientPartyBase.MobileParty);
            Assert.True(clientPartyBase.IsVisualDirty);
            Assert.Equal(clientItemRoster, clientPartyBase.ItemRoster);
            //Assert.Equal(clientMapEventSide, clientPartyBase.MapEventSide);
            Assert.Equal(clientMemberRoster, clientPartyBase.MemberRoster);
            Assert.Equal(clientPrisonRoster, clientPartyBase.PrisonRoster);
            //Assert.Equal(5, clientPartyBase.RandomValue);
            //Assert.Equal(clientSettlement, clientPartyBase.Settlement);
            Assert.Equal(5, clientPartyBase._cachedPartyMemberSizeLimit);
            Assert.Equal(5, clientPartyBase._cachedPrisonerSizeLimit);
            Assert.Equal(5, clientPartyBase._cachedTotalStrength);
            Assert.Equal(clientHero, clientPartyBase._customOwner);
            Assert.Equal(5, clientPartyBase._index);
            Assert.Equal(new CampaignTime(5), clientPartyBase._lastEatingTime);
            Assert.Equal(5, clientPartyBase._lastMemberRosterVersionNo);
            Assert.Equal(5, clientPartyBase._lastNumberOfMenPerTierVersionNo);
            Assert.Equal(5, clientPartyBase._lastNumberOfMenWithHorseVersionNo);
            //Assert.Equal(clientMapEventSide, clientPartyBase._mapEventSide);
            Assert.Equal(5, clientPartyBase._numberOfMenWithHorse);
            Assert.Equal(5, clientPartyBase._partyMemberSizeLastCheckVersion);
            Assert.Equal(5, clientPartyBase._prisonerSizeLastCheckVersion);
            Assert.Equal(5, clientPartyBase._remainingFoodPercentage);
        }
    }

    [Fact]
    public void Client_MobileParty_DoesNothing()
    {
        // Arrange
        var server = TestEnvironment.Server;

        // Act
        string? partyBaseId = null;

        var firstClient = TestEnvironment.Clients.First();
        firstClient.Call(() =>
        {
            var party = new PartyBase(default(MobileParty));

            Assert.False(server.ObjectManager.TryGetId(party, out partyBaseId));
        });

        // Assert
        Assert.Null(partyBaseId);
    }
}


using Autofac;
using Common.Messaging;
using E2E.Tests.Environment;
using E2E.Tests.Util;
using GameInterface.Services.Armies.Messages.Lifetime;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using Xunit.Abstractions;

namespace E2E.Tests.Services.Armies;

public class ArmyCreationTests : IDisposable
{
    E2ETestEnvironment TestEnvironment { get; }
    public ArmyCreationTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

    [Fact]
    public void ServerCreateArmy_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironment.Server;

        var kingdom = new Kingdom();
        var hero = GameObjectCreator.CreateInitializedObject<Hero>();
        var settlement = GameObjectCreator.CreateInitializedObject<Settlement>();
        var serverMessageBroker = server.Container.Resolve<IMessageBroker>();

        SetupKingdom(kingdom, hero, settlement);

        server.ObjectManager.AddNewObject(kingdom, out string kingdomStringId);
        server.ObjectManager.AddNewObject(hero, out string heroStringId);
        server.ObjectManager.AddNewObject(hero.PartyBelongedTo, out string partyStringId);

        foreach (var client in TestEnvironment.Clients)
        {
            client.ObjectManager.AddExisting(kingdomStringId, kingdom);
            client.ObjectManager.AddExisting(heroStringId, hero);
            client.ObjectManager.AddExisting(partyStringId, hero.PartyBelongedTo);
        }

        string? newArmyStringId = null;
        serverMessageBroker.Subscribe<ArmyCreated>(payload =>
        {
            newArmyStringId = payload.What.Data.StringId;
        });

        // Act
        server.Call(() =>
        {
            kingdom.CreateArmy(hero, settlement, Army.ArmyTypes.Patrolling);
        });

        // Assert
        Assert.NotNull(newArmyStringId);

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<Army>(newArmyStringId, out var newArmy));
        }
    }

    [Fact]
    public void ClientCreateArmy_DoesNothing()
    {
        // Arrange
        var server = TestEnvironment.Server;
        var client1 = TestEnvironment.Clients.First();

        var kingdom = new Kingdom();
        var hero = GameObjectCreator.CreateInitializedObject<Hero>();
        var settlement = GameObjectCreator.CreateInitializedObject<Settlement>();
        var serverMessageBroker = server.Container.Resolve<IMessageBroker>();

        SetupKingdom(kingdom, hero, settlement);

        server.ObjectManager.AddNewObject(kingdom, out string kingdomStringId);
        server.ObjectManager.AddNewObject(hero, out string heroStringId);
        server.ObjectManager.AddNewObject(hero.PartyBelongedTo, out string partyStringId);

        foreach (var client in TestEnvironment.Clients)
        {
            client.ObjectManager.AddExisting(kingdomStringId, kingdom);
            client.ObjectManager.AddExisting(heroStringId, hero);
            client.ObjectManager.AddExisting(partyStringId, hero.PartyBelongedTo);
        }

        string? newArmyStringId = null;
        serverMessageBroker.Subscribe<ArmyCreated>(payload =>
        {
            newArmyStringId = payload.What.Data.StringId;
        });

        // Act
        Army? clientArmy = null;
        client1.Call(() =>
        {
            kingdom.CreateArmy(hero, settlement, Army.ArmyTypes.Patrolling);
        });

        // Assert
        Assert.False(server.ObjectManager.TryGetObject<Army>(newArmyStringId, out var _));

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.False(client.ObjectManager.TryGetObject<Army>(newArmyStringId, out var _));
        }
    }

    private void SetupKingdom(Kingdom kingdom, Hero hero, Settlement settlement)
    {
        var settlements = kingdom._settlementsCache!;
        settlements.Add(settlement);

        hero.Clan.Kingdom = kingdom;
    }
}
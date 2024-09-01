﻿using Autofac;
using Common.Messaging;
using E2E.Tests.Environment;
using E2E.Tests.Util;
using GameInterface.Services.Armies.Messages.Lifetime;
using GameInterface.Services.PartyBases.Extensions;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using Xunit.Abstractions;

namespace E2E.Tests.Services.Armies;

public class PartyVisualsTests : IDisposable
{
    E2ETestEnvironment TestEnvironment { get; }
    public PartyVisualsTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

    [Fact]
    public void ServerCreatePartyVisual_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironment.Server;

        // Act
        string? partyId = null;
        server.Call(() =>
        {
            var mobileParty = GameObjectCreator.CreateInitializedObject<MobileParty>();
            Assert.True(server.ObjectManager.TryGetId(mobileParty, out partyId));
            mobileParty.Party.GetPartyVisual().ReleaseResources();
        });

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<MobileParty>(partyId, out var _));
        }
    }

    [Fact]
    public void ClientCreateArmy_DoesNothing()
    {
        // Arrange
        var server = TestEnvironment.Server;
        var client1 = TestEnvironment.Clients.First();

        string? kingdomId = null;
        string? partyId = null;

        server.Call(() =>
        {
            var kingdom = GameObjectCreator.CreateInitializedObject<Kingdom>();
            var mobileParty = GameObjectCreator.CreateInitializedObject<MobileParty>();

            Assert.True(server.ObjectManager.TryGetId(kingdom, out kingdomId));
            Assert.True(server.ObjectManager.TryGetId(mobileParty, out partyId));
        });

        Assert.NotNull(kingdomId);
        Assert.NotNull(partyId);

        // Act
        string? armyId = null;
        client1.Call(() =>
        {
            Assert.True(client1.ObjectManager.TryGetObject<Kingdom>(kingdomId, out var kingdom));
            Assert.True(client1.ObjectManager.TryGetObject<MobileParty>(partyId, out var mobileParty));


            var army = new Army(kingdom, mobileParty, Army.ArmyTypes.Patrolling);

            Assert.False(client1.ObjectManager.TryGetId(army, out armyId));
        });

        // Assert
        Assert.Null(armyId);
    }
}
using E2E.Tests.Environment;
using Xunit.Abstractions;
using TaleWorlds.Core;
using HarmonyLib;
using E2E.Tests.Environment.Instance;
using GameInterface.Services.Equipments.Patches;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using System.Xml.Linq;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem;
using GameInterface.Services.Heroes.Patches;

namespace E2E.Tests.Services.Heroes;

public class HeroCollectionTests : IDisposable
{
    E2ETestEnvironment TestEnvironment { get; }
    private string ChildId;
    private string HeroId;
    private string CharacterObjectId;

    public HeroCollectionTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

    [Fact]
    public void ServerUpdateEquipmentCollection_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironment.Server;
        CharacterObject Character = null;
        Hero Child = null;

        // Act

        server.Call(() =>
        {
            HeroId = TestEnvironment.CreateRegisteredObject<Hero>();
            ChildId = TestEnvironment.CreateRegisteredObject<Hero>();
            Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var Hero));
            Assert.True(server.ObjectManager.TryGetObject<Hero>(ChildId, out Child));
            HeroCollectionPatches.ChildrenAddIntercept(Hero._children, Child, Hero);
            Assert.Equal( Child, Hero._children.Last());

            CharacterObjectId = TestEnvironment.CreateRegisteredObject<CharacterObject>();
            Assert.True(server.ObjectManager.TryGetObject<CharacterObject>(CharacterObjectId, out Character));

            Assert.NotEqual(Character, Hero.VolunteerTypes[0]);
            HeroCollectionPatches.ArrayAssignIntercept(Hero.VolunteerTypes, 0, Character, Hero);
            Assert.Equal(Character, Hero.VolunteerTypes[0]);
        });

        // Assert
        Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var Hero));

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<Hero>(HeroId, out var clientHero));
            Assert.Equal(Child.StringId, clientHero.Children.Last().StringId);  // Some fields are not synced like BannerItem,  
            Assert.Equal(Character.StringId, clientHero.VolunteerTypes[0].StringId);  // CharacterObject props/fields is not synced yet
        }
    }

    [Fact]
    public void ClientUpdateEquipmentCollection_DoesNothing()
    {
        // Arrange
        var server = TestEnvironment.Server;
        CharacterObject Character = null;
        Hero Child = null;

        server.Call(() =>
        {
            HeroId = TestEnvironment.CreateRegisteredObject<Hero>();
            ChildId = TestEnvironment.CreateRegisteredObject<Hero>();
            Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var Hero));
            Assert.True(server.ObjectManager.TryGetObject<Hero>(ChildId, out Child));

            CharacterObjectId = TestEnvironment.CreateRegisteredObject<CharacterObject>();
            Assert.True(server.ObjectManager.TryGetObject<CharacterObject>(CharacterObjectId, out Character));
        });

        // Act
        var firstClient = TestEnvironment.Clients.First();
        firstClient.Call(() =>
        {
            Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var clientHero));
            Assert.True(server.ObjectManager.TryGetObject<Hero>(ChildId, out var clientChild));
            HeroCollectionPatches.ChildrenAddIntercept(clientHero._children, clientChild, clientHero);

            Assert.True(server.ObjectManager.TryGetObject<CharacterObject>(CharacterObjectId, out Character));
            HeroCollectionPatches.ArrayAssignIntercept(clientHero.VolunteerTypes, 0, Character, clientHero);
        });

        // Assert
        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<Hero>(HeroId, out var clientHero));
            if (clientHero.Children.Count > 0)
            {
                Assert.NotEqual(Child.StringId, clientHero.Children.Last().StringId);
            }
            Assert.Null(clientHero.VolunteerTypes[0]);
        }
    }
}
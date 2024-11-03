using E2E.Tests.Environment;
using E2E.Tests.Environment.Instance;
using E2E.Tests.Util;
using HarmonyLib;
using Common.Util;
using System.Reflection;
using Xunit.Abstractions;
using static Common.Extensions.ReflectionExtensions;
using TaleWorlds.CampaignSystem.Siege;

namespace E2E.Tests.Services.SiegeEvents;

public class SiegeEventLifetimeTests : IDisposable
{
    private readonly List<MethodBase> disabledMethods;
    private E2ETestEnvironment TestEnvironment { get; }
    private EnvironmentInstance Server => TestEnvironment.Server;
    private IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;
    private IEnumerable<EnvironmentInstance> AllEnvironmentInstances => Clients.Append(Server);

    private readonly string siegeEventId;

    public SiegeEventLifetimeTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);

        disabledMethods = new List<MethodBase> {
            //Add your disabled methods
        };
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

        [Fact]
    public void ServerCreateSiegeEvent_SyncAllClients()
    {
        // Arrange
        string? siegeEventId = null;

        // Act
        Server.Call(() =>
        {
            var siegeEvent = GameObjectCreator.CreateInitializedObject<SiegeEvent>();
            Assert.True(Server.ObjectManager.TryGetId(siegeEvent, out siegeEventId));
        }, disabledMethods
        );

        // Assert
        Assert.NotNull(siegeEventId);

        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var _));
        }
    }

    [Fact]
    public void ClientCreateSiegeEvent_DoesNothing()
    {
        // Arrange
        string? clientSiegeEventId = null;

        // Act
        var firstClient = TestEnvironment.Clients.First();
        firstClient.Call(() =>
        {
            var SiegeEvent = ObjectHelper.SkipConstructor<SiegeEvent>();

            Assert.False(firstClient.ObjectManager.TryGetId(SiegeEvent, out clientSiegeEventId));
        });

        // Assert
        Assert.Null(clientSiegeEventId);
    }
}

    
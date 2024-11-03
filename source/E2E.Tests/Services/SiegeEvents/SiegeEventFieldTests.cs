using E2E.Tests.Environment;
using E2E.Tests.Environment.Instance;
using E2E.Tests.Util;
using HarmonyLib;
using System.Reflection;
using Xunit.Abstractions;
using static Common.Extensions.ReflectionExtensions;
using Common.Util;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.Settlements;

namespace E2E.Tests.Services.SiegeEvents;

public class SiegeEventFieldTests : IDisposable
{
    private readonly List<MethodBase> disabledMethods;
    private E2ETestEnvironment TestEnvironment { get; }
    private EnvironmentInstance Server => TestEnvironment.Server;
    private IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;
    private IEnumerable<EnvironmentInstance> AllEnvironmentInstances => Clients.Append(Server);

    private readonly string siegeEventId;

    public SiegeEventFieldTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);

        disabledMethods = new List<MethodBase> {
            //Add your disabled methods
        };

        // Create SiegeEvent on the server
        siegeEventId = TestEnvironment.CreateRegisteredObject<SiegeEvent>();

        // Create SiegeEvent on all clients
        foreach (var client in Clients)
        {
            var clientSiegeEvent = ObjectHelper.SkipConstructor<SiegeEvent>();
            Assert.True(client.ObjectManager.AddExisting(siegeEventId, clientSiegeEvent));
        }
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }


    [Fact]
    public void ServerChangeSiegeEventBesiegedSettlement_SyncAllClients()
    {
        // Arrange
        var field = AccessTools.Field(typeof(SiegeEvent), nameof(SiegeEvent.BesiegedSettlement));
        var intercept = TestEnvironment.GetIntercept(field);

        /// Create instances on server
        Assert.True(Server.ObjectManager.AddNewObject(ObjectHelper.SkipConstructor<Settlement>(), out var besiegedSettlementId));

        /// Create instances on all clients
        foreach (var client in Clients)
        {
            var clientBesiegedSettlement = ObjectHelper.SkipConstructor<Settlement>();
            Assert.True(client.ObjectManager.AddExisting(besiegedSettlementId, clientBesiegedSettlement));
        }

        // Act
        Server.Call(() =>
        {
            Assert.True(Server.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var SiegeEvent));
            Assert.True(Server.ObjectManager.TryGetObject<Settlement>(besiegedSettlementId, out var serverBesiegedSettlement));

            Assert.Null(SiegeEvent.BesiegedSettlement);

            /// Simulate the field changing
            intercept.Invoke(null, new object[] { SiegeEvent, serverBesiegedSettlement});

            Assert.Same(serverBesiegedSettlement, SiegeEvent.BesiegedSettlement);
        });

        // Assert
        foreach (var client in Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var SiegeEvent));

            Assert.True(client.ObjectManager.TryGetObject<Settlement>(besiegedSettlementId, out var clientBesiegedSettlement));

            Assert.True(clientBesiegedSettlement == SiegeEvent.BesiegedSettlement);
        }
    }
    
    [Fact]
    public void ServerChangeSiegeEventBesiegerCamp_SyncAllClients()
    {
        // Arrange
        var field = AccessTools.Field(typeof(SiegeEvent), nameof(SiegeEvent.BesiegerCamp));
        var intercept = TestEnvironment.GetIntercept(field);

        /// Create instances on server
        Assert.True(Server.ObjectManager.AddNewObject(ObjectHelper.SkipConstructor<BesiegerCamp>(), out var besiegerCampId));

        /// Create instances on all clients
        foreach (var client in Clients)
        {
            var clientBesiegerCamp = ObjectHelper.SkipConstructor<BesiegerCamp>();
            Assert.True(client.ObjectManager.AddExisting(besiegerCampId, clientBesiegerCamp));
        }

        // Act
        Server.Call(() =>
        {
            Assert.True(Server.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var SiegeEvent));
            Assert.True(Server.ObjectManager.TryGetObject<BesiegerCamp>(besiegerCampId, out var serverBesiegerCamp));

            Assert.Null(SiegeEvent.BesiegerCamp);

            /// Simulate the field changing
            intercept.Invoke(null, new object[] { SiegeEvent, serverBesiegerCamp});

            Assert.Same(serverBesiegerCamp, SiegeEvent.BesiegerCamp);
        });

        // Assert
        foreach (var client in Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var SiegeEvent));

            Assert.True(client.ObjectManager.TryGetObject<BesiegerCamp>(besiegerCampId, out var clientBesiegerCamp));

            Assert.True(clientBesiegerCamp == SiegeEvent.BesiegerCamp);
        }
    }
    

    [Fact]
    public void ServerChangeSiegeEventIsBesiegerDefeated_SyncAllClients()
    {
        // Arrange
        var field = AccessTools.Field(typeof(BesiegerCamp), nameof(BesiegerCamp._leaderParty));
        var intercept = TestEnvironment.GetIntercept(field);
        Assert.True(Server.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var serverSiegeEvent));
        var newValue=Random<Boolean>();

        // Act
        Server.Call(() =>
        {
            /// Simulate the field changing
            intercept.Invoke(null, new object[] { serverSiegeEvent, newValue });

            Assert.Same(newValue, serverSiegeEvent._isBesiegerDefeated);
        });

        // Assert
        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<SiegeEvent>(siegeEventId, out var clientSiegeEvent));
            Assert.Equal(serverSiegeEvent._isBesiegerDefeated, clientSiegeEvent._isBesiegerDefeated);
        }
    }  
    }

    
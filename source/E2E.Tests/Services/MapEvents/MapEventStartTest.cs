using E2E.Tests.Environment;
using E2E.Tests.Util;
using TaleWorlds.CampaignSystem.MapEvents;
using Xunit.Abstractions;

namespace E2E.Tests.Services.MapEvents
{
    public class MapEventStartTest : IDisposable
    {
        E2ETestEnvironment TestEnvironment { get; }

        public MapEventStartTest(ITestOutputHelper output)
        {
            TestEnvironment = new E2ETestEnvironment(output);
        }

        public void Dispose()
        {
            TestEnvironment.Dispose();
        }

        [Fact]
        public void ServerCreate_MapEvent_SyncAllClients()
        {
            // Arrange
            var server = TestEnvironment.Server;

            // Act
            string? mapEventId = null;
            string side1Id = null;
            string side2Id = null;
            server.Call(() =>
            {
                var mapEvent = GameObjectCreator.CreateInitializedObject<MapEvent>();
                var side1 = GameObjectCreator.CreateInitializedObject<MapEventSide>();
                var side2 = GameObjectCreator.CreateInitializedObject<MapEventSide>();


                Assert.True(server.ObjectManager.TryGetId(mapEvent, out mapEventId));
                Assert.True(server.ObjectManager.TryGetId(side1, out side1Id));
                Assert.True(server.ObjectManager.TryGetId(side2, out side2Id));

                side1.LeaderParty.MapEventSide = side1;
                side2.LeaderParty.MapEventSide = side2;
            });

            // Assert
            Assert.NotNull(mapEventId);

            foreach (var client in TestEnvironment.Clients)
            {
                Assert.True(client.ObjectManager.TryGetObject<MapEvent>(mapEventId, out var _));
                Assert.True(client.ObjectManager.TryGetObject(side1Id, out MapEventSide clientSide1));
                Assert.True(client.ObjectManager.TryGetObject(side2Id, out MapEventSide clientSide2));

                clientSide1.IsSurrendered = true;
                clientSide2.IsSurrendered = true;
            }
        }
    }
}

using E2E.Tests.Environment;
using E2E.Tests.Environment.Instance;
using E2E.Tests.Util;
using System.Runtime.InteropServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements;
using Xunit.Abstractions;
using TaleWorlds.Core;
using Autofac.Features.OwnedInstances;

namespace E2E.Tests.Services.Heroes
{
    public class HeroPropertyTests : IDisposable
    {
        E2ETestEnvironment TestEnvironment { get; }

        EnvironmentInstance Server => TestEnvironment.Server;

        IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;

        private string HeroId;

        StaticBodyProperties body = new StaticBodyProperties(1, 2, 1, 2, 1, 3, 1, 2);

        public HeroPropertyTests(ITestOutputHelper output)
        {
            TestEnvironment = new E2ETestEnvironment(output);
        }

        public void Dispose()
        {
            TestEnvironment.Dispose();
        }

        [Fact]
        public void Server_Sync_Hero()
        {
            var server = TestEnvironment.Server;
            Hero hero = null; 

            server.Call(() =>
            {
                hero = GameObjectCreator.CreateInitializedObject<Hero>();

                hero.StaticBodyProperties = body;

                Assert.Equal(body, hero.StaticBodyProperties);

                Assert.True(server.ObjectManager.TryGetId(hero, out HeroId));
            });

            foreach (var client in Clients)
            {
                Assert.True(client.ObjectManager.TryGetObject<Hero>(HeroId, out var clientHero));

                Assert.Equal(body, clientHero.StaticBodyProperties);
            }
        }
    }
}

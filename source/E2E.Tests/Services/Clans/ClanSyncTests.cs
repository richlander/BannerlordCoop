﻿using Autofac.Features.OwnedInstances;
using E2E.Tests.Environment;
using E2E.Tests.Environment.Instance;
using E2E.Tests.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using Xunit.Abstractions;

namespace E2E.Tests.Services.Clans
{
    public class ClanSyncTests : IDisposable
    {
        E2ETestEnvironment TestEnvironment { get; }

        EnvironmentInstance Server => TestEnvironment.Server;

        IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;

        private string KingdomId;
        private string SettlementId;
        private string CharacterObjectId;
        private string HeroId;

        public ClanSyncTests(ITestOutputHelper output)
        {
            TestEnvironment = new E2ETestEnvironment(output);

            var kingdom = GameObjectCreator.CreateInitializedObject<Kingdom>();
            var settlement = GameObjectCreator.CreateInitializedObject<Settlement>();
            var characterObject = GameObjectCreator.CreateInitializedObject<CharacterObject>();
            var hero = GameObjectCreator.CreateInitializedObject<Hero>();

            // Create objects on the server
            Assert.True(Server.ObjectManager.AddNewObject(kingdom, out KingdomId));
            Assert.True(Server.ObjectManager.AddNewObject(settlement, out SettlementId));
            Assert.True(Server.ObjectManager.AddNewObject(characterObject, out CharacterObjectId));
            Assert.True(Server.ObjectManager.AddNewObject(hero, out HeroId));

            // Create objects on all clients
            foreach (var client in Clients)
            {
                Assert.True(client.ObjectManager.AddExisting(KingdomId, kingdom));
                Assert.True(client.ObjectManager.AddExisting(SettlementId, settlement));
                Assert.True(client.ObjectManager.AddExisting(CharacterObjectId, characterObject));
                Assert.True(client.ObjectManager.AddExisting(HeroId, hero));
            }
        }

        public void Dispose()
        {
            TestEnvironment.Dispose();
        }

        [Fact]
        public void ServerUpdateClan_SyncAllClients()
        {
            // Arrange
            var server = TestEnvironment.Server;

            // Act
            string? clanId = null;
            server.Call(() =>
            {
                var clan = Clan.CreateClan("");
                clanId = clan.StringId;
                clan.Name = new TextObject("testName");
                clan.InformalName = new TextObject("testInformal");
                clan.Culture = new CultureObject();
                clan.LastFactionChangeTime = new CampaignTime(100);
                clan.AutoRecruitmentExpenses = 10;
                clan.IsNoble = true;
                clan.TotalStrength = 199;
                clan.MercenaryAwardMultiplier = 5;
                clan.LabelColor = 2;
                clan.InitialPosition = new TaleWorlds.Library.Vec2(1, 2);
                clan.IsRebelClan = true;
                clan.IsUnderMercenaryService = true;
                clan.Color = 3;
                clan.Color2 = 4;
                clan.BannerBackgroundColorPrimary = 5;
                clan.BannerBackgroundColorSecondary = 6;
                clan.BannerIconColor = 7;
                clan._midPointCalculated = true;
                clan.Renown = 55f;
                clan.NotAttackableByPlayerUntilTime = new CampaignTime(300);

            });

            var isEliminatedField = AccessTools.Field(typeof(Clan), nameof(Clan._isEliminated));
            var kingdomField = AccessTools.Field(typeof(Clan), nameof(Clan._kingdom));
            var influenceField = AccessTools.Field(typeof(Clan), nameof(Clan._influence));
            var clanMidSettlementField = AccessTools.Field(typeof(Clan), nameof(Clan._clanMidSettlement));
            var basicTroopField = AccessTools.Field(typeof(Clan), nameof(Clan._basicTroop));
            var leaderField = AccessTools.Field(typeof(Clan), nameof(Clan._leader));
            var bannerField = AccessTools.Field(typeof(Clan), nameof(Clan._banner));
            var tierField = AccessTools.Field(typeof(Clan), nameof(Clan._tier));
            var aggressivenessField = AccessTools.Field(typeof(Clan), nameof(Clan._aggressiveness));
            var tributeWalletField = AccessTools.Field(typeof(Clan), nameof(Clan._tributeWallet));
            var homeField = AccessTools.Field(typeof(Clan), nameof(Clan._home));
            var clanDebtToKingdomField = AccessTools.Field(typeof(Clan), nameof(Clan._clanDebtToKingdom));

            // Get field intercept to use on the server to simulate the field changing
            var isEliminatedIntercept = TestEnvironment.GetIntercept(isEliminatedField);
            var kingdomIntercept = TestEnvironment.GetIntercept(kingdomField);
            var influenceIntercept = TestEnvironment.GetIntercept(influenceField);
            var clanMidSettlementIntercept = TestEnvironment.GetIntercept(clanMidSettlementField);
            var basicTroopIntercept = TestEnvironment.GetIntercept(basicTroopField);
            var leaderIntercept = TestEnvironment.GetIntercept(leaderField);
            var bannerIntercept = TestEnvironment.GetIntercept(bannerField);
            var tierIntercept = TestEnvironment.GetIntercept(tierField); 
            var aggressivenessIntercept = TestEnvironment.GetIntercept(aggressivenessField);
            var tributeWalletIntercept = TestEnvironment.GetIntercept(tributeWalletField);
            var homeIntercept = TestEnvironment.GetIntercept(homeField);
            var clanDebtToKingdomIntercept = TestEnvironment.GetIntercept(clanDebtToKingdomField);

            // Assert
            Assert.True(server.ObjectManager.TryGetObject(clanId, out Clan serverClan));

            server.Call(() =>
            {
                Assert.True(server.ObjectManager.TryGetObject<Kingdom>(KingdomId, out var serverKingdom));
                Assert.True(server.ObjectManager.TryGetObject<Settlement>(SettlementId, out var serverSettlement));
                Assert.True(server.ObjectManager.TryGetObject<CharacterObject>(CharacterObjectId, out var serverCharacter));
                Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var serverHero));
                Banner banner = new Banner();

                isEliminatedIntercept.Invoke(null, new object[] { serverClan, true });
                kingdomIntercept.Invoke(null, new object[] { serverClan, serverKingdom });
                influenceIntercept.Invoke(null, new object[] { serverClan, 500f });
                clanMidSettlementIntercept.Invoke(null, new object[] { serverClan, serverSettlement });
                basicTroopIntercept.Invoke(null, new object[] { serverClan, serverCharacter });
                leaderIntercept.Invoke(null, new object[] { serverClan, serverHero });
                bannerIntercept.Invoke(null, new object[] { serverClan, banner });
                tierIntercept.Invoke(null, new object[] { serverClan, 5 });
                aggressivenessIntercept.Invoke(null, new object[] { serverClan, 60f });
                tributeWalletIntercept.Invoke(null, new object[] { serverClan, 30 });
                homeIntercept.Invoke(null, new object[] { serverClan, serverSettlement });
                clanDebtToKingdomIntercept.Invoke(null, new object[] { serverClan, 25 });
            });

            foreach (var client in TestEnvironment.Clients)
            {
                Assert.True(client.ObjectManager.TryGetObject(clanId, out Clan clientClan));
                Assert.Equal(serverClan.Name.Value, clientClan.Name.Value);
                Assert.Equal(serverClan.InformalName.Value, clientClan.InformalName.Value);
                Assert.Equal(serverClan.Culture, clientClan.Culture);
                Assert.Equal(serverClan.LastFactionChangeTime, clientClan.LastFactionChangeTime);
                Assert.Equal(serverClan.AutoRecruitmentExpenses, clientClan.AutoRecruitmentExpenses);
                Assert.Equal(serverClan.IsNoble, clientClan.IsNoble);
                Assert.Equal(serverClan.TotalStrength, clientClan.TotalStrength);
                Assert.Equal(serverClan.MercenaryAwardMultiplier, clientClan.MercenaryAwardMultiplier);
                Assert.Equal(serverClan.LabelColor, clientClan.LabelColor);
                Assert.Equal(serverClan.InitialPosition, clientClan.InitialPosition);
                Assert.Equal(serverClan.IsRebelClan, clientClan.IsRebelClan);
                Assert.Equal(serverClan.IsUnderMercenaryService, clientClan.IsUnderMercenaryService);
                Assert.Equal(serverClan.Color, clientClan.Color);
                Assert.Equal(serverClan.Color2, clientClan.Color2);
                Assert.Equal(serverClan.BannerBackgroundColorPrimary, clientClan.BannerBackgroundColorPrimary);
                Assert.Equal(serverClan.BannerBackgroundColorSecondary, clientClan.BannerBackgroundColorSecondary);
                Assert.Equal(serverClan.BannerIconColor, clientClan.BannerIconColor);
                Assert.Equal(serverClan._midPointCalculated, clientClan._midPointCalculated);
                Assert.Equal(serverClan.Renown, clientClan.Renown);
                Assert.Equal(serverClan.NotAttackableByPlayerUntilTime, clientClan.NotAttackableByPlayerUntilTime);

                Assert.Equal(serverClan._isEliminated, clientClan._isEliminated);
                Assert.Equal(serverClan._kingdom, clientClan._kingdom);
                Assert.Equal(serverClan._influence, clientClan._influence);
                Assert.Equal(serverClan._clanMidSettlement, clientClan._clanMidSettlement);
                Assert.Equal(serverClan._basicTroop, clientClan._basicTroop);
                Assert.Equal(serverClan._leader, clientClan._leader);
                Assert.Equal(serverClan._banner._bannerVisual, clientClan._banner._bannerVisual);
                Assert.Equal(serverClan._banner._bannerDataList, clientClan._banner._bannerDataList);
                Assert.Equal(serverClan._tier, clientClan._tier);
                Assert.Equal(serverClan._aggressiveness, clientClan._aggressiveness);
                Assert.Equal(serverClan._tributeWallet, clientClan._tributeWallet);
                Assert.Equal(serverClan._home, clientClan._home);
                Assert.Equal(serverClan._clanDebtToKingdom, clientClan._clanDebtToKingdom);
            }
        }
    }
}

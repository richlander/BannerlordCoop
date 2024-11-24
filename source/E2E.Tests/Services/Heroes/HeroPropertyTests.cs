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
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Issues;
using static TaleWorlds.CampaignSystem.Issues.BettingFraudIssueBehavior;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace E2E.Tests.Services.Heroes
{
    public class HeroPropertyTests : IDisposable
    {
        E2ETestEnvironment TestEnvironment { get; }

        EnvironmentInstance Server => TestEnvironment.Server;

        IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;

        private string HeroId;

        StaticBodyProperties body = new StaticBodyProperties(1, 2, 1, 2, 1, 3, 1, 2);
        float newFloat = 5f;
        int newInt = 9;
        long newLong = 99;
        TextObject newText = new TextObject("testText");
        CampaignTime newCampaignTime = new CampaignTime(999);
        FormationClass newFormation = new FormationClass();
        Hero.CharacterStates newCharState = Hero.CharacterStates.Released;
        Occupation newOccupation = Occupation.Mercenary;
        KillCharacterAction.KillCharacterActionDetail newKillAction = KillCharacterAction.KillCharacterActionDetail.Murdered;
        EquipmentElement newEquipmentElement = new EquipmentElement();

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
            Hero newHero = null;

            server.Call(() =>
            {
                hero = GameObjectCreator.CreateInitializedObject<Hero>();
                newHero = GameObjectCreator.CreateInitializedObject<Hero>();
                Clan newClan = GameObjectCreator.CreateInitializedObject<Clan>();
                Settlement newSettlement = GameObjectCreator.CreateInitializedObject<Settlement>();
                Town newTown = GameObjectCreator.CreateInitializedObject<Town>();
                MobileParty newMobileParty = GameObjectCreator.CreateInitializedObject<MobileParty>();
                Equipment newBattleEquipment = new Equipment(false);
                Equipment newCivEquipment = new Equipment(true);
                BettingFraudIssue newIssue = new BettingFraudIssue(hero);



                hero.StaticBodyProperties = body;
                hero.Weight = newFloat;
                hero.Build = newFloat;
                hero.PassedTimeAtHomeSettlement = newFloat;
                hero.EncyclopediaText = newText;
                hero.IsFemale = true;
                hero._battleEquipment = newBattleEquipment;
                hero._civilianEquipment = newCivEquipment;
                hero.CaptivityStartTime = newCampaignTime;
                hero.PreferredUpgradeFormation = newFormation;
                hero.HeroState = newCharState;
                hero.IsMinorFactionHero = true;
                hero.Issue = newIssue;
                hero.CompanionOf = newClan;
                hero.Occupation = newOccupation;
                hero.DeathMark = newKillAction;
                hero.DeathMarkKillerHero = newHero;
                hero.LastKnownClosestSettlement = newSettlement;
                hero.HitPoints = newInt;
                hero.DeathDay = newCampaignTime;
                hero.LastExaminedLogEntryID = newLong;
                hero.Clan = newClan;
                hero.SupporterOf = newClan;
                hero.GovernorOf = newTown;
                hero.PartyBelongedTo = newMobileParty;
                hero.PartyBelongedToAsPrisoner = newMobileParty.Party;
                hero.StayingInSettlement = newSettlement;
                hero.IsKnownToPlayer = true;
                hero.HasMet = true;
                hero.LastMeetingTimeWithPlayer = newCampaignTime;
                hero.BornSettlement = newSettlement;
                hero.Gold = newInt;
                hero.RandomValue = newInt;
                hero.BannerItem = newEquipmentElement;
                hero.Father = newHero;
                hero.Mother = newHero;
                hero.Spouse = newHero;

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

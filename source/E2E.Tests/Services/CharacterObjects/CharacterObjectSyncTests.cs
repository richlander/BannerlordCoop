using E2E.Tests.Environment;
using E2E.Tests.Environment.Instance;
using E2E.Tests.Util;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using Xunit.Abstractions;

namespace E2E.Tests.Services.CharacterObjects
{
    public class CharacterObjectSyncTests : IDisposable
    {
        E2ETestEnvironment TestEnvironment { get; }

        EnvironmentInstance Server => TestEnvironment.Server;

        IEnumerable<EnvironmentInstance> Clients => TestEnvironment.Clients;

        private string HeroId;
        private Occupation newOccupation = Occupation.Armorer;
        private CharacterObject newCharacterObject = new CharacterObject();
        private CharacterTraits newCharacterTraits = new CharacterTraits();
        private TraitObject newTraitObject = new TraitObject("testObject");
        private CharacterRestrictionFlags newRestrictionFlags = CharacterRestrictionFlags.CanNotGoInHideout;

        public CharacterObjectSyncTests(ITestOutputHelper output)
        {
            TestEnvironment = new E2ETestEnvironment(output);
        }

        public void Dispose()
        {
            TestEnvironment.Dispose();
        }

        [Fact]
        public void Server_CharacterObject_Sync()
        {
            // Arrange
            var server = TestEnvironment.Server;

            var occupationField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._occupation));
            var battleEquipmentTemplateField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._battleEquipmentTemplate));
            var civilianEquipmentTemplateField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._civilianEquipmentTemplate));
            var characterTraitsField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._characterTraits));
            var personaField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._persona));
            var originCharacterField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._originCharacter));
            var characterRestrictionField = AccessTools.Field(typeof(CharacterObject), nameof(CharacterObject._characterRestrictionFlags));

            // Get field intercept to use on the server to simulate the field changing
            var occupationIntercept = TestEnvironment.GetIntercept(occupationField);
            var battleEquipmentTemplateIntercept = TestEnvironment.GetIntercept(battleEquipmentTemplateField);
            var civilianEquipmentTemplateIntercept = TestEnvironment.GetIntercept(civilianEquipmentTemplateField);
            var characterTraitsIntercept = TestEnvironment.GetIntercept(characterTraitsField);
            var personaIntercept = TestEnvironment.GetIntercept(personaField);
            var originCharacterIntercept = TestEnvironment.GetIntercept(originCharacterField);
            var characterRestrictionIntercept = TestEnvironment.GetIntercept(characterRestrictionField);

            // Act
            server.Call(() =>
            {
                Hero hero = GameObjectCreator.CreateInitializedObject<Hero>();

                Assert.True(server.ObjectManager.TryGetId(hero, out HeroId));

                Assert.True(server.ObjectManager.TryGetObject<Hero>(HeroId, out var serverHero));

                // Simulate the field changing
                occupationIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newOccupation });
                battleEquipmentTemplateIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newCharacterObject });
                civilianEquipmentTemplateIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newCharacterObject });
                characterTraitsIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newCharacterTraits });
                personaIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newTraitObject });
                originCharacterIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newCharacterObject });
                characterRestrictionIntercept.Invoke(null, new object[] { serverHero.CharacterObject, newRestrictionFlags });

                serverHero.CharacterObject.HiddenInEncylopedia = true;
                serverHero.CharacterObject.HeroObject = serverHero;

                Assert.Equal(newOccupation, serverHero.CharacterObject.Occupation);
                Assert.Equal(newCharacterObject, serverHero.CharacterObject._battleEquipmentTemplate);
                Assert.Equal(newCharacterObject, serverHero.CharacterObject._civilianEquipmentTemplate);
                Assert.Equal(newCharacterTraits, serverHero.CharacterObject._characterTraits);
                Assert.Equal(newTraitObject, serverHero.CharacterObject._persona);
                Assert.Equal(newCharacterObject, serverHero.CharacterObject._originCharacter);
                Assert.Equal(newRestrictionFlags, serverHero.CharacterObject._characterRestrictionFlags);

                Assert.True(serverHero.CharacterObject.HiddenInEncylopedia);
                Assert.Equal(serverHero, serverHero.CharacterObject.HeroObject);
            });

            // Assert
            foreach (var client in Clients)
            {
                Assert.True(client.ObjectManager.TryGetObject<Hero>(HeroId, out var clientHero));

                Assert.Equal(newOccupation, clientHero.CharacterObject.Occupation);
                Assert.Equal(newCharacterObject, clientHero.CharacterObject._battleEquipmentTemplate);
                Assert.Equal(newCharacterObject, clientHero.CharacterObject._civilianEquipmentTemplate);
                Assert.Equal(newCharacterTraits, clientHero.CharacterObject._characterTraits);
                Assert.Equal(newTraitObject, clientHero.CharacterObject._persona);
                Assert.Equal(newCharacterObject, clientHero.CharacterObject._originCharacter);
                Assert.Equal(newRestrictionFlags, clientHero.CharacterObject._characterRestrictionFlags);

                Assert.True(clientHero.CharacterObject.HiddenInEncylopedia);
                Assert.Equal(clientHero, clientHero.CharacterObject.HeroObject);
            }
        }
    }
}

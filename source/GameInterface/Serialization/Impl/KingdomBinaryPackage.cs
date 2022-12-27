﻿using Common.Extensions;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;
using System.Linq;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map;

namespace GameInterface.Serialization.Impl
{
    /// <summary>
    /// Binary package for Kingdom
    /// </summary>
    [Serializable]
    public class KingdomBinaryPackage : BinaryPackageBase<Kingdom>
    {
        public static readonly FieldInfo Kingdom_Armies = typeof(Kingdom).GetField("_armies", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Clans = typeof(Kingdom).GetField("_clans", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Fiefs = typeof(Kingdom).GetField("_fiefsCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Heroes = typeof(Kingdom).GetField("_heroesCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Lords = typeof(Kingdom).GetField("_lordsCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Settlements = typeof(Kingdom).GetField("_settlementsCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_Villages = typeof(Kingdom).GetField("_villagesCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo Kingdom_WarPartyComponents = typeof(Kingdom).GetField("_warPartyComponentsCache", BindingFlags.NonPublic | BindingFlags.Instance);

        private string stringId;
        private string[] clanIds;
        private string[] fiefIds;
        private string[] heroIds;
        private string[] lordIds;
        private string[] settlementIds;
        private string[] villageIds;

        public KingdomBinaryPackage(Kingdom obj, BinaryPackageFactory binaryPackageFactory) : base(obj, binaryPackageFactory)
        {
        }

        private static readonly HashSet<string> excludes = new HashSet<string>
        {
            "_distanceToClosestNonAllyFortificationCacheDirty",
            "_distanceToClosestNonAllyFortificationCache",
            "_clans",
            "_warPartyComponentsCache",
            "_lordsCache",
            "_heroesCache",
            "_settlementsCache",
            "_villagesCache",
            "_fiefsCache",
            "<Armies>k__BackingField",
            "<Fiefs>k__BackingField",
            "<Villages>k__BackingField",
            "<Settlements>k__BackingField",
            "<Heroes>k__BackingField",
            "<Lords>k__BackingField",
            "<WarPartyComponents>k__BackingField",
        };

        public override void Pack()
        {
            stringId = Object.StringId;

            foreach (FieldInfo field in ObjectType.GetAllInstanceFields(excludes))
            {
                object obj = field.GetValue(Object);
                StoredFields.Add(field, BinaryPackageFactory.GetBinaryPackage(obj));
            }

            clanIds = PackIds(Object.Clans);
            fiefIds = PackIds(Object.Fiefs);
            heroIds = PackIds(Object.Heroes);
            lordIds = PackIds(Object.Lords);
            settlementIds = PackIds(Object.Settlements);
            villageIds = PackIds(Object.Villages);
        }

        private string[] PackIds<T>(IEnumerable<T> values) where T : MBObjectBase
        {
            if (values == null) return new string[0];

            return values.Select(value => value.StringId).ToArray();
        }

        public static readonly MethodInfo InitializeCachedLists = typeof(Kingdom).GetMethod("InitializeCachedLists", BindingFlags.NonPublic | BindingFlags.Instance);
        protected override void UnpackInternal()
        {
            if(stringId != null)
            {
                Kingdom kingdom = MBObjectManager.Instance.GetObject<Kingdom>(stringId);
                if (kingdom != null)
                {
                    Object = kingdom;
                    return;
                }
            }

            TypedReference reference = __makeref(Object);
            foreach (FieldInfo field in StoredFields.Keys)
            {
                field.SetValueDirect(reference, StoredFields[field].Unpack());
            }

            InitializeCachedLists.Invoke(Object, new object[0]);

            // Cached armies are handed in the ArmyBinaryPackage

            // Cached WarPartyComponents are handed in the
            // BanditComponentBinaryPackage and LordPartyComponentBinaryPackage

            Kingdom_Clans.SetValue(Object, ResolveIds<Clan>(clanIds));
            Kingdom_Fiefs.SetValue(Object, ResolveIds<Town>(fiefIds));
            Kingdom_Heroes.SetValue(Object, ResolveIds<Hero>(heroIds));
            Kingdom_Lords.SetValue(Object, ResolveIds<Hero>(lordIds));
            Kingdom_Settlements.SetValue(Object, ResolveIds<Settlement>(settlementIds));
            Kingdom_Villages.SetValue(Object, ResolveIds<Village>(villageIds));
        }

        private List<T> ResolveIds<T>(string[] ids) where T : MBObjectBase
        {
            // Convert ids to instances
            List<T> values = ids.Select(id => MBObjectManager.Instance.GetObject<T>(id)).ToList();

            // Ensure all instances are resolved
            if (values.Any(v => v == null))
                throw new Exception($"Some values were not resolved in {values}");

            return values;
        }
    }
}

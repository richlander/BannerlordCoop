﻿using Common.Extensions;
using System;
using System.Reflection;
using static TaleWorlds.Core.HorseComponent;

namespace GameInterface.Serialization.Impl
{
    /// <summary>
    /// Binary package for MaterialProperty
    /// </summary>
    [Serializable]
    public class MaterialPropertyBinaryPackage : BinaryPackageBase<MaterialProperty>
    {
        public MaterialPropertyBinaryPackage(MaterialProperty obj, BinaryPackageFactory binaryPackageFactory) : base(obj, binaryPackageFactory)
        {
        }

        public override void Pack()
        {
            foreach (FieldInfo field in ObjectType.GetAllInstanceFields())
            {
                object obj = field.GetValue(Object);
                StoredFields.Add(field, BinaryPackageFactory.GetBinaryPackage(obj));
            }
        }

        protected override void UnpackInternal()
        {
            TypedReference reference = __makeref(Object);
            foreach (FieldInfo field in StoredFields.Keys)
            {
                field.SetValueDirect(reference, StoredFields[field].Unpack());
            }
        }
    }
}

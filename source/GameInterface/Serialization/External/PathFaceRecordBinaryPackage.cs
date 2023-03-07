﻿using Common.Extensions;
using System;
using System.Reflection;
using TaleWorlds.Library;

namespace GameInterface.Serialization.External
{
    /// <summary>
    /// Binary package for PathFaceRecord
    /// </summary>
    [Serializable]
    public class PathFaceRecordBinaryPackage : BinaryPackageBase<PathFaceRecord>
    {
        public PathFaceRecordBinaryPackage(PathFaceRecord obj, BinaryPackageFactory binaryPackageFactory) : base(obj, binaryPackageFactory)
        {
        }

        protected override void PackInternal()
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

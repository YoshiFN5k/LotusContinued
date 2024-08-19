using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Il2CppInterop.Runtime.InteropTypes;

namespace Lotus.Extensions;

public static class ObjectExtensions
{
    public static bool TryCast<T>(this Il2CppObjectBase obj, out T? casted)
    where T : Il2CppObjectBase
    {
        casted = obj.TryCast<T>();
        return casted != null;
    }
}
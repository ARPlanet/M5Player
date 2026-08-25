using System;
using UnityEngine;

namespace Module5.Player
{
    public static class DefaultVariableRegistration
    {
        public static void RegisterDefaultVariableTypes(IVariableTypeRegistry registry)
        {
            if (registry == null) return;
            registry.RegisterType<string>("string");
            registry.RegisterType<float>("float");
            registry.RegisterType<DateTime>("datetime");
            //registry.RegisterType<int>("int");
            //registry.RegisterType<bool>("bool");
            //registry.RegisterType<Vector3>("vector3");
        }
    }
}

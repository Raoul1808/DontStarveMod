using HarmonyLib;
using OWML.Common;

namespace DontStarveMod;

[HarmonyPatch]
public class ModPatches
{
    [HarmonyPatch(typeof(Marshmallow), nameof(Marshmallow.Eat))]
    [HarmonyPostfix]
    public static void Marshmallow_Eat_Postfix()
    {
        DontStarveMod.Log("Eating Marshmallow", MessageType.Success);
        DontStarveMod.Instance.ResetHunger(fullRefill: false);
    }
}

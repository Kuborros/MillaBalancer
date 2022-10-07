using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MillaBalancer;
using System.Collections.Generic;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace MillaBalancer
{
    [BepInPlugin("com.kuborro.plugins.fp2.millacube", "MillaCubeEditor", "1.1.0")]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<float> configRange;
        public static ConfigEntry<float> configProjectiles;
        public static ConfigEntry<int> configCubes;
        public static ConfigEntry<bool> configAlwaysSpawn;
        private void Awake()
        {
            configRange = Config.Bind("General", "Range",(float) -50.0, "Set Milla's cube/projectile range. Value must be negative float. 0 = No range, -10 = Default, -50 = Basically infinite.");
            configProjectiles = Config.Bind("General", "Cubes fired per cube", (float) 500.0, "Set how many cubes can Milla fire per one follower cube . Value must be a positive float. 0 = No cubes, 100 = Default, 500 = 5x the shots.");
            configCubes = Config.Bind("General", "Extra cubes spawned", 10, "Set how many cubes the cube powerup and guard (if enabled below) will spawn. 0 and negative values are not allowed.");
            configAlwaysSpawn = Config.Bind("General", "Spawn extra cubes on guard", true, "Set if you want all the extra cubes to be spawned on guard.");

            if (configRange.Value > (float) 0.0)
            {
                configRange.Value = (float) -50.0;
            }
            if (configCubes.Value <= 0) configCubes.Value = 1;

                //HarmonyFileLog.Enabled = true;
                var harmony = new Harmony("com.kuborro.plugins.fp2.millacube");
                harmony.PatchAll(typeof(Patch));
                harmony.PatchAll(typeof(Patch2));
                harmony.PatchAll(typeof(Patch3));
                harmony.PatchAll(typeof(Patch4));
        }
    }

    class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MillaCube), nameof(MillaCube.ObjectCreated), MethodType.Normal)]
        static void Postfix(ref float ___explodeTimer)
        {
            //FileLog.Log("Who is milla now? " + GameObject.Find("Player 1").GetComponent<FPPlayer>().characterID.ToString());
            if (GameObject.Find("Player 1").GetComponent<FPPlayer>().characterID.ToString() == "MILLA")
            {
                ___explodeTimer = Plugin.configRange.Value;
            }
        }
    }
}

class Patch2
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FPPlayer), nameof(FPPlayer.Action_MillaCubeSpawn), MethodType.Normal)]
    static void Postfix(ref float ___millaCubeEnergy, FPPlayer __instance)
    {
        {
            ___millaCubeEnergy = Plugin.configProjectiles.Value;
            if (Plugin.configAlwaysSpawn.Value)
            {
                __instance.Action_MillaMultiCube();
            }
        }
    }
}

class Patch3
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FPPlayer), nameof(FPPlayer.Action_MillaMultiCube), MethodType.Normal)]
    static void Postfix(ref float ___millaCubeEnergy)
    {
        {
            ___millaCubeEnergy = Plugin.configProjectiles.Value;
        }
    }
}

public static class Patch4
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(FPPlayer), nameof(FPPlayer.Action_MillaMultiCube), MethodType.Normal)]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        for (var i = 0; i < codes.Count; i++)            
        {
            if (codes[i].opcode.Name == "ldc.i4.3")
            {
                codes[i].opcode = OpCodes.Ldc_I4_S;
                codes[i].operand = Plugin.configCubes.Value;           
            }
            if (codes[i].opcode.Name == "ldc.i4.6")
            {
                codes[i].opcode = OpCodes.Ldc_I4_S;
                codes[i].operand = Plugin.configCubes.Value;
                break;
            }
        }
        return codes;
    }
}
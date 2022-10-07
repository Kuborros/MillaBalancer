using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MillaBalancer;
using UnityEngine;

namespace MillaBalancer
{
    [BepInPlugin("com.kuborro.plugins.fp2.millacube", "MillaCubeEditor", "1.1.0")]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<float> configRange;
        public static ConfigEntry<float> configProjectiles;
        private void Awake()
        {
            configRange = Config.Bind("General", "Range",(float) -50.0, "Set Milla's cube/projectile range. Value must be negative float. 0 = No range, -10 = Default, -50 = Basically infinite.");
            configProjectiles = Config.Bind("General", "Cubes fired per cube", (float) 500.0, "Set how many cubes can Milla fire per one follower cube . Value must be a positive float. 0 = No cubes, 100 = Default, 500 = 5x the shots.");

            if (configRange.Value > (float) 0.0)
            {
                configRange.Value = (float) -50.0;
            }
                //HarmonyFileLog.Enabled = true;
                var harmony = new Harmony("com.kuborro.plugins.fp2.millacube");
                harmony.PatchAll(typeof(Patch));
                harmony.PatchAll(typeof(Patch2));
                harmony.PatchAll(typeof(Patch3));
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
    static void Postfix(ref float ___millaCubeEnergy)
    {
        {
            ___millaCubeEnergy = Plugin.configProjectiles.Value;
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

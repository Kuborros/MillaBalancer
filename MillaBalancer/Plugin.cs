using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MillaBalancer;
using System.Collections.Generic;
using UnityEngine;

namespace MillaBalancer
{
    [BepInPlugin("com.kuborro.plugins.fp2.millacube", "MillaCubeEditor", "2.0.1")]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<float> configRange;
        //public static ConfigEntry<float> configProjectiles;
        public static ConfigEntry<int> configCubes;
        public static ConfigEntry<bool> configAlwaysSpawn;


        private void Awake()
        {
            configRange = Config.Bind("General", "Range",(float) 50.0, new ConfigDescription("Set Milla's cube/projectile range. Value must be negative float. 0 = No range, 10 = Default, 50 = Basically infinite.", new AcceptableValueRange<float>(0f, 100f)));
            //configProjectiles = Config.Bind("General", "Cubes fired per cube", (float) 500.0, "Set how many cubes can Milla fire per one follower cube . Value must be a positive float. 0 = No cubes, 100 = Default, 500 = 5x the shots.");
            configCubes = Config.Bind("General", "Extra cubes spawned", 10, new ConfigDescription("Set how many cubes the cube powerup and guard (if enabled below) will spawn. 0 and negative values are not allowed.", new AcceptableValueRange<int>(1,30)));
            configAlwaysSpawn = Config.Bind("General", "Spawn extra cubes on guard", true, "Set if you want all the extra cubes to be spawned on guard.");

            if (configCubes.Value <= 0) configCubes.Value = 1;

            //HarmonyFileLog.Enabled = true;
            var harmony = new Harmony("com.kuborro.plugins.fp2.millacube");
            harmony.PatchAll(typeof(PatchObjectCreated));
            harmony.PatchAll(typeof(PatchPlayerStart));
            harmony.PatchAll(typeof(PatchCubeSpawn));
            harmony.PatchAll(typeof(PatchAddCube));
            harmony.PatchAll(typeof(PatchMultiCube));
        }
    }

class PatchObjectCreated
{
     [HarmonyPostfix]
     [HarmonyPatch(typeof(MillaCube), "ObjectCreated", MethodType.Normal)]
     static void Postfix(ref float ___explodeTimer)
     {
        if (FPSaveManager.character == FPCharacterID.MILLA)
            {
                ___explodeTimer = Plugin.configRange.Value;
            }
        }
    }
}

class PatchPlayerStart
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FPPlayer), "Start", MethodType.Normal)]
    static void Postfix(ref List<MillaMasterCube> ___millaCubes)
    {
            ___millaCubes = new List<MillaMasterCube>(Plugin.configCubes.Value);
    }
}

class PatchMultiCube
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FPPlayer), "Action_MillaMultiCube", MethodType.Normal)]
    static bool Prefix(FPPlayer __instance, ref List<MillaMasterCube> ___millaCubes)
    {
        int i = 0;
        int num = ___millaCubes.Count;
        while (i < num)
        {
            __instance.RemoveMillaCubeAt(num - i - 1);
            i++;
        }
        num = Mathf.Min(num + Plugin.configCubes.Value, Plugin.configCubes.Value);
        for (i = 0; i < num; i++)
        {
            MillaMasterCube millaMasterCube = (MillaMasterCube)FPStage.CreateStageObject(MillaMasterCube.classID, __instance.position.x, __instance.position.y);
            millaMasterCube.SetEnergyFull();
            millaMasterCube.floatStep = (float)i * -30f;
            __instance.AddMillaCube(millaMasterCube);
        }
        __instance.Action_PlaySoundUninterruptable(__instance.sfxMillaCubeSpawn);

        return false;
    }
}

class PatchCubeSpawn
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FPPlayer), "Action_MillaCubeSpawn", MethodType.Normal)]
    static void Postfix(FPPlayer __instance)
    {
        if (Plugin.configAlwaysSpawn.Value)
            __instance.Action_MillaMultiCube();
    }
}

class PatchAddCube
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FPPlayer), "AddMillaCube", MethodType.Normal)]
    static void Postfix(MillaMasterCube cube, ref bool __result, List<MillaMasterCube> ___millaCubes, FPPlayer __instance)
    {
        if (___millaCubes.Contains(cube))
        {
            __result = false;
            return;
        }
        if (cube == null)
        {
            __result = false;
            return;
        }
        if (___millaCubes.Count <= Plugin.configCubes.Value)
        {
            cube.parentObject = __instance;
            ___millaCubes.Add(cube);
            __instance.ReindexMillaCubes(true, true);
            __result = true;
        }
    }
}
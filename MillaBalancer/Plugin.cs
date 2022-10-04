using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MillaBalancer
{
    [BepInPlugin("com.kuborro.plugins.fp2.millacube", "Milla cube range editor", "1.0.0")]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<float> configRange;
        private void Awake()
        {
            configRange = Config.Bind("General", "Range",(float) -50.0, "Set Milla's cube/projectile range. Value must be negative float. 0 = No range, -10 = Default, -50 = Basically infinite.");

            if (configRange.Value > (float) 0.0)
            {
                configRange.Value = (float) -50.0;
            }
                //HarmonyFileLog.Enabled = true;
                var harmony = new Harmony("com.kuborro.plugins.fp2.millacube");
                harmony.PatchAll(typeof(Patch));
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

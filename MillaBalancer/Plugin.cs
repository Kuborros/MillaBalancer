using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace MillaBalancer
{
    [BepInPlugin("com.kuborro.plugins.fp2.millabal", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("FP2.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<float> configRange;
        private void Awake()
        {
            configRange = Config.Bind("General", "Range",(float) -50.0, "Set Milla's cube/projectile range. Value must be negative float. 0 = No range, -100 = Wraps whole screen vertically twice.");

            //HarmonyFileLog.Enabled = true;
            var harmony = new Harmony("com.kuborro.plugins.fp2.millabal");
            harmony.PatchAll(typeof(Patch));
        }
    }

    class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MillaCube), nameof(MillaCube.ObjectCreated), MethodType.Normal)]
        static void Postfix(ref float ___explodeTimer)
        {
            ___explodeTimer = Plugin.configRange.Value;
        }
    }
}

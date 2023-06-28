using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UnlimitedShots;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }
    
    [HarmonyPatch(typeof(FireProjectile), "Update")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> FireProjectileUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        Debug.Log("attempting to patch FireProjectile.Update()");
        
        var matcher = new CodeMatcher(instructions).MatchForward(false, 
            new CodeMatch(OpCodes.Ldstr, "Fire1"), 
            new CodeMatch(OpCodes.Call,
                AccessTools.Method(typeof(Input), "GetButtonDown", new[] { typeof(string) })),
            new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));
        
        matcher.Advance(1).SetOperandAndAdvance(AccessTools.Method(typeof(Input), "GetButton", new[] { typeof(string) }));
        return matcher.InstructionEnumeration();
    }
}
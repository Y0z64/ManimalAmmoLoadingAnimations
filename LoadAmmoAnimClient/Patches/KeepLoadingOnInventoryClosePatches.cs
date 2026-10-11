using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace Manimal.LoadAmmoAnim.Patches
{
    internal static class InventoryCloseState
    {
        public static bool Closing;
    }

    public class InventoryScreenCloseFlagPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("EFT.UI.InventoryScreen"), "Close");

        [PatchPrefix]
        public static void Prefix() => InventoryCloseState.Closing = true;

        [PatchFinalizer]
        public static void Finalizer() => InventoryCloseState.Closing = false;
    }

    public class SkipStopProcessesOnClosePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(Player.PlayerInventoryController), "StopProcesses");

        [PatchPrefix]
        public static bool Prefix()
        {
            if (!InventoryCloseState.Closing) return true;

            var player = Singleton<GameWorld>.Instance?.MainPlayer;
            var session = LoadAmmoAnimState.TryGet(player);
            return session == null || !session.IsLoading;
        }
    }
}

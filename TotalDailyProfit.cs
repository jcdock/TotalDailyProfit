using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.Persistence.Datas;
using ModManagerPhoneApp;
using FishNet.Managing.Statistic;
using Il2CppScheduleOne.Money;



namespace TotalDailyProfit
{
    public class TotalDailyProfit : MelonMod
    {
        

        public override void OnInitializeMelon()
        {
           TotalDailyProfit.KeybindCategory = MelonPreferences.CreateCategory("TotalDailyProfit_Main", "Total Daily Profit Keybind Settings");

           TotalDailyProfit.Keybind = TotalDailyProfit.KeybindCategory.CreateEntry<KeyCode>("Keybind", KeyCode.F9, "Keybind to open/close the Daily Summary window.",null,false,false,null,null);
           TotalDailyProfit.CrtlModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("CrtlModifier", false, "Use Ctrl + Keybind.", null, false, false, null, null);
           TotalDailyProfit.ShiftModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("ShiftModifier", false, "Use Shift + Keybind.", null, false, false, null, null);
           TotalDailyProfit.AltModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("AltModifier", false, "Use Alt + Keybind.", null, false, false, null, null);

           SubscribeToModManagerEvents();

           harmony = new HarmonyLib.Harmony("com.jcdock.totaldailyprofit");
           harmony.PatchAll();
           LoggerInstance.Msg(base.Info.Name +  " v" + base.Info.Version + " Initialized");
        }


        public override void OnDeinitializeMelon()
        {
            UnsubscribeFromModManagerEvents();
        }

        public override void OnUpdate()
        {
            var crtlModifier = TotalDailyProfit.CrtlModifier.Value;
            var shiftModifier = TotalDailyProfit.ShiftModifier.Value;
            var altModifier = TotalDailyProfit.AltModifier.Value;


       

            if (crtlModifier && shiftModifier && altModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();
                }
            }
            else if (crtlModifier && shiftModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();

                }
            }
            else if (crtlModifier && altModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();

                }
            }
            else if (shiftModifier && altModifier)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();

                }
            }
            else if (crtlModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();
                }
            }
            else if (shiftModifier)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();
                }
            }
            else if (altModifier)
            {
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();
                }
            }
            else
            {
                if (Input.GetKeyDown(TotalDailyProfit.Keybind.Value))
                {
                    ToggleDailySummary();
                }
            }
        }

        public static void ToggleDailySummary()
        {
            if (Il2CppScheduleOne.UI.DailySummary.Instance.IsOpen)
            {
                Il2CppScheduleOne.UI.DailySummary.Instance.Close();
              
            }
            else
            {
                Il2CppScheduleOne.UI.DailySummary.Instance.Open();
            }
        }


        [HarmonyPatch(typeof(Il2CppScheduleOne.UI.DailySummary), "DailySummary")]
        public class DailySummaryPatch
        {
            [HarmonyPatch("Open")]
            [HarmonyPostfix]
            public static void Postfix(Il2CppScheduleOne.UI.DailySummary __instance)
            {
                var dailySummary = __instance;
                float playerMoney = dailySummary.moneyEarnedByPlayer;
                float dealerMoney = dailySummary.moneyEarnedByDealers;
                float totalMoney = playerMoney + dealerMoney;

                dailySummary.DealerEarningsLabel.text = $"{MoneyManager.FormatAmount(dealerMoney,false,true)} ({MoneyManager.FormatAmount(totalMoney,false,true)})";

            }
        }

     



        // All credit for the following code goes to the author of the Mod Manager, Prowiler.
        private void UnsubscribeFromModManagerEvents()
        {
            //LoggerInstance.Msg("Attempting to unsubscribe from Mod Manager events...");
            try { ModManagerPhoneApp.ModSettingsEvents.OnPhonePreferencesSaved -= HandleSettingsUpdate; } catch { /* Ignore */ }
            try { ModManagerPhoneApp.ModSettingsEvents.OnMenuPreferencesSaved -= HandleSettingsUpdate; } catch { /* Ignore */ }
        }

        private void SubscribeToModManagerEvents()
        {
            // Subscribe to Phone App saves
            try
            {
                ModSettingsEvents.OnPhonePreferencesSaved += HandleSettingsUpdate;
            }
            // Catch potential runtime errors during subscription
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error subscribing to OnPhonePreferencesSaved: {ex}");
            }

            // Subscribe to Main Menu Config saves
            try
            {
                ModSettingsEvents.OnMenuPreferencesSaved += HandleSettingsUpdate; 
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error subscribing to OnMenuPreferencesSaved: {ex}");
            }
        }

        private void HandleSettingsUpdate() // Can be static if it only accesses static fields/methods
        {
        
            try
            {
                TotalDailyProfit.Keybind = TotalDailyProfit.KeybindCategory.GetEntry<KeyCode>("Keybind");
                TotalDailyProfit.CrtlModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("CrtlModifier");
                TotalDailyProfit.ShiftModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("ShiftModifier");
                TotalDailyProfit.AltModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("AltModifier");

          
           
                LoggerInstance.Msg("Settings reloaded successfully.");
            }
            catch (System.Exception ex) { LoggerInstance.Error($"Error applying updated settings after save: {ex}"); }
        }

        private static MelonPreferences_Category KeybindCategory;
        private static MelonPreferences_Entry<KeyCode> Keybind;
        private static MelonPreferences_Entry<bool> CrtlModifier;
        private static MelonPreferences_Entry<bool> ShiftModifier;
        private static MelonPreferences_Entry<bool> AltModifier;
        public HarmonyLib.Harmony harmony;


    }
}
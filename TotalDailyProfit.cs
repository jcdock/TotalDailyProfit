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



namespace TotalDailyProfit
{
    public class TotalDailyProfit : MelonMod
    {
        

        public override void OnInitializeMelon()
        {
           TotalDailyProfit.KeybindCategory = MelonPreferences.CreateCategory("TotalDailyProfit_Main", "Total Daily Profit Keybind Settings");

           TotalDailyProfit.Keybind = TotalDailyProfit.KeybindCategory.CreateEntry<string>("Keybind", "F9", "Keybind to open/close the Daily Summary window.",null,false,false,null,null);
           TotalDailyProfit.CrtlModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("CrtlModifier", false, "Use Ctrl + Keybind.", null, false, false, null, null);
           TotalDailyProfit.ShiftModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("ShiftModifier", false, "Use Shift + Keybind.", null, false, false, null, null);
           TotalDailyProfit.AltModifier = TotalDailyProfit.KeybindCategory.CreateEntry<bool>("AltModifier", false, "Use Alt + Keybind.", null, false, false, null, null);
       



            SubscribeToModManagerEvents();

       

            harmony = new HarmonyLib.Harmony("com.jcdock.totaldailyprofit");
           harmony.PatchAll();
           LoggerInstance.Msg(base.Info.Name +  " v" + base.Info.Version + " Initialized");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")

            {
                var _keybind = TotalDailyProfit.Keybind.Value;
             var isKeyValid = IsKeyBindValid(_keybind);
                if (isKeyValid == false)
                {
                    TotalDailyProfit.Keybind.Value = "F9";
                    LoggerInstance.Error("Keybind not set or is set to invalid key. Defaulting to F9.");
                }
               
            }
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
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();
                }
            }
            else if (crtlModifier && shiftModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();

                }
            }
            else if (crtlModifier && altModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();

                }
            }
            else if (shiftModifier && altModifier)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();

                }
            }
            else if (crtlModifier)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();
                }
            }
            else if (shiftModifier)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();
                }
            }
            else if (altModifier)
            {
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
                {
                    ToggleDailySummary();
                }
            }
            else
            {
                if (Input.GetKeyDown(ParseKeybind(TotalDailyProfit.Keybind.Value)))
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

        public static string FormatNumber(float number)
        {
            // Convert the float to a string
            string numberString = number.ToString("F0");
            // Split the string into whole and decimal parts
            string[] parts = numberString.Split('.');
            // Add commas to the whole part
            parts[0] = string.Format("{0:n0}", int.Parse(parts[0]));
            // Join the parts back together
            return string.Join(".", parts);
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
                //round total to 0 decimal places
                totalMoney = (float)Math.Round(totalMoney, 0);
                dealerMoney = (float)Math.Round(dealerMoney, 0);
                //format the numbers with commas
               var totalMoneyString = FormatNumber(totalMoney);
                //format the dealer money with commas
                var dealerMoneyString = FormatNumber(dealerMoney);


                dailySummary.DealerEarningsLabel.text = $"${dealerMoneyString} (${totalMoneyString})";

            }
        }

        private bool IsKeyBindValid(string keybind)
        {
            bool flag = Enum.TryParse<KeyCode>(keybind, out KeyCode keyCode);
            return flag;
        }
        private KeyCode ParseKeybind(string keybind)
        {
            bool flag = Enum.TryParse<KeyCode>(keybind, out KeyCode keyCode);
            KeyCode result;
            if (flag)
            {
                result = keyCode;
            }
            else
            {
                result = KeyCode.None;
            }
            return result;
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
                TotalDailyProfit.Keybind = TotalDailyProfit.KeybindCategory.GetEntry<string>("Keybind");
                TotalDailyProfit.CrtlModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("CrtlModifier");
                TotalDailyProfit.ShiftModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("ShiftModifier");
                TotalDailyProfit.AltModifier = TotalDailyProfit.KeybindCategory.GetEntry<bool>("AltModifier");

                var _keybind = TotalDailyProfit.Keybind.Value;
                var isKeyValid = IsKeyBindValid(_keybind);
                if (isKeyValid == false)
                {
                    TotalDailyProfit.Keybind.Value = "F9";
                    LoggerInstance.Error("Keybind not set or is set to invalid key. Defaulting to F9.");
                }
           
                LoggerInstance.Msg("Settings reloaded successfully.");
            }
            catch (System.Exception ex) { LoggerInstance.Error($"Error applying updated settings after save: {ex}"); }
        }

        private static MelonPreferences_Category KeybindCategory;
        private static MelonPreferences_Entry<string> Keybind;
        private static MelonPreferences_Entry<bool> debugMode;
        private static MelonPreferences_Entry<bool> CrtlModifier;
        private static MelonPreferences_Entry<bool> ShiftModifier;
        private static MelonPreferences_Entry<bool> AltModifier;
        public HarmonyLib.Harmony harmony;


    }
}
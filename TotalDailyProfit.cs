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
        public HarmonyLib.Harmony harmony;

        public override void OnInitializeMelon()
        {
           TotalDailyProfit.Category = MelonPreferences.CreateCategory("TotalDailyProfit_Main", "Total Daily Profit Settings");
           TotalDailyProfit.Keybind = TotalDailyProfit.Category.CreateEntry<string>("Keybind", "F9", "Keybind to open/close the Daily Summary window.",null,false,false,null,null);

           

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

            if (Input.GetKeyDown(this.ParseKeybind(TotalDailyProfit.Keybind.Value)))
            {


                Il2CppScheduleOne.UI.DailySummary dailySummary = Il2CppScheduleOne.UI.DailySummary.Instance;
                if (dailySummary != null)
                {
                    bool isOpen = dailySummary.IsOpen;
                    if (!isOpen)
                    {
                        dailySummary.Open();
                    }
                    else
                    {
                        dailySummary.Close();
                   
                    }
                }

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
               var dealerMoneyString = FormatNumber(dealerMoney);


                dailySummary.DealerEarningsLabel.text = $"${dealerMoneyString} (${totalMoneyString})";

            }
        }

        private KeyCode ParseKeybind(string keybind)
        {
            KeyCode keyCode;
            bool flag = Enum.TryParse<KeyCode>(keybind, out keyCode);
            KeyCode result;
            if (flag)
            {
                result = keyCode;
            }
            else
            {
                result = KeyCode.F9;
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
                ModSettingsEvents.OnMenuPreferencesSaved += HandleSettingsUpdate; // Can use the SAME handler
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error subscribing to OnMenuPreferencesSaved: {ex}");
            }
        }

        private void HandleSettingsUpdate() // Can be static if it only accesses static fields/methods
        {
        
            LoggerInstance.Msg("Mod Manager saved preferences. Reloading settings...");
            try
            {
                TotalDailyProfit.Keybind = TotalDailyProfit.Category.GetEntry<string>("Keybind");

                LoggerInstance.Msg("Settings reloaded successfully.");
            }
            catch (System.Exception ex) { LoggerInstance.Error($"Error applying updated settings after save: {ex}"); }
        }

        private static MelonPreferences_Category Category;
        private static MelonPreferences_Entry<string> Keybind;


    }
}
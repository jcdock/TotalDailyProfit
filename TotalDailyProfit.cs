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



namespace TotalDailyProfit
{


    public class TotalDailyProfit : MelonMod
    {
        public HarmonyLib.Harmony harmony;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("TotalDailyProfit Initialized");
            harmony = new HarmonyLib.Harmony("com.jcdock.totaldailyprofit");
            harmony.PatchAll();
        }

        //public override void OnUpdate()
        //{
        //    if (Input.GetKeyDown(KeyCode.F9))
        //    {
        //        Il2CppScheduleOne.UI.DailySummary.Instance.AddPlayerMoney(1000000f);
        //        Il2CppScheduleOne.UI.DailySummary.Instance.AddDealerMoney(90000f);       
        //        MelonLogger.Msg("Added to player and dealer money");
        //    }
       
        //}

    
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




    }
}
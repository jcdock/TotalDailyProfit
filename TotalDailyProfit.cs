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
                float total = playerMoney + dealerMoney;
                //round total to 0 decimal places
                total = (float)Math.Round(total, 0);
                dealerMoney = (float)Math.Round(dealerMoney, 0);


                dailySummary.DealerEarningsLabel.text = $"${dealerMoney} (${total})";

            }

        }




    }
}
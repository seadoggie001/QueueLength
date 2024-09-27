using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System;
using KitchenMods;
using KitchenData;
using KitchenLib.Preferences;

namespace KitchenQueue;

public class ShowCustomersCountPreferences<T> : KLMenu<T>
{
    private Option<int> Option;

    private Option<float> UpdateDelay;

    public ShowCustomersCountPreferences(Transform container, ModuleList module_list) : base(container, module_list){}

    public override void Setup(int player_id)
    {
        this.Option = new Option<int>(
            [0, 1],
            Main.PreferenceManager.GetPreference<PreferenceInt>(Main.QUEUE_LENGTH_ID).Get(),
            ["Expected Groups", "Expected Customers"]);

        AddLabel("Customers Display Type");
        Add<int>(this.Option).OnChanged += delegate (object _, int f)
        {
            Main.PreferenceManager.GetPreference<PreferenceInt>(Main.QUEUE_LENGTH_ID).Set(f);
            Main.PreferenceManager.Save();
        };


        this.UpdateDelay = new Option<float>(
            [0.5f, 1f, 1.5f, 2f, 3f, 4f, 5f, 10f],
            Main.PreferenceManager.GetPreference<PreferenceFloat>(Main.UPDATE_DELAY_ID).Get(),
            ["0.5", "1.0", "1.5", "2.0", "3.0", "4.0", "5.0", "10.0"]);

        AddLabel("Update Delay");
        AddInfo("\"Seed Affects Layout Only\" causes the customer count to be randomized. Set the delay for updates to prevent flickering.");
        Add(this.UpdateDelay).OnChanged += delegate (object _, float f)
        {
            Main.PreferenceManager.GetPreference<PreferenceFloat>(Main.UPDATE_DELAY_ID).Set(f);
            Main.PreferenceManager.Save();
        };

        New<SpacerElement>();
        New<SpacerElement>();

        AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
        {
            RequestPreviousMenu();
        });
    }
}
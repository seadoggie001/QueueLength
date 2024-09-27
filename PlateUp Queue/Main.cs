﻿using Kitchen;
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

public class Main : BaseMod {

    // guid must be unique and is recommended to be in reverse domain name notation
    // mod name that is displayed to the player and listed in the mods menu
    // mod version must follow semver e.g. "1.2.3"
    public const string MOD_GUID = "Seadoggie.PlateUp.CustomerQueues";
    public const string MOD_NAME = "Customer Queues";
    public const string MOD_VERSION = "1.0.0";
    public const string MOD_AUTHOR = "Seadoggie";
    public const string MOD_GAMEVERSION = ">=1.1.1";
    // Game version this mod is designed for in semver
    // e.g. ">=1.1.1" current and all future
    // e.g. ">=1.1.1 <=1.2.3" for all from/until

    public const string QUEUE_LENGTH_ID = "QueueLength";
    public const int QUEUE_LENGTH_INITIAL = 1;
    public const string UPDATE_DELAY_ID = "updateDelay";
    public const float UPDATE_DELAY_INITIAL = 2f;

    public static PreferenceManager PreferenceManager;

    public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

    protected override void OnPostActivate(Mod mod)
    {
        base.OnPostActivate(mod);
        // For log file output so the official plateup support staff can identify if/which a mod is being used
        LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

        PreferenceManager = new(MOD_GUID);
        PreferenceManager.RegisterPreference(new PreferenceInt(QUEUE_LENGTH_ID, QUEUE_LENGTH_INITIAL));
        PreferenceManager.RegisterPreference(new PreferenceFloat(UPDATE_DELAY_ID, UPDATE_DELAY_INITIAL));
        PreferenceManager.Load();
        LogInfo($"Registered preference {MOD_GUID}:{QUEUE_LENGTH_ID} -- value to follow");
        LogInfo(PreferenceManager.GetPreference<PreferenceInt>(QUEUE_LENGTH_ID).Get());
        SetupPreferences();
    }

    protected override void OnInitialise()
    {
        base.OnInitialise();
        try
        {
            World.GetExistingSystem<ParametersDisplayView.UpdateView>().Enabled = false;
        }
        catch (NullReferenceException)
        {
            LogInfo("Could not disable system Kitchen.ParametersDisplayView.UpdateView!");
        }
    }

    private void SetupPreferences()
    {
        //Setting Up For Pause Menu
        Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
        {
            args.Menus.Add(typeof(ShowCustomersCountPreferences<PauseMenuAction>), new ShowCustomersCountPreferences<PauseMenuAction>(args.Container, args.Module_list));
        };
        ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(ShowCustomersCountPreferences<PauseMenuAction>), typeof(PauseMenuAction));
    }

    protected override void OnUpdate()
    {
    }

    #region Logging
    // You can remove this, I just prefer a more standardized logging
    public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
    public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
    public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
    public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
    public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
    public static void LogError(object _log) { LogError(_log.ToString()); }
    #endregion

}

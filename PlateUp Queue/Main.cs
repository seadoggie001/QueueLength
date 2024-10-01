﻿using Kitchen;
using KitchenLib;
using System.Reflection;
using KitchenMods;
using PreferenceSystem;
using Unity.Entities;
using Unity.Collections;

namespace KitchenQueue;

public class Main : BaseMod {

    #region KitchenLib.BaseMod Info
    /// <summary>
    /// guid must be unique and is recommended to be in reverse domain name notation 
    /// </summary>
    public const string MOD_GUID = "Seadoggie.PlateUp.CustomerQueues";
    /// <summary>
    /// mod name that is displayed to the player and listed in the mods menu
    /// </summary>
    public const string MOD_NAME = "Customer Queues";
    /// <summary>
    /// mod version must follow semver e.g. "1.2.3"
    /// </summary>
    public const string MOD_VERSION = "0.0.1";
    public const string MOD_AUTHOR = "Seadoggie";
    /// <summary>
    /// Game version this mod is designed for in semver
    /// </summary>
    /// <remarks>
    /// e.g. ">=1.1.1" current and all future
    /// e.g. ">=1.1.1 <=1.2.3" for all from/until
    /// </remarks>
    public const string MOD_GAMEVERSION = ">=1.1.1";
    #endregion

    private const string MaxQGroupPref = "MaxQueuedGroups";
    private const string MaxQCustPref = "MaxQueuedCustomers";
    public static PreferenceSystemManager PrefManager;
    public static Main Instance;

    public static int QueuedGroups = 0;
    public static int QueuedCustomers = 0;
    public static int MaxQueuedGroups = 0;
    public static int MaxQueuedCustomers = 0;

    private EntityQuery QueuedGroupsQuery;
    private EntityQuery QueuedCustomersQuery;

    public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { 
        Instance = this;
    }

    /// <summary>
    /// Called when PlateUp is launched
    /// </summary>
    /// <param name="mod"></param>
    protected override void OnPostActivate(Mod mod)
    {
        base.OnPostActivate(mod);
        // For log file output so the official plateup support staff can identify if/which a mod is being used
        LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

        PrefManager = new(MOD_GUID, MOD_NAME);
        PrefManager.AddProperty(MaxQGroupPref, 0);
        PrefManager.AddProperty(MaxQCustPref, 0);
        PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        MaxQueuedGroups = PrefManager.Get<int>(MaxQGroupPref);
        MaxQueuedCustomers = PrefManager.Get<int>(MaxQCustPref);
    }

    protected override void OnInitialise()
    {
        base.OnInitialise();

        RegisterMenu<QueueMenu>();

        QueuedGroupsQuery = GetEntityQuery(new QueryHelper().All(typeof(CCustomerGroup), typeof(CGroupPhaseQueue)));
        QueuedCustomersQuery = GetEntityQuery(new QueryHelper().All(typeof(CWaitingGroup)));
    }

    protected override void OnUpdate()
    {
        QueuedGroups = QueuedGroupsQuery.CalculateEntityCount();
        QueuedCustomers = 0;
        NativeArray<CWaitingGroup> nativeArray = QueuedCustomersQuery.ToComponentDataArray<CWaitingGroup>(Allocator.Temp);
        foreach(CWaitingGroup item in nativeArray){
            QueuedCustomers += item.MemberCount;
        }
        if(QueuedGroups > MaxQueuedGroups){
            MaxQueuedGroups = QueuedGroups;
            PrefManager.Set(MaxQGroupPref, MaxQueuedGroups);
        }
        if(QueuedCustomers > MaxQueuedCustomers){
            MaxQueuedCustomers = QueuedCustomers;
            PrefManager.Set(MaxQCustPref, MaxQueuedCustomers);
        } 
    }

    #region Logging
    public static void LogInfo(string _log) { Instance.Log($"[{MOD_NAME}] " + _log); }
    public static void LogWarning(string _log) { Instance.Warning($"[{MOD_NAME}] " + _log); }
    public static void LogError(string _log) { Instance.Error($"[{MOD_NAME}] " + _log); }
    public static void LogInfo(object _log) { Instance.Log(_log.ToString()); }
    public static void LogWarning(object _log) { Instance.Warning(_log.ToString()); }
    public static void LogError(object _log) { Instance.Error(_log.ToString()); }
    #endregion

}

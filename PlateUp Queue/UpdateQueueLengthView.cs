using Kitchen;
using Unity.Entities;
using Unity.Collections;
using KitchenData;
using KitchenQueue.Patches;
using KitchenLib.Preferences;

namespace KitchenQueue;

public class UpdateQueueLengthView : ParametersDisplayView.UpdateView
{
    EntityQuery Players;
    EntityQuery Views;

    private bool _forceTextUpdateToggle = true;

    protected override void Initialise()
    {
        base.Initialise();
        Players = GetEntityQuery(typeof(CPlayer));
        Views = GetEntityQuery(new QueryHelper()
            .All(typeof(CLinkedView),
                    typeof(CParametersDisplay)));
        RequireSingletonForUpdate<SKitchenParameters>();
    }

    protected override void OnUpdate()
    {
        bool showCustomers;
        try{
            showCustomers = Main.PreferenceManager.GetPreference<PreferenceInt>(Main.QUEUE_LENGTH_ID).Get() == 1;
        }catch(Exception){
            showCustomers = true;
        }

        string correctText = showCustomers ? ParametersDisplayView_Patch.ExpectedCustomersText : ParametersDisplayView_Patch.ExpectedGroupsText;
        bool isApproximation = showCustomers && !Preferences.Get<bool>(Pref.SeedsAffectEverything);

        if (correctText == ParametersDisplayView_Patch.DisplayedText && isApproximation == ParametersDisplayView_Patch.IsApproximated)
        {
            _forceTextUpdateToggle = false;
        }
        else
        {
            _forceTextUpdateToggle = !_forceTextUpdateToggle;
        }

        if (showCustomers && Has<SIsNightTime>())
        {
            KitchenParameters sKitchenParameters = GetSingleton<SKitchenParameters>().Parameters;
            bool isNight = HasSingleton<SIsNightTime>();

            int standardCustomers = 0;
            int rushCustomers = 0;

            if (!_forceTextUpdateToggle)
            {
                standardCustomers = QueueLengthController.StandardCustomers;
                rushCustomers = QueueLengthController.RushCustomers;
            }

            Entity entity = GetEntity<SGlobalStatusList>();
            if (!base.EntityManager.HasComponent<CDecorationScore>(entity))
            {
                base.EntityManager.AddBuffer<CDecorationScore>(entity);
            }
            DynamicBuffer<CDecorationScore> buffer = GetBuffer<CDecorationScore>(entity);
            DecorationValues decoValues = default(DecorationValues);
            for (int i = 0; i < buffer.Length; i++)
            {
                CDecorationScore cDecorationScore = buffer[i];
                decoValues[cDecorationScore] = cDecorationScore.Value;
            }

            NativeArray<Entity> linkedViews = Views.ToEntityArray(Allocator.Temp);

            foreach (Entity e in linkedViews)
            {
                if (Require<CLinkedView>(e, out CLinkedView linked_view))
                    SendUpdate(linked_view, new ParametersDisplayView.ViewData
                    {
                        IsNight = isNight,
                        ExpectedGroupCount = standardCustomers,
                        MinimumGroupSize = sKitchenParameters.MinimumGroupSize,
                        MaximumGroupSize = sKitchenParameters.MaximumGroupSize,
                        Decoration = decoValues,
                        ExtraGroups = rushCustomers
                    });
            }
        }
        else
        {
            base.OnUpdate();
        }
    }
}
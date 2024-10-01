using KitchenLib.DevUI;
using UnityEngine;

namespace KitchenQueue;

public class QueueMenu : BaseUI
{
    protected GUIStyle LabelLeftStyle { get; private set; }
    protected GUIStyle LabelRightStyle { get; private set; }
    protected GUIStyle LabelCenterStyle { get; private set; }

    bool _isInit = false;

    protected Texture2D Background { get; private set; }

    public QueueMenu(){
        ButtonName = "Queue Info";
    }

    public sealed override void OnInit()
    {
        _isInit = true;
        Background = new Texture2D(64, 64);
        UnityEngine.Color grayWithAlpha = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 0.6f);
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                Background.SetPixel(x, y, grayWithAlpha);
            }
        }
        Background.Apply();
        OnInitialise();
    }

    public sealed override void Setup()
    {
        if(!_isInit) OnInit();

        LabelLeftStyle ??= new GUIStyle(GUI.skin.label){
            alignment = TextAnchor.MiddleLeft,
            padding = new(10, 0, 0, 0),
            stretchWidth = true,
        };

        LabelCenterStyle ??= new GUIStyle(GUI.skin.label){
            alignment = TextAnchor.MiddleCenter,
            stretchWidth = true,
        };

        LabelRightStyle ??= new GUIStyle(GUI.skin.label){
            alignment = TextAnchor.MiddleRight,
            stretchWidth = true,
        };

        OnSetup();
    }

    protected virtual void OnInitialise(){

    }

    private static readonly float windowWidth = 770f;
    private static readonly float windowHeight = 1050f;
    protected virtual void OnSetup(){
        
        float labelColumn = windowWidth * 0.4f;
        float valueColumn = windowWidth * 0.1f;

        GUILayout.BeginArea(new Rect(10f, 10f, windowWidth, windowHeight));
        GUI.DrawTexture(new Rect(0f, 0f, windowWidth, windowHeight), Background);
        
        GUILayout.Label("Queue Info", LabelCenterStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Queued Groups", LabelLeftStyle, GUILayout.Width(labelColumn));
        GUILayout.TextField(Main.QueuedGroups.ToString(), LabelRightStyle, GUILayout.Width(valueColumn));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Queued Customers", LabelLeftStyle, GUILayout.Width(labelColumn));
        GUILayout.TextField(Main.QueuedCustomers.ToString(), LabelRightStyle, GUILayout.Width(valueColumn));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Max Queued Groups", LabelLeftStyle, GUILayout.Width(labelColumn));
        GUILayout.TextField(Main.MaxQueuedGroups.ToString(), LabelRightStyle, GUILayout.Width(valueColumn));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Max Queued Customers", LabelLeftStyle, GUILayout.Width(labelColumn));
        GUILayout.TextField(Main.MaxQueuedCustomers.ToString(), LabelRightStyle, GUILayout.Width(valueColumn));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

}

using System.Globalization;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace DontStarveMod;

public class DontStarveMod : ModBehaviour
{
    public static DontStarveMod Instance { get; private set; }

    public const int HUNGER_TIME_START = 300;
    public const int CRITICAL_THRESHOLD = 60;
    
    private static float _hungerTimeLeft = HUNGER_TIME_START;
    private static bool _criticalThresholdMet = false;
    private static bool _death = false;
    private static OWScene _currentScene;
    
    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        // Starting here, you'll have access to OWML's mod helper.
        ModHelper.Console.WriteLine($"My mod {nameof(DontStarveMod)} is loaded!", MessageType.Success);
        
        // Example of accessing game code.
        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            _currentScene = loadScene;
            ResetHunger();
        };
    }

    private void Update()
    {
        if (OWInput.IsNewlyPressed(InputLibrary.rollMode))
        {
            ResetHunger();
            _hungerTimeLeft = 60f;
        }
        
        if (_death || _currentScene != OWScene.SolarSystem)
            return;
        
        _hungerTimeLeft -= Time.unscaledDeltaTime * Time.timeScale;  // Note: I have some doubts on the scalability of the Time.deltaTime variable.

        if (!_criticalThresholdMet && _hungerTimeLeft <= CRITICAL_THRESHOLD)
        {
            _criticalThresholdMet = true;
            NotificationManager.SharedInstance.PostNotification(
                new NotificationData(
                    NotificationTarget.Player,
                    "60 SECONDS UNTIL STARVATION",
                    5f,
                    false));
            Locator.GetPlayerAudioController().PlaySuitWarning();
        }

        if (_hungerTimeLeft <= 0f)
        {
            _death = true;
            Locator.GetDeathManager().KillPlayer(DeathType.Asphyxiation);
        }
    }

    public void ResetHunger()
    {
        _hungerTimeLeft = HUNGER_TIME_START;
        _criticalThresholdMet = false;
        _death = false;
    }

    public static void Log(string msg, MessageType type) => Instance.ModHelper.Console.WriteLine(msg, type);
}

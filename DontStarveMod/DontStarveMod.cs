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

    public static int HungerStartingValue { get; private set; }
    public static int HungerRestorationValue { get; private set; }

    private static float _hungerTimeLeft = 0f;
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
        HungerStartingValue = ModHelper.Config.GetSettingsValue<int>("Starting Value");
        HungerRestorationValue = ModHelper.Config.GetSettingsValue<int>("Restoration Value");
        
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

        if (!_criticalThresholdMet && _hungerTimeLeft <= 60)
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

    public void ResetHunger(bool fullRefill = true)
    {
        _hungerTimeLeft += HungerRestorationValue;
        if (fullRefill || _hungerTimeLeft > HungerStartingValue)
            _hungerTimeLeft = HungerStartingValue;
        _criticalThresholdMet = false;
        _death = false;
    }

    public static void Log(string msg, MessageType type) => Instance.ModHelper.Console.WriteLine(msg, type);
}

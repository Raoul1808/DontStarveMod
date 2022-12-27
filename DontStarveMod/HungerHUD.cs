using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DontStarveMod;

[HarmonyPatch]
public class HungerHUD
{
    private static GameObject _hungerBarGroup;
    private static float _hungerFraction = 1f;
    private static Image _hungerBarImage;
    private static Image _hungerBarArrowImage;
    private static float _hungerArrowOriginRotation;
    private static GameObject _hungerBarBackground;

    [HarmonyPatch(typeof(HUDCanvas), "Start")]
    [HarmonyPostfix]
    public static void HUDCanvas_Start_Postfix(HUDCanvas __instance)
    {
        var gaugeBoost = GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/GaugeGroup/Gauge/Boost");
        _hungerBarGroup = GameObject.Instantiate(gaugeBoost, gaugeBoost.transform.parent);
        _hungerBarGroup.name = "Hunger";
        var hungerBar = _hungerBarGroup.transform.GetChild(0).gameObject;
        hungerBar.name = "HungerBar";
        _hungerBarImage = hungerBar.GetComponent<Image>();
        var hungerArrow = _hungerBarGroup.transform.GetChild(1).gameObject;
        _hungerBarArrowImage = hungerArrow.GetComponent<Image>();
        __instance.InitArrowIndicator(_hungerBarImage, _hungerBarArrowImage.transform.parent.gameObject, ref _hungerArrowOriginRotation);

        var gaugeBackground = GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/GaugeGroup/BackgroundBoost");
        _hungerBarBackground = GameObject.Instantiate(gaugeBackground, gaugeBackground.transform.parent);
        _hungerBarBackground.name = "BackgroundHunger";
        
        _hungerBarBackground.transform.localRotation = Quaternion.Euler(0f, 0f, 210f);
        _hungerBarBackground.transform.localScale = new Vector3(1.16f, -1.16f, 1f);
        _hungerBarGroup.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
        _hungerBarGroup.transform.localScale = new Vector3(1.16f, -1.16f, 1f);
    }

    [HarmonyPatch(typeof(HUDCanvas), "Update")]
    [HarmonyPostfix]
    public static void HUDCanvas_Update_Postfix()
    {
        float fraction = DontStarveMod.HungerFraction;
        if (OWMath.ApproxEquals(_hungerFraction, fraction, 0.01f))
            return;
        _hungerFraction = fraction;
        _hungerBarImage.fillAmount = 0.75f + _hungerFraction * 0.25f;
        float num = (1f - fraction) * 90f;
        if (!_hungerBarImage.fillClockwise)
            num *= -1f;
        Vector3 angles = new Vector3(0f, 0f, _hungerArrowOriginRotation + num);
        _hungerBarArrowImage.transform.localEulerAngles = angles;
    }
}

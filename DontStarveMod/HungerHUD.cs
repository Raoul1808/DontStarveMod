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

    [HarmonyPatch(typeof(HUDCanvas), "Start")]
    [HarmonyPostfix]
    public static void HUDCanvas_Start_Postfix(HUDCanvas __instance)
    {
        GameObject gaugeFuel = GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/GaugeGroup/Gauge/Fuel");
        _hungerBarGroup = GameObject.Instantiate(gaugeFuel, gaugeFuel.transform.parent);
        _hungerBarGroup.name = "Hunger";
        var hungerBar = _hungerBarGroup.transform.GetChild(0).gameObject;
        hungerBar.name = "HungerBar";
        _hungerBarImage = hungerBar.GetComponent<Image>();
        var hungerArrow = _hungerBarGroup.transform.GetChild(1).gameObject;
        _hungerBarArrowImage = hungerArrow.GetComponent<Image>();
        var pos = _hungerBarGroup.transform.position;
        _hungerBarGroup.transform.position = new Vector3(pos.x + 0.5f, pos.y, pos.z);
        __instance.InitArrowIndicator(_hungerBarImage, _hungerBarArrowImage.transform.parent.gameObject, ref _hungerArrowOriginRotation);
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

        // const float SPEED = 0.05f;
        // float x = 0f;
        // float y = 0f;
        // if (Keyboard.current[Key.L].isPressed)
        //     x += SPEED;
        // if (Keyboard.current[Key.J].isPressed)
        //     x -= SPEED;
        // if (Keyboard.current[Key.K].isPressed)
        //     y -= SPEED;
        // if (Keyboard.current[Key.I].isPressed)
        //     y += SPEED;
        // var pos = _hungerBarGroup.transform.position;
        // if (Keyboard.current[Key.O].isPressed)
        //     DontStarveMod.Log("X: " + pos.x + " ; Y : " + pos.y, MessageType.Success);
        // _hungerBarGroup.transform.position = new Vector3(pos.x + x, pos.y + y, pos.z);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] BatteryCharge _batteryCharge;
    [SerializeField] Image _batteryFill;
    [SerializeField] TextMeshProUGUI _batteryText;

    void Update()
    {
        _batteryFill.fillAmount = _batteryCharge.BatteryLevel / 100f;
        _batteryText.text = $"{(int)_batteryCharge.BatteryLevel}%";
    }
}
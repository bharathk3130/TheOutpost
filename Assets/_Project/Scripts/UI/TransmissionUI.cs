using TMPro;
using UnityEngine;

public class TransmissionUI : MonoBehaviour
{
    [SerializeField] Transmitter _transmitter;
    [SerializeField] TextMeshProUGUI _timerText;
    
    void Update()
    {
        float timeLeft = _transmitter.TimeLeft;
        int minutes = (int)(timeLeft / 60);
        int seconds = (int)(timeLeft % 60);

        string extraZero = seconds < 10 ? "0" : "";

        _timerText.text = $"{minutes}:{extraZero}{seconds}";
    }
}

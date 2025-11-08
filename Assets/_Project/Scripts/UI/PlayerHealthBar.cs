using Clickbait.Utilities;
using UnityEngine;

// The player UI uses a mask so this health bar simply pushes the fill transform backwards rather than changing the fill amount
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Health _playerHealth;
    [SerializeField] RectTransform _fillTransform;

    float _totalOffset = 383f;

    void Start()
    {
        _playerHealth.CurrentHealthPercent.AddListener(UpdateHealthBar);
    }

    void UpdateHealthBar(float healthPercent)
    {
        float offset = (1 - Mathf.Clamp01(healthPercent)) * _totalOffset;
        _fillTransform.localPosition = _fillTransform.localPosition.With(x: -offset);
    }
}

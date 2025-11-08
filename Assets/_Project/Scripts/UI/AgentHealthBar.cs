using UnityEngine;
using UnityEngine.UI;

public class AgentHealthBar : MonoBehaviour
{
    [SerializeField] Health _health;
    [SerializeField] Image _fillImage;
    [SerializeField] AgentDeath _agentDeath;

    void Start()
    {
        _health.CurrentHealthPercent.AddListener(UpdateHealthBar);
        _agentDeath.OnDeath += () => gameObject.SetActive(false);
    }

    void UpdateHealthBar(float healthPercent)
    {
        _fillImage.fillAmount = healthPercent;
    }
}
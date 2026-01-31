using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private CanvasGroup _rootGroup;
    [SerializeField] private TextMeshProUGUI _moneyTmPro, _addendTmPro;

    private FadeableCanvasGroup _rootFader;

    public void Init()
    {
        _rootFader = new FadeableCanvasGroup(_rootGroup, fadeDuration: 0.5f);
        _rootFader.FadeInstantly(false);
        Game.Instance.Player.CoinsCollected += OnPlayerCollectedCoins;
    }

    private void OnDestroy()
    {
        Game.Instance.Player.CoinsCollected -= OnPlayerCollectedCoins;
    }

    private void OnPlayerCollectedCoins(int amount)
    {
        Debug.Log($"collected {amount}");
    }
}
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private CanvasGroup _rootGroup;
    [SerializeField] private TextMeshProUGUI _moneyTmPro, _addendTmPro;

    private FadeableCanvasGroup _rootFader;
    private Tween _fadeTween;
    private MoneyTransfer _moneyTransfer;

    public void Init()
    {
        _rootFader = new FadeableCanvasGroup(_rootGroup, fadeDuration: 0.5f);
        _rootFader.FadeInstantly(false);
        Game.Instance.Player.CoinsCollected += OnPlayerCollectedCoins;
    }

    public void Unload()
    {
        Game.Instance.Player.CoinsCollected -= OnPlayerCollectedCoins;   
    }

    private void OnPlayerCollectedCoins(int amount)
    {
        amount = UnityEngine.Random.Range(25, 55);
        Debug.Log($"collected {amount}");
        if (_fadeTween is { active: true })
        {
            _fadeTween.Kill();
        }
        _fadeTween = _rootFader.Fade(true);

        if (_moneyTransfer is null)
        {
            _moneyTransfer = new MoneyTransfer(amount, bankAmount: App.Instance.UserData.Money);
            _moneyTransfer.UpdateUICallback = UpdateCounters;
            
            DOTween.Kill(this);
            Sequence sequence = DOTween.Sequence().SetId(this);
            sequence.AppendInterval(2f);
            sequence.Append(_moneyTransfer.TransferGold(duration: 2f));
            sequence.OnComplete(OnTransferComplete);
        }
        else
        {
            _moneyTransfer.Add(amount);
        }
        
        UpdateCounters(_moneyTransfer.BankAmountSnapShot, _moneyTransfer.Addend);
        return;

        void OnTransferComplete()
        {
            _moneyTransfer = null;
            _fadeTween = _rootFader.Fade(fadeIn: false);
        }
    }

    private void UpdateCounters(int bank, int addend)
    {
        _moneyTmPro.text = bank.ToString();
        _addendTmPro.text = $"+{addend}";
    }

    private class MoneyTransfer
    {
        public MoneyTransfer(int amount, int bankAmount)
        {
            Addend = amount;
            BankAmountSnapShot = bankAmount;
        }

        public void Add(int amount)
        {
            Addend += amount;
        }
        
        public Tween TransferGold(float duration)
        {
            return DOVirtual.Float(0f, 1f, duration, OnUpdate).SetEase(Ease.OutSine);

            void OnUpdate(float progress)
            {
                int remainingAddend = Mathf.RoundToInt(Mathf.Lerp(Addend, 0, progress));
                UpdateUICallback?.Invoke(BankAmountSnapShot + (Addend - remainingAddend), remainingAddend);
            }
        }
        
        public Action<int, int> UpdateUICallback { get; set; }
        public int Addend { get; private set; }
        public int BankAmountSnapShot { get; }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _shopButton;
    
    void Start()
    {
        _startGameButton.onClick.AddListener(OnStartGamePressed);
        _shopButton.onClick.AddListener(OnShopPressed);

        void OnStartGamePressed()
        {
            Debug.Log("Start Game");
            SceneTransitions.Instance.StartGame();
        }

        void OnShopPressed()
        {
            Debug.Log("Shop");
        }
    }
}

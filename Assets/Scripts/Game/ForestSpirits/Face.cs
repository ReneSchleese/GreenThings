using UnityEngine;

namespace ForestSpirits
{
    public class Face : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _rendererLeft, _rendererRight;
        [SerializeField] private Sprite _sprite1, _sprite2;
        
        private void Start()
        {
            RandomizeFace();
        }

        private void RandomizeFace()
        {
            _rendererLeft.sprite = Random.Range(0, 2) == 0 ? _sprite1 : _sprite2;
            _rendererRight.sprite = Random.Range(0, 2) == 0 ? _sprite1 : _sprite2;
        }
    }
}
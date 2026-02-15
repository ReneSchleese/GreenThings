using UnityEngine;

public class CarouselBottle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _planeRenderer;
    
    public void SetTexture(Texture2D texture)
    {
        _planeRenderer.material.mainTexture = texture;
    }
    
    public string Url { get; set; }
}
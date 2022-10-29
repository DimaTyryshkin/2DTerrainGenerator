using UnityEngine;

namespace FieldGenerator
{
    public class Square : MonoBehaviour
    {
        [SerializeField] SpriteRenderer thisRenderer;
        [SerializeField] SpriteRenderer headRenderer;

        public void SetColor(Color color)
        {
            gameObject.SetActive(true);
            thisRenderer.color = color;
        }
        
        public void SetHeadColor(Color color)
        {
            gameObject.SetActive(true);
            
            headRenderer.color = color;
            headRenderer.gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

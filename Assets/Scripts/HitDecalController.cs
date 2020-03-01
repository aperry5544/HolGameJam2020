using TMPro;
using UnityEngine;

public class HitDecalController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh = null;

    private Vector3 large = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 small = new Vector3(0.15f, 0.15f, 0.15f);

    public void Activate(string text, Vector2 position, bool largeSize)
    {
        if(largeSize)
        {
            transform.localScale = large;
        }
        else
        {
            transform.localScale = small;
        }
        textMesh.text = text;
        gameObject.transform.position = position;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

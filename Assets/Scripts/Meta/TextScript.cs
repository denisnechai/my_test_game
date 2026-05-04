using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    public LayerMask blockingLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.parent.transform.parent.transform.position.y <= 0)
        {
            gameObject.GetComponent<TMP_Text>().rectTransform.position = gameObject.transform.parent.transform.parent.transform.position;
            gameObject.GetComponent<TMP_Text>().rectTransform.localScale = Vector3.one * (0.75f / (0.75f + (gameObject.GetComponent<TMP_Text>().text.Length / 4f)));
        }
        else
        {
            if (gameObject.transform.parent.transform.parent.transform.localScale == Vector3.one)
            {
                gameObject.GetComponent<TMP_Text>().rectTransform.position = gameObject.transform.parent.transform.parent.transform.position + Vector3.down * 0.3f - Vector3.down * 0.125f / (float)gameObject.GetComponent<TMP_Text>().text.Length;
                gameObject.GetComponent<TMP_Text>().rectTransform.localScale = Vector3.one * (0.66f / (0.5f + (gameObject.GetComponent<TMP_Text>().text.Length / 2f)));
            }
            else
            {
                gameObject.GetComponent<TMP_Text>().rectTransform.position = gameObject.transform.parent.transform.parent.transform.position + Vector3.up * 0.3f - Vector3.up * 0.125f / (float)gameObject.GetComponent<TMP_Text>().text.Length;
                gameObject.GetComponent<TMP_Text>().rectTransform.localScale = new Vector3(1, -1, 1) * (0.66f / (0.5f + (gameObject.GetComponent<TMP_Text>().text.Length / 2f)));
            }
        }
    }
}

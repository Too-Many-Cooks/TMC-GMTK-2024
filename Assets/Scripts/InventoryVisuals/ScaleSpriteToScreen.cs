using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScaleSpriteToScreen : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Screen.width / (1f * rectTransform.sizeDelta.x),
                                           Screen.height / (1f * rectTransform.sizeDelta.y),
                                           0f);

    }


}

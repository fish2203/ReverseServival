using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickIcon : MonoBehaviour
{
    // 인스펙터에서 할당
    public Sprite normalIcon;
    public Sprite redIcon;

    Image image;

    public RectTransform fillMaskRectTransform;



    float timer = 0f;
    readonly float coolTime = 15.0f;
    readonly float blinkPeriod = 0.25f; // 활성화 시, 우클릭아이콘이 깜빡이는 주기

    private void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        timer += Time.deltaTime;

        // 활성화 상태
        if (timer > coolTime)
        {
            // 아이콘
            image.sprite = (timer % blinkPeriod >= (blinkPeriod / 2f))
                ? normalIcon : redIcon;

            // fillMask
            fillMaskRectTransform.sizeDelta = new Vector2(100f, 100f);

            // 우클릭하면 버프 적용
            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(GameManager.instance.BuffMonsters());
                timer = 0f;
            }
        }
        // 비활성화 상태
        else
        {
            // 아이콘
            image.sprite = normalIcon;

            // fillMask
            fillMaskRectTransform.sizeDelta = new Vector2(100f, (timer / coolTime) * 100f);
        }
    }
}

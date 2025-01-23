using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightClickIcon : MonoBehaviour
{
    // �ν����Ϳ��� �Ҵ�
    public Sprite normalIcon;
    public Sprite redIcon;

    Image image;

    public RectTransform fillMaskRectTransform;



    float timer = 0f;
    readonly float coolTime = 15.0f;
    readonly float blinkPeriod = 0.25f; // Ȱ��ȭ ��, ��Ŭ���������� �����̴� �ֱ�

    private void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        timer += Time.deltaTime;

        // Ȱ��ȭ ����
        if (timer > coolTime)
        {
            // ������
            image.sprite = (timer % blinkPeriod >= (blinkPeriod / 2f))
                ? normalIcon : redIcon;

            // fillMask
            fillMaskRectTransform.sizeDelta = new Vector2(100f, 100f);

            // ��Ŭ���ϸ� ���� ����
            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(GameManager.instance.BuffMonsters());
                timer = 0f;
            }
        }
        // ��Ȱ��ȭ ����
        else
        {
            // ������
            image.sprite = normalIcon;

            // fillMask
            fillMaskRectTransform.sizeDelta = new Vector2(100f, (timer / coolTime) * 100f);
        }
    }
}

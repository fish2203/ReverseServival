using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIcon : MonoBehaviour
{
    // �ν����Ϳ��� �Ҵ� �ʿ�
    public Monster.Type myMonsterType; // �� �������� �ð� �ִ� ����
    public Sprite unactiveSprite; // ���õ��� ����, ����� ��������Ʈ
    public Sprite activeSprite; // ���õ�(��Ŭ�� �� ��ȯ�Ǵ�) ������ ��������Ʈ

    private Image uiImage;

    private void Start()
    {
        uiImage = GetComponent<Image>();
    }
    public void SetSprite(bool isActive)
    {
        uiImage.sprite = isActive ? activeSprite : unactiveSprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIcon : MonoBehaviour
{
    // 인스펙터에서 할당 필요
    public Monster.Type myMonsterType; // 이 아이콘이 맡고 있는 몬스터
    public Sprite unactiveSprite; // 선택되지 않은, 평소의 스프라이트
    public Sprite activeSprite; // 선택된(좌클릭 시 소환되는) 상태의 스프라이트

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

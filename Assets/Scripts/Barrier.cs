using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Sprite normalBarrier;
    public Sprite brokenBarrier;

    public GameObject buffOra;

    SpriteRenderer spriteRenderer;
    int durability; // ³»±¸µµ

    private void Start()
    {
        durability = 20;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normalBarrier;
    }
    
    private void Update()
    {
        transform.position = GameManager.instance.hero.transform.position + new Vector3(0, 1.15f, 0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "S_Monster")
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if(monster)
                monster.Death();
            else
                Destroy(collision.gameObject);
            durability--;

            if (durability <= 5)
            {
                spriteRenderer.sprite = brokenBarrier;
            }
            if (durability <= 0)
            {
                GameManager.instance.hero.walkSpeed += 0.3f;
                GameManager.instance.hero.hp += 5f;
                GameManager.instance.hero.OpenAllSkill();
                for (int i = 0; i < GameManager.instance.hero.attackPoint.Length; i++)
                {
                    GameManager.instance.hero.attackPoint[i] += 2;
                }
                buffOra.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }
}

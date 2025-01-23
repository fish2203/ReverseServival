using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_HeroNormalAttack : MonoBehaviour
{
    // 4개의 휘두르는 스프라이트
    public Sprite[] swingAnims;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SwingLeftRight());
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position =  GameManager.instance.heroObject.transform.position;
    }

    IEnumerator SwingLeftRight()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // 좌
        transform.position = GameManager.instance.hero.transform.position + new Vector3(-1.5f, 0f, 0f);
        spriteRenderer.flipX = false;
        for (int i = 0; i < swingAnims.Length; i++)
        {
            spriteRenderer.sprite = swingAnims[i];
            yield return new WaitForSeconds(0.03f);
        }

        // 우
        transform.position = GameManager.instance.hero.transform.position + new Vector3(1.5f, 0f, 0f);
        spriteRenderer.flipX = true;
        for (int i = 0; i < swingAnims.Length; i++)
        {
            spriteRenderer.sprite = swingAnims[i];
            yield return new WaitForSeconds(0.03f);
        }

        Destroy(gameObject);
    }
    // 좌우로 검을 휘두름.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "S_Monster")
        {
            Debug.Log("JJJJJJJJJJJJJJ");
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster == null)
            {
                Destroy(collision.gameObject);
                return;
            }
            J_Hero hero = GameManager.instance.heroObject.gameObject.GetComponent<J_Hero>();

            monster.health -= hero.attackPoint[0];

            if (monster.health <= 0)
            {
                hero.GetExp(monster.xp);
                monster.Death();
            }
        }
    }


}

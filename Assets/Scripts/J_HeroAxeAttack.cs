using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_HeroAxeAttack : MonoBehaviour
{
    J_Hero hero;
    float speed;
    private float angle = 0.0f;
    private float number = 3000.0f;
    // Start is called before the first frame update
    void Start()
    {
        hero = GameManager.instance.heroObject.transform.GetComponent<J_Hero>();
        speed = hero.axeSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * hero.walkSpeed * Time.deltaTime;
        angle += Time.deltaTime * number;
        number -= Random.Range(1, 10);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        var rotain = transform.rotation * (Vector3.up + Vector3.left * 3);
        transform.position += rotain * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "S_Monster")
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster == null)
            {
                Destroy(collision.gameObject);
                return;
            }
            J_Hero hero = GameManager.instance.heroObject.gameObject.GetComponent<J_Hero>();

            monster.health -= hero.attackPoint[1];

            if (monster.health <= 0)
            {
                hero.GetExp(monster.xp);
                monster.Death();
            }
        }
    }
}

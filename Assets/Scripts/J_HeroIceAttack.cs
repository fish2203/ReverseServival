using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_HeroIceAttack : MonoBehaviour
{
    private Camera cam;
    private float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GameManager.instance.mainCamera;
        speed = GameManager.instance.heroObject.transform.GetComponent<J_Hero>().iceShootSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        var rotain = transform.rotation * (Vector3.up + Vector3.left);
        transform.position += rotain * speed * Time.deltaTime;
        // if out of camera, destroy
        if (!IsInCamera())
        {
            Destroy(this.gameObject);
        }
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

            monster.health -= hero.attackPoint[2];

            if (monster.health <= 0)
            {
                hero.GetExp(monster.xp);
                monster.Death();
            }
        }
    }

    private bool IsInCamera()
    {
        Vector3 scr = cam.WorldToViewportPoint(transform.position);
        bool onScreeen = scr.x > 0 && scr.x < 1 && scr.y > 0 && scr.y < 1;
        return onScreeen;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_HeroTriggerCircle : MonoBehaviour
{
    J_Hero hero;
    public List<GameObject> MonsterList = new List<GameObject>();

    GameObject monster = null;

    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        hero = GameManager.instance.heroObject.GetComponent<J_Hero>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position = hero.transform.position;
        if (MonsterList.Count > 0 /*&& hero.state != J_Hero.State.Avoid && hero.state != J_Hero.State.Hit*/)
        {
            float shortest = 999.9f;
            GameObject dangerMonster = null;
            foreach (GameObject monster in MonsterList)
            {
                float length = Vector3.Magnitude(monster.transform.position - hero.transform.position);
                if (shortest > length)
                {
                    shortest = length;
                    dangerMonster = monster;
                }
            }

            if(monster != dangerMonster && timer > 3.0f)
            {
                monster = dangerMonster;
                hero.ChangeToAvoid(dangerMonster);
                timer = 0.0f;
            }
        }
        else if(hero.state == J_Hero.State.Avoid || hero.state == J_Hero.State.Hit)
            hero.ChangeToReturn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "S_Monster")
        {
            MonsterList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "S_Monster")
        {
            MonsterList.Remove(collision.gameObject);

            if(MonsterList.Count > 0)
            {
                hero.ChangeToReturn();
            }
        }
    }
}

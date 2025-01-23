using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_QW : MonoBehaviour
{
    // 이 오브젝트의 Monster
    public Monster monster;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // player와 닿으면 폭발
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("IsDead", true);
            Destroy(gameObject, 0.4f);
        }
    }
}

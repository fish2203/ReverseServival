using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_QW : MonoBehaviour
{
    // �� ������Ʈ�� Monster
    public Monster monster;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // player�� ������ ����
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("IsDead", true);
            Destroy(gameObject, 0.4f);
        }
    }
}

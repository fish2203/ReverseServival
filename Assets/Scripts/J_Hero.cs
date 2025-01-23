using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class J_Hero : MonoBehaviour
{
    public enum State
    {
        Normal, 
        Attack,
        Hit,
        Avoid,
        Return,
        Die
    }

    public GameObject normalAttackPref;
    public GameObject axeAttackPref;
    public GameObject iceAttackPref;

    // stat
    public float hp = 10.0f;
    public int level = 1;
    public float lv_exp = 0.0f;
    public bool hasShield = true;
    public float walkSpeed = 1.0f;
    public int[] attackPoint = new int[3] { 1, 1, 1 };
    public bool[] attackPointFlag = new bool[3] { true, false, false };

    // for test
    public float normalAttackTime = 1.0f;
    public float normalDestroyTime = 0.2f;
    public float axeAttactTime = 3.0f;
    public float axeSpeed = 10.0f;
    public float axeDestroyTime = 2.0f;
    public float iceAttactTime = 5.0f;
    public float iceShootSpeed = 1.0f;
    public float noHitTime = 3.0f;

    // for AI
    private Vector3 originPosition;
    private Vector3 avoidPosition;
    private Vector3 runPosition; 


    // hero state
    public State state = State.Normal;

    private SpriteRenderer renderer;

    // Animation
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("IsPlaying", true);
        originPosition = transform.position;
        StartCoroutine(CoNormalAttack());
    }

    // Update is called once per frame
    void Update()
    {
        // z축 위치 차이로 오류가 나지 않도록 항상 0으로 세팅
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // 용사가 나아가야 할 길을 항상 기억
        if(Vector3.Magnitude(transform.position - originPosition) < 2.0f && state != State.Return)
            originPosition += Vector3.up * walkSpeed * Time.deltaTime;
        else if(state == State.Return) // 피했다가 originPosition으로 귀환중인 경우
            originPosition += Vector3.up * walkSpeed * 0.5f * Time.deltaTime;

        
        if (state == State.Normal) // 일반적인 경우 가야할 길을 감
        {
            transform.position = originPosition;
        }
        else if(state == State.Avoid) // 몬스터를 피할 경우 
        {
            transform.position += avoidPosition;
            
            // 방향에 맞춰 애니메이션 뒤집기
            if (avoidPosition.x > 0)
                renderer.flipX = true;
            else 
                renderer.flipX = false;
        }
        else if(state == State.Return) // 안전상황이 되어 귀환하는 경우
        {
            transform.position += (originPosition - transform.position)/*.normalized*/ * walkSpeed * Time.deltaTime;
            if (Vector3.Magnitude(transform.position - originPosition) < 0.2f)
                state = State.Normal;
        }
        else if(state == State.Hit) // 몬스터에게 맞은 경우
        {
            transform.position += runPosition;

            // 방향에 맞춰 애니메이션 뒤집기
            if (runPosition.x > 0)
                renderer.flipX = true;
            else
                renderer.flipX = false;
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "S_Monster" && state != State.Hit)
        {
            // hit by monster
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster)
            {
                // 효과음 재생
                if (monster.type == Monster.Type.Q) AudioManager.instance.PlaySfx(AudioManager.Sfx.Q_hit);
                else if (monster.type == Monster.Type.W) AudioManager.instance.PlaySfx(AudioManager.Sfx.W_hit);
                else if (monster.type == Monster.Type.E) AudioManager.instance.PlaySfx(AudioManager.Sfx.E_hit);
                else if (monster.type == Monster.Type.QW) AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_hit);
                else if (monster.type == Monster.Type.QE) AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_hit);
                else if (monster.type == Monster.Type.WE) AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_hit);

                hp -= monster.power;
            }
            else // 마녀의 투사체를 맞은 경우
            {
                hp -= collision.gameObject.GetComponent<WitchBullet>().power;
/*                Destroy(collision.gameObject);
*/            }

            if (hp <= 0)
            {
                Death();
                return;
            }

            // change state
            ChangeToHit(collision.gameObject);
            StartCoroutine("CoHitEffect");
        }
    }

    public void Death()
    {
        // 플레이어가 승리 (용사 죽음)
        //Time.timeScale = 0.0f;
        StartCoroutine(GameManager.instance.GameOver(true));
    }

    // if hit, blink effect
    private IEnumerator CoHitEffect()
    {
        int countTime = 0;

        while(countTime < noHitTime * 10)
        {
            if (countTime % 2 == 0)
                renderer.color = new Color32(255, 255, 255, 90);
            else
                renderer.color = new Color32(255, 255, 255, 180);

            transform.position += runPosition;

            yield return new WaitForSeconds(0.1f);

            countTime++;
        }

        state = State.Avoid;
        renderer.color = new Color32(255, 255, 255, 255);

        yield return null;
    }

    private IEnumerator CoNormalAttack()
    {
        while (true)
        {
            Debug.Log("NormalAttack");
            GameObject normalAttack = Instantiate(normalAttackPref);
            normalAttack.transform.position = transform.position;
            normalAttack.SetActive(true);
            //Destroy(normalAttack, 0.5f);

            yield return new WaitForSeconds(normalAttackTime);
        }
    }

    private IEnumerator CoAxeAttack()
    {
        while (true)
        {
            Debug.Log("AxeAttack");
            GameObject axeAttack = Instantiate(axeAttackPref);
            axeAttack.transform.position = transform.position;
            axeAttack.SetActive(true);

            Destroy(axeAttack, axeDestroyTime);
            yield return new WaitForSeconds(axeAttactTime);
        }
    }

    private IEnumerator CoIceAttack()
    {
        while (true)
        {
            Debug.Log("IceAttack");

            for(int i = 0; i < 6; i++)
            {
                GameObject iceAttack = Instantiate(iceAttackPref);
                iceAttack.transform.position = transform.position;
                iceAttack.transform.rotation = Quaternion.Euler(0, 0, (60.0f * i)-45.0f);
                iceAttack.SetActive(true);
            }

            yield return new WaitForSeconds(iceAttactTime);
        }
    }

    public void GetExp(float exp)
    {
        lv_exp += exp;

        if(level * 10 <= lv_exp)
        {
            lv_exp -= level * 10;
            level++;
            LevelUp();
        }
    }

     void LevelUp()
    {
        // Choose Random Skill Index
        var randomSkillNumber = new List<int>() { 0, 1, 2 };
        
        // 기본 스킬이 최대치인 경우 선택 불가
        if (attackPoint[0] == 5)
            randomSkillNumber.Remove(0);

        // 다른 스킬이 최대치인 경우 선택 불가
        for (int i = 1; i < 3; i++)
        {
            if (attackPoint[i] == 6)
                randomSkillNumber.Remove(i);
        }

        // 레벨을 올릴 수 있는 스킬이 있는 경우 스킬 강화
        if (randomSkillNumber.Count > 0)
        {
            int randomNumber = randomSkillNumber.OrderBy(_ => Guid.NewGuid()).First();
            ++attackPoint[randomNumber];

            // 재생되지 않은 기술인 경우 재생 시작
            if(attackPointFlag[randomNumber] == false)
            {
                attackPointFlag[randomNumber] = true;

                switch (randomNumber)
                {
                    case 1:
                        StartCoroutine(CoAxeAttack());
                        break;
                    case 2:
                        StartCoroutine(CoIceAttack());
                        break;
                }
            }
        }
    }

    // 방어막이 깨질 경우 모든 스킬 오픈
    public void OpenAllSkill()
    {
        hasShield = false;
        for(int i = 0; i < 3; i++)
        {
            if (attackPointFlag[i]) continue;

            attackPointFlag[i] = true;

            switch (i)
            {
                case 1:
                    StartCoroutine(CoAxeAttack());
                    break;
                case 2:
                    StartCoroutine(CoIceAttack());
                    break;
            }
        }
    }

    public void ChangeToAvoid(GameObject monster)
    {
        // 몬스터가 향하는 방향에서 90도 꺾음
        Vector3 avoidDir = Quaternion.Euler(new Vector3(90, 90, 90)) * (monster.transform.position - transform.position);
        //var dir = new Vector3(avoidDir.z, avoidDir.y, avoidDir.x);
        avoidPosition = avoidDir/*.normalized */* walkSpeed * Time.deltaTime;
        state = State.Avoid;
        animator.SetBool("IsBlocked", true);
    }

    public void ChangeToReturn()
    {
        state = State.Return;
        animator.SetBool("IsBlocked", false);
    }

    public void ChangeToHit(GameObject monster)
    {
        state = State.Hit;
        animator.SetBool("IsBlocked", true);

        // 몬스터가 향하는 방향에서 90도 꺾음
        Vector3 runDir = Quaternion.Euler(new Vector3(90, 90, 90)) * (monster.transform.position - transform.position);
        //var dir = new Vector3(runDir.z, runDir.y, runDir.x);
        runPosition = runDir.normalized * walkSpeed * 3 * Time.deltaTime;
    }
}

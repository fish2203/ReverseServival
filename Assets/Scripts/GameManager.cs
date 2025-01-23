using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;




    public Image whitete;
    public Image princesessessee;


    public int qDeathrattle;
    public int wDeathrattle;
    public int eDeathrattle;

    public int qCoin; // q몬스터를 소환하기 위한 코인
    public int wCoin; // w몬스터를 소환하기 위한 코인
    public int eCoin; // e몬스터를 소환하기 위한 코인

    public float rubyDropPer; // 루비드랍확률, 0 ~ 100



    // 프리팹 (인스펙터에서 할당)
    public GameObject monsterQpref;
    public GameObject monsterWpref;
    public GameObject monsterEpref;
    public GameObject monsterQWpref;
    public GameObject monsterQEpref;
    public GameObject monsterWEpref;
    public GameObject monsterQWEpref;

    public GameObject rubyPref;



    public GameObject heroObject; // 영웅
    public J_Hero hero;

    public Image DefeatImage;
    public Image WinImage;

    readonly float startY = 0f; // 게임 시작 시 y좌표
    readonly float endY = 30f; // 게임이 종료되는(공주가 있는) y좌표

    public Camera mainCamera; // 메인 카메라

    public Text haveRubyText;
    public Text qCoinText;
    public Text wCoinText;
    public Text eCoinText;
    public RectTransform progressBarTransform;
    public Image[] princesses;
    public GameObject background;

    private Monster.Type activeMonster; // 활성화된 몬스터 (클릭 시 해당 몬스터 소환)
    public MonsterIcon[] monsterIcons; // 총 7개, Q/W/E/QW/QE/WE/QWE 순서

    public Image luciferImage;

    public Text[] needRuby; // QW, QE, WE 3가지의 요구 루비

    public GameObject hpBarMask; // 플레이어 체력바 마스크

    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public Texture2D errorCursor;



    
    [HideInInspector] public int haveRuby; // 보유중인 루비
    [HideInInspector] public float progress; // 게임 진행도 (0 ~ 1)
    private bool is75 = true;

    [HideInInspector] public bool isRightBuff; // 우클릭 시 적용되는 버프 여부



    private List<Monster> monsterPool = new List<Monster>();
    public Monster GetMonster(Monster.Type monsterType)
    {
        // 풀에 몬스터가 없다면 먼저 인스턴스화
/*        if (!monsterPool.Any(m => m.type == monsterType))
        {*/
            GameObject newMonsterObj = null;
            switch (monsterType)
            {
                case Monster.Type.Q:
                    newMonsterObj = Instantiate(monsterQpref, transform);
                    break;

                case Monster.Type.W:
                    newMonsterObj = Instantiate(monsterWpref, transform);
                    break;

                case Monster.Type.E:
                    newMonsterObj = Instantiate(monsterEpref, transform);
                    break;

                case Monster.Type.QW:
                    newMonsterObj = Instantiate(monsterQWpref, transform);
                    break;

                case Monster.Type.QE:
                    newMonsterObj = Instantiate(monsterQEpref, transform);
                    break;

                case Monster.Type.WE:
                    newMonsterObj = Instantiate(monsterWEpref, transform);
                    break;

                case Monster.Type.QWE:
                    newMonsterObj = Instantiate(monsterQWEpref, transform);
                    break;

                default:
                    Debug.Log("엄");
                    break;
            }
        /*
                    Monster newMonster = newMonsterObj.GetComponent<Monster>();
                    monsterPool.Add(newMonster);
                }*/
        /*
                // 리스트의 첫번째 몬스터를 삭제 후 반환
                Monster monster = monsterPool.First(m => m.type == monsterType);
                monsterPool.Remove(monster);*/
        /*monster.gameObject.SetActive(true);
        return monster;*/

        newMonsterObj.gameObject.SetActive(true);
        return newMonsterObj.gameObject.transform.GetComponent<Monster>();
    }
    public void ReturnMonster(Monster monster)
    {
        // 몬스터가 리스트에 없는지 확인 후 추가
        if (!monsterPool.Contains(monster))
        {
            monsterPool.Add(monster);

            monster.gameObject.SetActive(false);
        }
    }



    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        activeMonster = Monster.Type.Q;

        // 초기 재화 지정
        haveRuby = 0;

        StartCoroutine(Princess());
    }
    private void Update()
    {
        

        // 마우스 커서 텍스쳐
        if (showError == null)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.SetCursor(clickCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);
            }
        }

        // 마우스 클릭 시, 현재 활성화된 몬스터 소환
        if (Input.GetMouseButtonDown(0))
        {
            if (activeMonster == Monster.Type.QWE)
            {
                SummonMonster(activeMonster);
            }
            else
            {
                ContinuousSummon();
            }
        }

        // 보유중인 루비, 코인 화면에 표시
        haveRubyText.text = haveRuby.ToString();
        qCoinText.text = qCoin.ToString();
        wCoinText.text = wCoin.ToString();
        eCoinText.text = eCoin.ToString();
        needRuby[0].color = (haveRuby >= 1)
            ? Color.white
            : Color.red;
        needRuby[1].color = (haveRuby >= 3)
            ? Color.white
            : Color.red;
        needRuby[2].color = (haveRuby >= 5)
            ? Color.white
            : Color.red;

        // 진행도 계산
        float progress = (heroObject.transform.position.y - startY) / (endY - startY);
        // 진행도 바
        progressBarTransform.sizeDelta = new Vector2(1770.205f * progress, 70f);
        
        // 진행도가 처음으로 75%가 넘어갔다면, 마왕 몬스터아이콘 표시
        if (progress > 0.75f)
        {
            luciferImage.color = Color.white;          
        }
        // 진행도가 100%에 도달했다면, 패배 (용사 승리)
        if (progress >= 1.0f)
        {
            StartCoroutine(GameOver(false));
        }

        // 플레이어 체력바
        float hpRatio = hero.hp / 30f; // 이후 플레이어 체력과 연동해야 함
        hpBarMask.transform.localScale = new Vector3(0.711f * hpRatio, 0.16f, 1f);
        hpBarMask.transform.localPosition = new Vector3(-0.3555f * (1f - hpRatio), 0f, 0f);

        // 활성화 몬스터 지정
        // 한 키를 눌렀을 때, 기준시간 내 다른 키를 누르면 QW/QE/WE 활성화
        // 그렇지 않다면 Q/W/E 활성화
        if (Input.GetKeyDown(KeyCode.Q))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.QW
                : Monster.Type.Q;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.QE
                : Monster.Type.W;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.WE
                : Monster.Type.E;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (progress > 0.75f)
            {
                activeMonster = Monster.Type.QWE;
            }
        }
        foreach (MonsterIcon icon in monsterIcons)
        {
            // 활성화된 아이콘만 흰색으로 강조
            icon.SetSprite(icon.myMonsterType == activeMonster);
        }
    }


    public IEnumerator GameOver(bool isWin)
    {
        if (isWin)
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            whitete.gameObject.SetActive(true);
            float timer = 0f;
            float duration = 1f;
            while (timer < duration)
            {
                whitete.color = new Color(1f, 1f, 1f, timer / duration);

                timer += Time.deltaTime;
                yield return null;
            }

            princesessessee.gameObject.SetActive(true);
            float timerr = 0f;
            float durationr = 1f;
            while (timerr < durationr)
            {
                princesessessee.color = new Color(1f, 1f, 1f, timerr / durationr);

                timerr += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
            while (timerr > 0f)
            {
                princesessessee.color = new Color(1f, 1f, 1f, timerr / durationr);

                timerr -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            SceneManager.LoadScene("Defeat");
        }
    }

    Coroutine continuousSummon;
    private void ContinuousSummon()
    {
        if (continuousSummon != null)
        {
            StopCoroutine(continuousSummon);
        }

        StartCoroutine(_ContinuousSummon());
    }
    IEnumerator _ContinuousSummon()
    {
        float firstDelay = 0.0f; // 처음 기다릴 시간
        float interval = 0.1f; // 소환 시간간격

        yield return new WaitForSeconds(firstDelay);

        float timer = interval;
        while (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                SummonMonster(activeMonster);
                timer -= interval;
            }

            yield return null;
        }

        
    }
    private void SummonMonster(Monster.Type type)
    {
        // 용사와 가까이 소환 시도 시 경고
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        if (Vector2.Distance(mousePos, heroObject.transform.position) < 2.2f) // <<< 이 값을 바꿔 거리 조절
        {
            return;
        }

        // 비용 지불, 비용을 지불할 수 없다면 소환 취소 및 에러경고
        bool charged = false;
        if (type == Monster.Type.QWE)
        {
            if (is75 && haveRuby >= 50)
            {
                is75 = false;
                haveRuby -= 50;
                charged = true;
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.Q)
        {
            if (qCoin > 0)
            {
                qCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Q_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.W)
        {
            if (wCoin > 0)
            {
                wCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.W_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.E)
        {
            if (eCoin > 0)
            {
                eCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.E_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.QW)
        {
            if (haveRuby >= 1)
            {
                haveRuby -= 1;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.QE)
        {
            if (haveRuby >= 3)
            {
                haveRuby -= 3;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.WE)
        {
            if (haveRuby >= 5)
            {
                haveRuby -= 5;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_spawn);
            }
            else
            {
                ShowError();
            }
        }

        if (charged)
        {
            // 소환
            Monster summoningMonster = GetMonster(type);
            summoningMonster.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }       
    }



    Coroutine showError;
    void ShowError()
    {
        Debug.Log("errrroorrror");

        if (showError != null)
        {
            StopCoroutine(showError);
        }

        showError = StartCoroutine(_ShowError());
    }
    IEnumerator _ShowError()
    {
        // 마우스 커서 변경
        Cursor.SetCursor(errorCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);

        // 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.NoMoney);

        // 화면 떨림
        yield return new WaitForSeconds(0.5f);

        showError = null;
    }

    IEnumerator Princess()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            princesses[0].gameObject.SetActive(true);
            princesses[1].gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            princesses[0].gameObject.SetActive(false);
            princesses[1].gameObject.SetActive(true);
        }
    }


    public IEnumerator BuffMonsters()
    {
        isRightBuff = true;

        int duration = 5; // 지속시간(초)

        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForSeconds(1f);

            // 모든 몬스터 체력 회복
            List<Monster> monsterList = new List<Monster>();
            foreach (Transform child in transform)
            {
                // 자식 오브젝트에서 Monster 컴포넌트가 있는지 확인
                Monster monsterScript = child.GetComponent<Monster>();
                if (monsterScript != null)
                {
                    // Monster 스크립트를 가진 오브젝트라면 리스트에 추가
                    monsterList.Add(monsterScript);
                }
            }

            foreach (Monster monster in monsterList)
            {
                monster.health += 1;
            }
        }

        isRightBuff = false;
    }

    /*
    // q,w,e를 누르고 기준시간 내에 다른 키를 누르면 QW/QE/WE 활성화
    // 두 키를 누른 상태에서 나머지 한 키를 누르면 QWE 활성화
    // 그렇지 않고 기준시간이 지나가면 Q/W/E 활성화
    private Coroutine inputWait;
    private void InputWait(bool isQ, bool isW, bool isE)
    {
        if (inputWait != null)
        {
            StopCoroutine(inputWait);
        }

        inputWait = StartCoroutine(_InputWait(isQ, isW, isE));
    }
    IEnumerator _InputWait(bool isQ, bool isW, bool isE)
    {
        float duration = 0.005f; // 확인할 기준시간
        float timer = 0f;

        while (timer < duration)
        {
            // 새로운 키가 눌렸는지 확인
            if (Input.GetKey(KeyCode.Q) && !isQ)
            {
                InputWait(true, isW, isE);
            }
            if (Input.GetKey(KeyCode.W) && !isW)
            {
                InputWait(isQ, true, isE);
            }
            if (Input.GetKey(KeyCode.E) && !isE)
            {
                InputWait(isQ, isW, true);
            }

            timer += Time.deltaTime; // 경과 시간 증가
            yield return null;       // 다음 프레임까지 대기
        }

        // 기준시간 동안 새 키가 눌리지 않았으므로, 활성화키 변경
        // 무지성 확인
        if (isQ && isW && isE)
        {
            // 75 이전에는 활성화 불가능
            if (is75) activeMonster = Monster.Type.QWE;
        }
        else if (isQ && isW)
        {
            activeMonster = Monster.Type.QW;
        }
        else if (isQ && isE)
        {
            activeMonster = Monster.Type.QE;
        }
        else if (isW && isE)
        {
            activeMonster = Monster.Type.WE;
        }
        else if (isQ)
        {
            activeMonster = Monster.Type.Q;
        }
        else if (isW)
        {
            activeMonster = Monster.Type.W;
        }
        else if (isE)
        {
            activeMonster = Monster.Type.E;
        }
        Debug.Log("now active : " + activeMonster);
        // 몬스터아이콘 시각적 변경
        foreach (MonsterIcon icon in monsterIcons)
        {
            // 활성화된 아이콘만 흰색으로 강조
            icon.SetSprite(icon.myMonsterType == activeMonster);
        }
    }
    */
}

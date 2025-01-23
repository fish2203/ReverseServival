using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiinn : MonoBehaviour
{
    public SpriteRenderer gameClearMessage;
    public SpriteRenderer text;
    public SpriteRenderer clickToRestart;

    void Start()
    {
        StartCoroutine(MessageUp());
    }

    IEnumerator MessageUp()
    {
        
        float timer = 0f;
        float duration = 1.0f;
        gameClearMessage.transform.position = new Vector3(-0.3f, 0f, 0f);
        yield return new WaitForSeconds(1.0f);
        while (timer < duration)
        {
            gameClearMessage.transform.position += new Vector3(0f, 3f * Time.deltaTime, 0f);
            //gameClearMessage.color = new Color(1f, 1f, 1f, timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        text.transform.position = Vector3.zero;

        yield return new WaitForSeconds(2.0f);

        clickToRestart.transform.position = new Vector3(0f, -3f, 0f);
    }
}

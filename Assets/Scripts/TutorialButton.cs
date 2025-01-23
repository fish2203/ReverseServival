using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    public Image descriptionImage;

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            {
                GetComponent<Button>().interactable = true;
                descriptionImage.gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void ShowDescription()
    {
        GetComponent<Button>().interactable = false;
        descriptionImage.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}

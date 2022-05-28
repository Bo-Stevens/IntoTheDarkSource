using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class EndGame : MonoBehaviour
{

    public Image endGameScreen;
    public AudioClip EthanSucks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine("EndTheGame");
        }
    }

    IEnumerator EndTheGame()
    {
        endGameScreen.gameObject.SetActive(true);
        DOTween.To(() => endGameScreen.color.a,
            x => endGameScreen.color = new Color(endGameScreen.color.r, endGameScreen.color.g, endGameScreen.color.b, x),
            1f, 1.5f);

        yield return new WaitForSeconds(5f);

        GetComponent<AudioSource>().PlayOneShot(EthanSucks);

        yield return new WaitForSeconds(6f);

        Application.Quit();
    }
}

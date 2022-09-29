using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SpaceCabron.UI
{
    public class FadeInFadeOutLoadScene : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run() 
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            yield return FadeIn(text);
            yield return new WaitForSeconds(3f);
            yield return FadeOut(text);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Menu");
        }


        IEnumerator FadeIn(TextMeshProUGUI text)
        {
            yield return TweenAlphaTo(text, 1f);
        }

        IEnumerator FadeOut(TextMeshProUGUI text)
        {
            yield return TweenAlphaTo(text, 0f);
        }

        IEnumerator TweenAlphaTo(TextMeshProUGUI text, float alpha)
        {
            while (text.color.a != alpha)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.MoveTowards(text.color.a, alpha, Time.deltaTime));
                yield return null;
            }
        }

    }
}
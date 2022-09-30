using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCabron.UI {
    public class UIFadeIn : MonoBehaviour
    {
        [SerializeField] Image image;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(FadeInImage());
        }

        IEnumerator FadeInImage()
        {
            while (true) {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.MoveTowards(image.color.a, 1f, Time.deltaTime));
                yield return null;
            }
        }

    }
}
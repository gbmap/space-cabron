using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.UI
{
    public class UISelectOnEnable : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Selectable selectable;
        void OnEnable()
        {
            StartCoroutine(Select());
        }

        IEnumerator Select()
        {
            yield return new WaitForFixedUpdate();
            if (selectable)
                selectable.Select();
        }
    }
}
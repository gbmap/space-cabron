using UnityEngine;
using UnityEngine.Events;

public class Latch : MonoBehaviour
{
    [SerializeField] int count;
    [SerializeField] bool resetOnZero = false;
    public UnityEvent OnLatch;

    private int initialValue;

    void Awake()
    {
        initialValue = count;
    }

    public void Decrease()
    {
        count--;
        if (count == 0)
        {
            OnLatch?.Invoke();

            if (resetOnZero)
                count = initialValue;
            else
                Destroy(this);
        }
    }
}

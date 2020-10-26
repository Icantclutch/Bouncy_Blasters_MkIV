using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{

    public float TimeToDestroy = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
       Destroy(gameObject, TimeToDestroy);
    }
}

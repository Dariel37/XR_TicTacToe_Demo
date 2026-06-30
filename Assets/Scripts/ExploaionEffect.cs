using UnityEngine;

public class ExploaionEffect : MonoBehaviour
{
    // How fast the explosion grows.
    public float growSpeed = 8f;
    // How long the explosion stays before disappearing.
    public float lifeTime = 0.4f;

    void Start()
    {
        // Automatically destroy explosion after lifeTime seconds.
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Increase scale every frame to make explosion expand.
        transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
    }
}

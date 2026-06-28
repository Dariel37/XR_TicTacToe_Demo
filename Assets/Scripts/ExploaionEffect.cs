using UnityEngine;

public class ExploaionEffect : MonoBehaviour
{
    public float growSpeed = 8f;
    public float lifeTime = 0.4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
    }
}

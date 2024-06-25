using Unity.Entities;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = em.CreateEntity();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
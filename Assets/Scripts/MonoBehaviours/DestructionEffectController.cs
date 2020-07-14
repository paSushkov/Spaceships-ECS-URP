using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionEffectController : MonoBehaviour
{
    [SerializeField]
    private float DestructionTime = 3f;

    private Vector3 newPosition;

    void Start()
    {
        StartCoroutine(selfDestruction(DestructionTime));
    }

    private void Update()
    {
        newPosition = transform.position;
        newPosition.z -= GameManager.instance.CurrentEnviromentSpeed*Time.deltaTime;
        transform.position = newPosition;
    }
    private IEnumerator selfDestruction(float SelfDesrouAfter)
    {
        yield return new WaitForSecondsRealtime(SelfDesrouAfter);
        Destroy(this.gameObject);
    }

}

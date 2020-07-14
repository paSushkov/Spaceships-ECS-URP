using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class msgParentToDrawGizmos : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        this.transform.parent.SendMessage("OnDrawGizmosSelected");
    }
}

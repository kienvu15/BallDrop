using System.Collections;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [SerializeField]
    private StackPartController[] stackPartControllers = null;

    public void ShatterAllPart()
    {
        if(transform.parent != null)
        {
            transform.parent = null;
            FindFirstObjectByType<Ball>().IncreaseBrokenStacks();
        }

        foreach(StackPartController o in stackPartControllers)
        {
            o.Shatter();
        }
        StartCoroutine(RemoveAllParts());
    }

    IEnumerator RemoveAllParts()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}

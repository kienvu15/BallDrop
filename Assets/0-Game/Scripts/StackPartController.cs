using UnityEngine;

public class StackPartController : MonoBehaviour
{

    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private StackController stackController;
    private Collider collider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        stackController = transform.parent.GetComponent<StackController>();
    }

    public void Shatter()
    {
        rb.isKinematic = false;
        collider.enabled = false;

        Vector3 forcePoint = transform.parent.position;
        float paretXpos = transform.parent.position.x;
        float xPos = meshRenderer.bounds.center.x;

        Vector3 subBir = (paretXpos - xPos < 0) ? Vector3.right : Vector3.left;
        Vector3 dir = (Vector3.up * 1.5f + subBir).normalized;

        float force = Random.Range(20, 35);
        float torque = Random.Range(110, 180);

        rb.AddForceAtPosition(dir * force, forcePoint, ForceMode.Impulse);
        rb.AddTorque(Vector3.left * torque);
        rb.linearVelocity = Vector3.down;
    }

    public void RemoveAllChilds()
    {
        for(int i = 0; i<transform.childCount; i++)
        {
            transform.GetChild(i).SetParent(null);
            i--;
        }
    }
}

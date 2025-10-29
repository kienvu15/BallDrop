using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private float currentTime;
    private bool smash, invicible;

    private int currentBrokenStacks, totalStacks;

    public GameObject invicibleObj;
    public Image invicibleFill;
    public GameObject fireEffect, winEffect, splashEffect;

    public enum BallState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public BallState ballState = BallState.Prepare;

    public AudioClip bounceOffClip, deadClip, winClip, destroyClip, iDestroyClip;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentBrokenStacks = 0;
    }

    private void Start()
    {
        totalStacks = FindObjectsByType<StackController>(FindObjectsSortMode.None).Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(ballState == BallState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                smash = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                smash = false;
            }
            if (invicible)
            {
                currentTime -= Time.deltaTime * .35f;
                if (!fireEffect.activeInHierarchy)
                {
                    fireEffect.SetActive(true);
                }
            }
            else
            {
                if (fireEffect.activeInHierarchy)
                {
                    fireEffect.SetActive(false);
                }

                if (smash)
                {
                    currentTime += Time.deltaTime * .8f;
                }
                else
                {
                    currentTime -= Time.deltaTime * .5f;
                }
            }

            if(currentTime >= 0.3f || invicibleFill.color == Color.red)
            {
                invicibleObj.SetActive(true);
            }
            else
            {
                invicibleObj.SetActive(false);
            }

            if (currentTime >= 1)
            {
                currentTime = 1;
                invicible = true;
                invicibleFill.color = Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invicible = false;
                invicibleFill.color = Color.white;
            }
            if (invicibleObj.activeInHierarchy)
            {
                invicibleFill.fillAmount = currentTime / 1;
            }
        }

        

        if(ballState == BallState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindFirstObjectByType<LevelSpawner>().NextLevel();
            }
        }
    }

    void FixedUpdate()
    {
        if (ballState == BallState.Playing)
        {
            if (Input.GetMouseButton(0))
            {
                smash = true;
                rb.linearVelocity = new Vector3(0, -100 * Time.deltaTime * 7, 0);
            }
        }

        if(rb.linearVelocity.y > 5)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5, rb.linearVelocity.z);
        }
    }

    public void IncreaseBrokenStacks()
    {
        currentBrokenStacks++;
        if (!invicible)
        {
            ScoreManager.instance.AddScore(1);
            SoundManager.instance.PlaySound(destroyClip, 0.5f);

        }
        else
        {
            ScoreManager.instance.AddScore(2);
            SoundManager.instance.PlaySound(iDestroyClip, 0.5f);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!smash)
        {
            rb.linearVelocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
            if(collision.gameObject.tag != "Finish")
            {
                GameObject splash = Instantiate(splashEffect);
                splash.transform.SetParent(collision.transform);
                splash.transform.localEulerAngles = new Vector3(90, Random.Range(0, 359), 0);
                float randomScale = Random.Range(0.18f, 0.3f);
                splash.transform.localScale = new Vector3(randomScale, randomScale, 1);
                splash.transform.position = new Vector3(transform.position.x, transform.position.y - 0.22f, transform.position.z);
                splash.GetComponent<SpriteRenderer>().color = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            }
            SoundManager.instance.PlaySound(bounceOffClip, 0.5f);
        }
        else
        {
            if (invicible)
            {
                if(collision.gameObject.tag == "enemy" || collision.gameObject.tag == "plane")
                {
                    collision.transform.parent.GetComponent<StackController>().ShatterAllPart();
                }
            }
            else
            {
                if(collision.gameObject.tag == "enemy")
                {
                    collision.transform.parent.GetComponent<StackController>().ShatterAllPart();
                }
                if (collision.gameObject.tag == "plane")
                {
                    rb.isKinematic = true;
                    transform.GetChild(0).gameObject.SetActive(false);
                    ballState = BallState.Died;
                    SoundManager.instance.PlaySound(deadClip, 0.5f);

                }
            }
        }

        FindFirstObjectByType<GameUI>().LevelSliderFill(currentBrokenStacks / (float)totalStacks);

        if(collision.gameObject.tag == "Finish" && ballState == BallState.Playing)
        {
            ballState = BallState.Finish;
            SoundManager.instance.PlaySound(winClip, 0.7f);
            GameObject win = Instantiate(winEffect);
            win.transform.SetParent(Camera.main.transform);
            win.transform.localPosition = Vector3.up * 1.5f;
            win.transform.eulerAngles = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(!smash || collision.gameObject.tag == "Finish")
        {
            rb.linearVelocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
    }
}

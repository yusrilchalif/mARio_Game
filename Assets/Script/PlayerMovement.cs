using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vuforia;

public class PlayerMovement : MonoBehaviour, ITrackableEventHandler
{
    AudioSource[] sources;
    AudioSource backgroundMusic;
    AudioSource jumpSound;
    AudioSource MushroomSound;
    AudioSource LostLifeSound;
    public AudioClip backgroundClip;
    public AudioClip jumpClip;
    public AudioClip MushroomClip;
    public AudioClip LostLifeClip;
    public float turnSmoothing = 15f;   
    public float speedDampTime = 0.1f;  

    public int score = 0;
    public Text scoreText;

    public float movementSpeed = 5.0f;
    public float jumpSpeed = 100.0f;
    float distToGround;
    public Animation jump;
    public Animation run;
    public Animation idle;
    public Collider ground;

    Vector3 forward;
    float curSpeed;
    float speed = 3.0F;
    public GameObject imageTarget;
    private bool targetFound = false;
    private TrackableBehaviour mTrackableBehaviour;
    void Start()
    {
        mTrackableBehaviour = imageTarget.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        distToGround = ground.bounds.extents.y;

        sources = GetComponents<AudioSource>();
        backgroundMusic = sources[0];
        jumpSound = sources[1];
        MushroomSound = sources[2];
        LostLifeSound = sources[3];
        jump = GetComponent<Animation>();
        run = GetComponent<Animation>();
        idle = GetComponent<Animation>();
    }


    void FixedUpdate()
    {
        if (targetFound)
        {
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.clip = backgroundClip;
                backgroundMusic.Play();
            }
            CharacterController controller = GetComponent<CharacterController>();
            // Cache the inputs.
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            scoreText.text = score + "";



            if ((x != 0f || z != 0f))
            { // && !Input.GetButton ("Jump") 
                Rotating(x, z);
                Moving(x, z);
            }
            else if (Input.GetButton("Jump") != true && !idle.IsPlaying("idle") && (x == 0f || z == 0f) && IsGrounded() == true)
                idle.Play("idle");

            if (Input.GetButton("Jump") && IsGrounded() == true)
            {
                jumpSound.clip = jumpClip;
                jumpSound.Play();
                if (!jump.IsPlaying("jump"))
                {
                    jump["jump"].speed = 1.0f;
                    jump.Play("jump");
                }
                GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }
        }
        if (!targetFound)
        {
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            if (backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }
        }
    }


    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical); 
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up); 
        Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
        GetComponent<Rigidbody>().MoveRotation(newRotation); 
    }

    void Moving(float horizontal, float vertical)
    {
        /*if (horizontal < 0)
        {
            Debug.Log("Go Left");
            transform.Translate(0, 0, -horizontal * Time.fixedDeltaTime * speed);
        }
        if (horizontal > 0)
        {
            Debug.Log("Go Right");
            transform.Translate(0, 0, horizontal * Time.fixedDeltaTime * speed);
        }*/

        if (vertical < 1)
        {
            Debug.Log("Go Forward");
            transform.Translate(0, 0, -vertical * Time.fixedDeltaTime * speed);
        }

        if (vertical > 1)
        {
            Debug.Log("Go Back");
            transform.Translate(0, 0, vertical * Time.fixedDeltaTime * speed);
        }


        if (!run.IsPlaying("run") && !jump.IsPlaying("jump"))
            run.Play("run");
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void OnCollisionEnter(Collision info)
    {
        Debug.Log(info.gameObject.name);
        if (info.gameObject.name == "enemy" && !LostLifeSound.isPlaying)
        {
            LostLifeSound.clip = LostLifeClip;
            LostLifeSound.Play();
        }
        if (info.gameObject.name == "mushroom" && !MushroomSound.isPlaying)
        {
            MushroomSound.clip = MushroomClip;
            MushroomSound.Play();
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            //when target is found
            targetFound = true;
        }
        else
        {
            //when target is lost
            targetFound = false;
        }
    }

}
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Config params
    [SerializeField] Paddle paddle1;
    [SerializeField] float xPush = 2f;
    [SerializeField] float yPush = 15f;
    [SerializeField] AudioClip[] ballSounds;
    [SerializeField] float randomFactor = 0.1f;
    [SerializeField] float fixedSpeed = 15f; // Fixed speed for consistent movement

    // State
    Vector2 paddleToBallVector;
    bool hasStarted = false;

    // Cached component references
    AudioSource myAudioSource;
    Rigidbody2D myRigidBody2D;

    // Initialization
    void Start()
    {
        paddleToBallVector = transform.position - paddle1.transform.position;
        myAudioSource = GetComponent<AudioSource>();
        myRigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            LockBallToPaddle();
            LaunchOnMouseClick();
        }
    }

    private void LaunchOnMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hasStarted = true;
            myRigidBody2D.velocity = new Vector2(xPush, yPush);
        }
    }

    private void LockBallToPaddle()
    {
        Vector2 paddlePos = new Vector2(paddle1.transform.position.x, paddle1.transform.position.y);
        transform.position = paddlePos + paddleToBallVector;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only make adjustments if the game has started
        if (hasStarted)
        {
            // Play a random sound from the ballSounds array
            AudioClip clip = ballSounds[UnityEngine.Random.Range(0, ballSounds.Length)];
            myAudioSource.PlayOneShot(clip);

            // Slight tweak for natural feel, avoiding major direction change
            Vector2 velocityTweak = new Vector2(
                Random.Range(-randomFactor, randomFactor),
                Random.Range(-randomFactor, randomFactor)
            );

            // Calculate new velocity while maintaining a straight trajectory
            Vector2 currentVelocity = myRigidBody2D.velocity.normalized * fixedSpeed;
            Vector2 newVelocity = new Vector2(
                currentVelocity.x + velocityTweak.x,
                Mathf.Sign(currentVelocity.y) * Mathf.Max(Mathf.Abs(currentVelocity.y + velocityTweak.y), fixedSpeed * 0.5f)  // Ensure minimum vertical movement
            );

            // Apply the adjusted and normalized velocity
            myRigidBody2D.velocity = newVelocity.normalized * fixedSpeed;
        }
    }
}

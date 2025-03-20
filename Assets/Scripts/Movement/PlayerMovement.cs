using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float Drag = 0.85f;
    public int MaxCollisionIterations = 5;
    public float GravityModifier = 15f;

    private Vector3 velocity;
    private Collider playerCollider;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        bool grounded = IsGrounded();

        if (grounded && Input.GetKey(KeyCode.Space))
            velocity.y = JumpStrength;
        if (!grounded)
            velocity.y -= GravityModifier * Time.deltaTime;
        if (Speed > 0)
        {
            GetInputs();
        }

        Vector3 newPos = GetNewPosition();
        transform.position = newPos;

        // Drag nur auf horizontale Bewegung anwenden
        velocity = new Vector3(velocity.x * Drag, velocity.y, velocity.z * Drag);
    }

    void GetInputs()
    {
        Vector3 inputDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) inputDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) inputDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) inputDirection += transform.right;

        Vector3 horizontalInput = new Vector3(inputDirection.x, 0f, inputDirection.z);
        if (horizontalInput != Vector3.zero)
            velocity += horizontalInput.normalized;
    }

    Vector3 GetNewPosition()
    {
        Vector3 currentPos = transform.position;
        Vector3 intendedPos = currentPos + new Vector3(velocity.x, 0f, velocity.z) * Speed * Time.deltaTime;
        intendedPos.y += velocity.y * Time.deltaTime; // Vertikale Bewegung separat behandeln

        Vector3 halfExtents = playerCollider.bounds.extents;
        int iteration = 0;
        bool collisionDetected;
        do
        {
            collisionDetected = false;
            Collider[] hits = Physics.OverlapBox(intendedPos, halfExtents);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;
                if (Physics.ComputePenetration(playerCollider, intendedPos, transform.rotation,
                    hit, hit.transform.position, hit.transform.rotation,
                    out Vector3 collisionNormal, out float penetrationDistance))
                {
                    intendedPos += collisionNormal * penetrationDistance;
                    velocity = Vector3.ProjectOnPlane(velocity, collisionNormal);
                    collisionDetected = true;
                }
            }
            iteration++;
        } while (collisionDetected && iteration < MaxCollisionIterations);
        return intendedPos;
    }

    bool IsGrounded()
    {
        float radius = playerCollider.bounds.extents.x; // Radius für den Check
        float halfHeight = playerCollider.bounds.extents.y + -1f; // Halbe Höhe des Spielers
        float groundCheckDistance = 0.5f; // Kleine Distanz für den Check

        Vector3 startPosition = transform.position - Vector3.up * halfHeight; // Startpunkt an den Füßen

        if (Physics.SphereCast(startPosition, radius, Vector3.down, out RaycastHit hit, groundCheckDistance))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < 45f)
                return true;
        }
        return false;
    }
}

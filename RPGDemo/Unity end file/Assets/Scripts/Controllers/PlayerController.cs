using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;

/* Controls the player. Here we choose our "focus" and where to move. */

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	public Interactable focus;	// Our current focus: Item, Enemy etc.
    public Rigidbody fireball;
    public Rigidbody iceball;
    public Transform playerTransform;
	public LayerMask movementMask;	// Filter out everything not walkable
    public int spellTravelSpeed = 25;

    float pushSpellRadius = 10f;
    float pushSpellForce = 15000f;
    float pushSpellVelocityMag = 10f;

    float freezeSpellRadius = 10f;
    int freezeSpellDurationInSeconds = 5;

	Camera cam;			// Reference to our camera
	PlayerMotor motor;	// Reference to our motor

	// Get references
	void Start () {
		cam = Camera.main;
		motor = GetComponent<PlayerMotor>();
	}
	
	// Update is called once per frame
	void Update () {

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		// If we press left mouse
		if (Input.GetMouseButtonDown(0))
		{
			// We create a ray
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// If the ray hits
			if (Physics.Raycast(ray, out hit, 100, movementMask))
			{
				motor.MoveToPoint(hit.point);   // Move to where we hit
				RemoveFocus();
			}
		}

		// If we press right mouse
		if (Input.GetMouseButtonDown(1))
		{
			// We create a ray
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// If the ray hits
			if (Physics.Raycast(ray, out hit, 100))
			{
				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
					SetFocus(interactable);
				}
			}
		}

        //If we press 1
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //do something to cast fireball
            Debug.Log("Fireball!");
            CastFireball();
        }

        //If we press 2
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Iceball!");
            CastIceball();
        }

        //If we press 3
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Push Spell!");
            CastPushSpell();
        }

        //if we press 4
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Freeze Spell!");
            CastFreezeSpell();
        }
	}

	// Set our focus to a new focus
	void SetFocus (Interactable newFocus)
	{
		// If our focus has changed
		if (newFocus != focus)
		{
			// Defocus the old one
			if (focus != null)
				focus.OnDefocused();

			focus = newFocus;	// Set our new focus
			motor.FollowTarget(newFocus);	// Follow the new focus
		}
		
		newFocus.OnFocused(transform);
	}

	// Remove our current focus
	void RemoveFocus ()
	{
		if (focus != null)
			focus.OnDefocused();

		focus = null;
		motor.StopFollowingTarget();
	}

    void CastFireball()
    {

        //instantiate a fireball sphere and its cast point in the world relative to the player
        Vector3 castPoint = CalculateCastPoint();
        
        Quaternion playerRotation = playerTransform.rotation;
        Rigidbody fireball1 = Instantiate(fireball, castPoint, playerRotation);
        //set velocity of the sphere in the direction player is facing
        fireball1.velocity = transform.forward * spellTravelSpeed;

    }

    void CastIceball()
    {
        //instantiate a fireball sphere and its cast point in the world relative to the player
        Vector3 castPoint = CalculateCastPoint();
        Quaternion playerRotation = playerTransform.rotation;
    
        Rigidbody iceballDuplicate = Instantiate(iceball, castPoint, playerRotation);
        iceballDuplicate.velocity = transform.forward * spellTravelSpeed;
    }

    Vector3 CalculateCastPoint()
    {
        Vector3 castPoint;
        Vector3 playerPosition = playerTransform.position;
        Vector3 playerDirection = playerTransform.forward;
        Quaternion playerRotation = playerTransform.rotation;
        float castDistance = 1f;
        castPoint = playerPosition + playerDirection * castDistance;
        castPoint.y += 1; //this casts the spell at chest level instead of ground level
        return castPoint;
    }

    void CastPushSpell()
    {
        Vector3 castPoint = CalculateCastPoint();
        //grab list of objects within some distance sphere
        Collider[] nearbyColliders = Physics.OverlapSphere(CalculateCastPoint(), pushSpellRadius);

        foreach(Collider col in nearbyColliders)
        {
            //iterate over array, checking if each object is an enemy
            //if yes, add a velocity directly to the enemy, as animated objects wont accept an explosion force

            EnemyStats temp = col.GetComponent<EnemyStats>();
            if(temp != null)
            {
                Debug.Log("push spell found an enemy");
                //having trouble applying velocity to the skeleton enemy, so try disabling any animation first
                Animator tempAnimator = temp.GetComponentInChildren(typeof(Animator)) as Animator;
                tempAnimator.enabled = false;
                

                Rigidbody rb = temp.GetComponent<Rigidbody>();
                Vector3 pushVector;
                pushVector = (temp.transform.position - CalculateCastPoint()) * pushSpellVelocityMag;
                rb.velocity += pushVector;

                tempAnimator.enabled = true;

            }

            //debug to allow pushing non-enemy rigidbodies
            Rigidbody rbTemp = col.GetComponent<Rigidbody>();
            if(rbTemp != null)
            {
                Debug.Log("push spell found non-enemy");
                rbTemp.AddExplosionForce(pushSpellForce, castPoint, pushSpellRadius);
            }
            
        }

    }

    void CastFreezeSpell()
    {
        Collider[] localColliders = Physics.OverlapSphere(CalculateCastPoint(), freezeSpellRadius);
        foreach(Collider col in localColliders)
        {
            if(col.GetComponent<EnemyStats>() != null)
            {
                Animator tempAnimator = col.GetComponentInChildren(typeof(Animator)) as Animator;
                Debug.Log("freezing enemy animator");
                tempAnimator.enabled = false;
                StartCoroutine(DelayedUnfreeze(tempAnimator));
            }
        }
    }

    IEnumerator DelayedUnfreeze(Animator animator)
    {
        //wait for freeze spell duration, and then reenable the frozen animator and complete the unfreezing phase
        yield return new WaitForSeconds(freezeSpellDurationInSeconds);
        Debug.Log("finished coroutine wait time");
        animator.enabled = true;
        Debug.Log("enemy animator has been reenabled");

    }
}

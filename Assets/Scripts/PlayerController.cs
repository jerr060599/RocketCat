/*
 * A script that handles user input and controls the player avatar
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//This scripts requires that the object it is on of a rigidbody as well
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    [Header("Health")]
    public float maxHealth = 100f;          //The maximum health the player can have
    public float health = 100f;             //How much health the player currently have
    public float damageThreshold = 3f;      //The max impact impulse the player can take before sustaining damage
    public float damagePerImpulse = 0.01f;  //The ratio between damage taken and conllision impulse beyond the threshold
    public bool invincible = false;         //Whether the playe can take damage
    public Vector3 killBound = new Vector3(100, 100, 100);//Kill the player if he gets out of this zone

    [Header("GUI")]
    public float bloodFade = 2f;            //How fast the blood effect fades
    public float bloodIntesity = 1f;        //How intense the effect starts out
    float curBlood = 0f;                    //How intense the effect is currently

    [Header("Controls")]
    public float maxTurnSpeed = 3f;     //The max turn speed
    //Controls the magnitude of the fade vector for both turn speed and turn torque
    //0.5f Would mean the controller will try to turn at max speed when they are 60 degrees out of aligngment (aCos(0.5f) = 60 deg)
    [Range(0.01f, 1)]//This tells the inspector window to add a slider for this variable
    public float fadeArc = 0.3f;
    public float thrust = 20f;          //The max thrust of the cat
    public bool useMouseControls = false;//Whether to read from mouse controls or not

    [Header("References")]
    public GameObject explosionPrefab;//Prefab of the explosion
    public Animator thrusters;      //A reference to the animation controller for the flames
    public Slider healthbar;        //A reference to the health slider
    public Text heathText;          //A reference to the health slider
    public Image bloodScreen;       //A reference to the blood image
    public FocusLayer focusLayer;   //The focus layer the player controls are on
    public GameObject replayScreen; //A reference to the replay screen
    public GameObject lossScreen;   //A reference to the loss screen
    public GameObject winScreen;    //A reference to the win screen
    Rigidbody2D body = null;        //A reference to the rigidbody component of the player

    // Awake is called at object spawn (even before Start())
    void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    //Update is called once per frame or once everytime the screen is re-rendered
    //Other things like physics can update slower or faster than update() in fixedupdate
    //Everything about updating the graphics/animation/display will be put here
    void Update() {
        //Blood animation
        if (curBlood != 0f) {
            curBlood *= Mathf.Exp(-bloodFade * Time.deltaTime);
            if (curBlood <= 0.01f)
                curBlood = 0f;
            Color c = bloodScreen.color;
            c.a = curBlood;
            bloodScreen.color = c;
        }
    }

    //Everything physics related NEEDS to be in fixedupdate because thats when the physics get updated.
    void FixedUpdate() {
        if (focusLayer.inFocus) {
            if (useMouseControls) {
                //Turn control
                //Get a target direction vector that points from the charator to the cursor in object space
                Vector3 tarDir = transform.worldToLocalMatrix.MultiplyPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Make sure z is zero
                tarDir.z = 0f;
                //No input is possible if the vector is zero
                if (tarDir != Vector3.zero) {
                    //Make it a unit vector
                    tarDir.Normalize();
                    /*
                     * This is needed because we needed to satisfy a few conditions for the controls to feel natural
                     * 1. The turn speed needs to be 0 when the mouse is perfectly aligned
                     * 2. The turn speed needs to be capped at maxTurnSpeed
                     * 3. As the cat gets closer to facing the cursor, it needs to slow down before stoping
                     * 
                     * To satisfy those conditions, this line takes the x component of the unit vector and use that for turn control.
                     * If the y component is negative, the cursor is behind the cat and so the cat turns at max speed in the direction of the x component.
                     */
                    body.angularVelocity = -maxTurnSpeed * Mathf.Clamp(tarDir.y > 0 ? tarDir.x / fadeArc : Mathf.Sign(tarDir.x), -1f, 1f);
                }

                //Thrusting
                if (Input.GetMouseButton(0)) {
                    body.AddRelativeForce(new Vector2(0, thrust), ForceMode2D.Force);
                    thrusters.SetBool("Flameing", true);
                }
                else
                    thrusters.SetBool("Flameing", false);
            }
            else {
                //Turn control
                body.angularVelocity = -maxTurnSpeed * Input.GetAxisRaw("Horizontal");

                //Thrusting
                float input = Mathf.Max(0, Input.GetAxisRaw("Vertical"));
                body.AddRelativeForce(new Vector2(0, input * thrust), ForceMode2D.Force);
                thrusters.SetBool("Flameing", input > 0);
            }
        }

        //Check kill bound
        if (Mathf.Abs(transform.position.x) > killBound.x ||
            Mathf.Abs(transform.position.y) > killBound.y ||
            Mathf.Abs(transform.position.z) > killBound.z)
            Damage(float.PositiveInfinity);
    }

    static ContactPoint2D[] contacts = new ContactPoint2D[10];  //A reusable array of references to contact point objects

    //This is called whenever this rigidbodys collider intersect another
    void OnCollisionEnter2D(Collision2D collision) {

        //Get all contact points, convert impulse to damage, and apply damage
        int numContacts = collision.GetContacts(contacts);
        float dmg = 0f;
        for (int i = 0; i < numContacts; i++) {
            DamageMultiplier dm = contacts[i].collider.GetComponent<DamageMultiplier>();
            if (dm)
                dmg += Mathf.Max(0, (contacts[i].normalImpulse - damageThreshold) * damagePerImpulse) * dm.multiplier;
            else
                dmg += Mathf.Max(0, (contacts[i].normalImpulse - damageThreshold) * damagePerImpulse);
        }

        Damage(dmg);
    }

    //Called to damage the player
    public void Damage(float dmg) {
        if (dmg <= 0f || health < 0f || invincible)
            return;
        health -= dmg;                                      //Apply damage

        healthbar.value = health / maxHealth;               //Update health bar
        heathText.text = health.ToString(".#") + "/100 HP"; //Format and update health text

        curBlood = bloodIntesity;                           //Blood effect

        if (health <= 0f)                                   //Call OnDeath() if health dips bellow zero
            OnDeath();
    }

    //Called when the player dies
    void OnDeath() {
        //Instantiate an explosion effect
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //Freeze this physics
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        //Disable all sprites
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            r.enabled = false;

        OnLoss();
    }

    //Reloads this scene
    public void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Enables the right menus
    public void OnLoss() {
        invincible = true;
        replayScreen.SetActive(true);
        lossScreen.SetActive(true);
    }

    //Enables the right menus
    public void OnWin() {
        invincible = true;
        replayScreen.SetActive(true);
        winScreen.SetActive(true);
    }
}

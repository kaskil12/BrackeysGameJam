using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Reflection;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Movement")]
    public Camera MyCam;
    public float Sensitivity;
    float camX;

    [Header("Rigidbody Movement")]
    public Rigidbody rb;
    public float speed;
    public bool Looks;
    public bool Sliding;
    public bool SlidAble;

    [Header("Jumping")]
    public GameObject JumpObject;
    public float jumprad;
    public float JumpPower;
    bool IsGrounded;
    public bool Jumpable = true;

    [Header("Sliding")]
    public bool hasAppliedForce;

    [Header("PlayerComponents")]
    public GameObject HandObject;
    public float Health = 100;
    public float MaxHealth = 100;
    public bool Walking;
    public bool Running;
    public float CapsuleHeight;
    public float WalkSpeed;
    public float RunningSpeed;
    public GameObject HipPosition;
    public GameObject AimPosition;
    public bool Aiming;
    public float HandLerpSpeed;

    [Header("Inventory")]
    public GameObject[] Inventory;
    public float InventoryLerpSpeed;
    public List<string> KeyList;
    public int CurrentObject;
    

    [Header("Interaction")]
    public float RayLength = 10;

    [Header("UI")]
    public TMP_Text PerkMessageText;
    public TMP_Text InteractionText;
    //Icons displaying items in inventory
    public Image[] InventoryIcons;
    public TMP_Text MessageText;
    public Image HitEffect;
    public Color HitEffectColor;

    [Header("Audio")]
    public AudioSource Footsteps;
    public AudioSource JumpSound;
    public AudioSource LandSound;
    public AudioSource KeyPickup;
    public AudioSource HitSound;

    public GameObject[] obj;
    [Header("EnergyPerks")]
    public float Energy;


    // Start is called before the first frame update
    void Start()
    {
        InitializePlayer();
        HitEffectColor = new Color(HitEffectColor.r, HitEffectColor.g, HitEffectColor.b, 0);
    }

    void InitializePlayer()
    {
        Aiming = false;
        SlidAble = true;
        Jumpable = true;
        Health = 100;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Looks = true;
        speed = 50;
        //obj is every renderer in the scene
        // obj = FindObjectsOfType<Renderer>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        // {
        //     // Check for null reference
        //     if (obj != null)
        //     {
        //         // Attempt to get Collider and MeshRenderer components
        //         Collider objCollider = obj.GetComponent<Collider>();
        //         MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();

        //         // Check for null references before using components
        //         if (objCollider != null && objRenderer != null)
        //         {
        //             Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        //             if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        //             {
        //                 // Object is inside the camera frustum
        //                 //print("The object " + obj.name + " has appeared");
        //                 objRenderer.enabled = true;
        //             }
        //             else
        //             {
        //                 // Object is outside the camera frustum
        //                 //print("The object " + obj.name + " has disappeared");
        //                 objRenderer.enabled = false;
        //             }
        //         }
        //         else
        //         {
        //             // Handle the case where either the Collider or MeshRenderer is missing
        //             Debug.LogWarning("Collider or MeshRenderer missing on object: " + obj.name);
        //         }
        //     }
        // }
        Color color = HitEffectColor;
        HitEffect.color = color;

        UpdateHandPosition();
        Look();
        IconDisplay();
        Jumping();
        //Fade the hit effect
        UpdateHitEffectColor();
        if(Health < MaxHealth){
            Health += 1f * Time.deltaTime;
        }
        if(rb.velocity.magnitude > 0.1f && IsGrounded && !Footsteps.isPlaying && !Running){
            Footsteps.Play();
        }else if(rb.velocity.magnitude < 0.1f && IsGrounded && Footsteps.isPlaying){
            Footsteps.Stop();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Aiming = !Aiming;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouched = !crouched;
        }
        //Drop current item in inventory
        if(Input.GetKeyDown(KeyCode.Q)){
            HandDrop();
        }
        //Change current item in inventory
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            ChangeCurrentObject(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            ChangeCurrentObject(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            ChangeCurrentObject(2);
        }
        if(Physics.Raycast(MyCam.transform.position, MyCam.transform.forward, out RaycastHit InteractionHit, RayLength)){
            if(InteractionHit.transform.tag == "Key"){
                InteractionText.gameObject.SetActive(true);
                InteractionText.text = "(E) To Pick Up";
                if(Input.GetKeyDown(KeyCode.E)){
                    string CurrentKey = InteractionHit.transform.gameObject.name;
                    InteractionHit.transform.gameObject.SetActive(false);
                    KeyList.Add(CurrentKey);
                    KeyPickup.Play();
                }
            }else if(InteractionHit.transform.tag == "Door"){
                InteractionText.gameObject.SetActive(true);
                InteractionText.text = "(E) To Interact With Door";
                if(Input.GetKeyDown(KeyCode.E)){
                    InteractionHit.transform.GetComponent<DoorScript>().Open();
                }
            }else if(InteractionHit.transform.root.tag == "HoldAble"){
                    InteractionText.gameObject.SetActive(true);
                    InteractionText.text = "(E) To Pick Up";
                    if(Input.GetKeyDown(KeyCode.E)){
                        HandPickup(InteractionHit.transform.root.gameObject);
                    }   
                
            }else if(InteractionHit.transform.tag =="EnergySphere"){
                InteractionText.gameObject.SetActive(true);
                InteractionText.text = "(E) To Pick Up";
                if(Input.GetKeyDown(KeyCode.E)){
                    Energy += 1;
                    InteractionHit.transform.gameObject.SetActive(false);
                }
                
            }else{
                InteractionText.gameObject.SetActive(false);
            }
        }else{
            InteractionText.gameObject.SetActive(false);
        }
    }
    public void PerkVoid()
    {
        int perk = Random.Range(0, 4);

        switch (perk)
        {
            case 0:
                // Increase jump force
                JumpPower *= 1.2f; // Increase by 20%
                sendPerkMessage("Jump Force Increased");
                Debug.Log("Jump Force Increased" + JumpPower);
                break;
            case 1:
                // Increase speed
                speed *= 1.2f; // Increase by 20%
                sendPerkMessage("Speed Increased");
                Debug.Log("Speed Increased" + speed);
                break;
            case 2:
                // Increase health
                MaxHealth *= 1.1f; // Increase by 20%
                sendPerkMessage("Health Increased");
                Debug.Log("Health Increased" + MaxHealth);
                break;
            case 3:
                // Increase attack damage
                if (Inventory[CurrentObject] != null)
                {
                    StaplerScript stapler = Inventory[CurrentObject].GetComponent<StaplerScript>();
                    if (stapler != null)
                    {
                        stapler.StaplerDamage *= 1.5f; // Increase by 30%
                        sendPerkMessage("Stapler Damage Increased");
                        Debug.Log("Stapler Damage Increased" + stapler.StaplerDamage);
                    }else{
                        Debug.Log("Perk not found");
                        PerkVoid();
                    }
                }else{
                    Debug.Log("Perk not found");
                    PerkVoid();
                }
                break;
            case 4:
                if (Inventory[CurrentObject] != null)
                {
                    //increase attack speed of the weapon in the inventory
                    StaplerScript staplerFire = Inventory[CurrentObject].GetComponent<StaplerScript>();
                    if (staplerFire != null)
                    {
                        staplerFire.StaplerFireRate *= 1.5f; // Increase by 10%
                        sendPerkMessage("Stapler Fire Rate Increased");
                        Debug.Log("Stapler Fire Rate Increased" + staplerFire.StaplerFireRate);
                    }else{
                        Debug.Log("Perk not found");
                        PerkVoid();
                    }           
                }else{
                    Debug.Log("Perk not found");
                    PerkVoid();
                }
                break;
        }
    }
    void IconDisplay(){
        //Get the image from each inventory item and display it in the UI
        // if (Inventory[CurrentObject] != null){
        // MonoBehaviour[] inventoryScripts = Inventory[CurrentObject].GetComponents<MonoBehaviour>();
        // foreach (var script in inventoryScripts)
        // {
        //     MethodInfo equipMethod = script.GetType().GetMethod("Equip");
        //     if (equipMethod != null)
        //     {
        //         equipMethod.Invoke(script, null);
        //         break;
        //     }
        // }
        // }
    }
    public void TakeDamage(float Damage){
        Health -= Damage;
        HitSound.Play();
        
        if(Health <= 0){
            Die();
        }
    }
    void UpdateHitEffectColor()
    {
        float healthPercentage = Health / MaxHealth;
        HitEffectColor.a = 1 - healthPercentage;
        HitEffect.color = HitEffectColor;
    }
    void Die(){
        SendMessage("You Died");
        StartCoroutine(Delay());
    }
    IEnumerator Delay(){
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    void ChangeCurrentObject(int NewCurrentObject){
        if(Inventory[NewCurrentObject] == null){
            Inventory[CurrentObject].SetActive(false);
            CurrentObject = NewCurrentObject;
            return;
        }
        CurrentObject = NewCurrentObject;
        Inventory[CurrentObject].transform.position = HandObject.transform.position;
        Inventory[CurrentObject].transform.rotation = HandObject.transform.rotation;
        Inventory[CurrentObject].SetActive(true);
        foreach(GameObject obj in Inventory){
            if(obj != Inventory[CurrentObject] && obj != null){
                obj.SetActive(false);
            }
        }
    }
    void HandPickup(GameObject Object){
        if(Inventory[CurrentObject] == null){
            Object.GetComponent<Rigidbody>().isKinematic = true;
            Object.GetComponent<Collider>().enabled = false;
            Inventory[CurrentObject] = Object;
            Inventory[CurrentObject].transform.position = HandObject.transform.position;
            Inventory[CurrentObject].transform.rotation = HandObject.transform.rotation;
            if (Inventory[CurrentObject] != null){
                MonoBehaviour[] inventoryScripts = Inventory[CurrentObject].GetComponents<MonoBehaviour>();
                foreach (var script in inventoryScripts)
                {
                    MethodInfo equipMethod = script.GetType().GetMethod("Equip");
                    if (equipMethod != null)
                    {
                        equipMethod.Invoke(script, null);
                        break;
                    }
                }
            }
        }
    }
    public void HandDrop(){
        if(Inventory[CurrentObject] != null){
            if (Inventory[CurrentObject] != null){
                MonoBehaviour[] inventoryScripts = Inventory[CurrentObject].GetComponents<MonoBehaviour>();
                foreach (var script in inventoryScripts)
                {
                    MethodInfo equipMethod = script.GetType().GetMethod("Unequip");
                    if (equipMethod != null)
                    {
                        equipMethod.Invoke(script, null);
                        break;
                    }
                }
            }
            Inventory[CurrentObject].GetComponent<Rigidbody>().isKinematic = false;
            Inventory[CurrentObject].GetComponent<Collider>().enabled = true;
            Inventory[CurrentObject].transform.position = HandObject.transform.position;
            Inventory[CurrentObject].transform.rotation = HandObject.transform.rotation;
            Inventory[CurrentObject] = null;
        }
    }
    public bool HasKey(string Key){
        if(KeyList.Contains(Key)){
            return true;
        }else{
            return false;
        }
    }
    public void RemoveKey(string Key){
        KeyList.Remove(Key);
    }
    public void SendMessage(string Message){
        MessageText.text = Message;
        StartCoroutine(MessageDelay());
    }
    public void sendPerkMessage(string Message){
        PerkMessageText.text = Message;
        StartCoroutine(SendPerDelay());
    }
    IEnumerator SendPerDelay(){
        yield return new WaitForSeconds(3);
        PerkMessageText.text = "";
    }
    IEnumerator MessageDelay(){
        yield return new WaitForSeconds(3);
        MessageText.text = "";
    }
    void FixedUpdate()
    {
        if (Inventory[CurrentObject] != null)
        {
            Inventory[CurrentObject].transform.position = Vector3.Lerp(Inventory[CurrentObject].transform.position, HandObject.transform.position, InventoryLerpSpeed * Time.deltaTime);
            Inventory[CurrentObject].transform.rotation = Quaternion.Slerp(Inventory[CurrentObject].transform.rotation, HandObject.transform.rotation, Mathf.Clamp01(InventoryLerpSpeed * Time.deltaTime));
        }
        if(IsGrounded){
            HandleInput();
        }
        SlidingMechanics();
    }
    void Look(){
        if (Looks)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.timeScale;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.timeScale;
            transform.Rotate(transform.up * mouseX);

            camX -= mouseY;
            camX = Mathf.Clamp(camX, -70, 70);
            GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(camX, 0, 0);
        }
    }
    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = (moveX * transform.right + moveZ * transform.forward).normalized;
        Vector3 move = moveDirection * speed * Time.deltaTime;
        rb.AddForce(move, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.LeftShift) && IsGrounded)
        {
            Running = true;
            speed = RunningSpeed;
        }
        else if (IsGrounded)
        {
            speed = WalkSpeed;
            Running = false;
        }

    }
    bool Landed = false;
        void Jumping(){
        IsGrounded = false;
        foreach(Collider i in Physics.OverlapSphere(JumpObject.transform.position, jumprad)){
            if(i.transform.tag != "Player"){
                IsGrounded = true;
                if(!Landed){
                    LandSound.Play();
                    Landed = true;
                }
                break;
            }
        }
        if(IsGrounded){
            if(Input.GetKeyDown(KeyCode.Space) && !Sliding && Jumpable == true){
                StartCoroutine(JumpDelay());
                JumpSound.Play();
                rb.AddForce(transform.up * JumpPower, ForceMode.VelocityChange);
            }
        }
        
        if(!Sliding){
            rb.drag = IsGrounded ? 15 : 0.1f;
        }
    }
    IEnumerator JumpDelay(){
        Jumpable = false;
        yield return new WaitForSeconds(0.1f);
        Landed = false;
        Jumpable = true;
    }

    bool crouched;
    float crouchspeed = 10000;
    void SlidingMechanics()
    {
        if (crouched)
        {
            CapsuleHeight = 1;
        }
        else
        {
            CapsuleHeight = 2;
        }

        float currentHeight = GetComponentInChildren<CapsuleCollider>().height;
        float targetHeight = Mathf.Lerp(currentHeight, CapsuleHeight, crouchspeed * Time.deltaTime);
        GetComponentInChildren<CapsuleCollider>().height = targetHeight;
    }

    // void ApplySlideForce()
    // {
    //     if (Sliding && !hasAppliedForce && SlidAble && rb.velocity.magnitude > 1)
    //     {
    //         Debug.Log("Force");
    //         StartCoroutine(SlideForceDelay());
    //         rb.AddForce(transform.forward * 20 * Time.deltaTime, ForceMode.VelocityChange);
    //         hasAppliedForce = true;
    //     }
    // }

    void UpdateHandPosition()
    {
        Vector3 aimPositionWorld = AimPosition.transform.position;
        Vector3 hipPositionWorld = HipPosition.transform.position;
        HandObject.transform.position = Vector3.Lerp(HandObject.transform.position, Aiming ? aimPositionWorld : hipPositionWorld, HandLerpSpeed * Time.deltaTime);
    }

    IEnumerator SlideForceDelay()
    {
        SlidAble = false;
        yield return new WaitForSeconds(0.4f);
        SlidAble = true;
    }
}

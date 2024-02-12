using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Reflection;

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
    public float RayLength = 5;

    [Header("UI")]
    public TMP_Text InteractionText;
    //Icons displaying items in inventory
    public Image[] InventoryIcons;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayer();
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandPosition();
        Look();
        IconDisplay();
        if (Input.GetMouseButtonDown(1))
        {
            Aiming = !Aiming;
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
                }
            }else if(InteractionHit.transform.tag == "Door"){
                InteractionText.gameObject.SetActive(true);
                InteractionText.text = "(E) To Open Door";
                if(Input.GetKeyDown(KeyCode.E)){
                    InteractionHit.transform.root.GetComponent<DoorScript>().Open();
                }
            }else if(InteractionHit.transform.root.tag == "HoldAble"){
                    InteractionText.gameObject.SetActive(true);
                    InteractionText.text = "(E) To Pick Up";
                    if(Input.GetKeyDown(KeyCode.E)){
                        HandPickup(InteractionHit.transform.root.gameObject);
                    }   
                
            }else{
                InteractionText.gameObject.SetActive(false);
            }
        }else{
            InteractionText.gameObject.SetActive(false);
        }
    }
    void IconDisplay(){
        //Get the image from each inventory item and display it in the UI
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
    void ChangeCurrentObject(int NewCurrentObject){
        if(Inventory[NewCurrentObject] == null){
            Inventory[CurrentObject].SetActive(false);
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
            Object.GetComponent<MeshCollider>().enabled = false;
            Inventory[CurrentObject] = Object;
            Inventory[CurrentObject].transform.position = HandObject.transform.position;
            Inventory[CurrentObject].transform.rotation = HandObject.transform.rotation;
        }
    }
    void HandDrop(){
        if(Inventory[CurrentObject] != null){
            Inventory[CurrentObject].GetComponent<Rigidbody>().isKinematic = false;
            Inventory[CurrentObject].GetComponent<MeshCollider>().enabled = true;
            Inventory[CurrentObject].transform.position = HandObject.transform.position;
            Inventory[CurrentObject].transform.rotation = HandObject.transform.rotation;
            Inventory[CurrentObject] = null;
        }
    }

    void FixedUpdate()
    {
        if (Inventory[CurrentObject] != null)
        {
            Inventory[CurrentObject].transform.position = Vector3.Lerp(Inventory[CurrentObject].transform.position, HandObject.transform.position, InventoryLerpSpeed * Time.deltaTime);
            Inventory[CurrentObject].transform.rotation = Quaternion.Slerp(Inventory[CurrentObject].transform.rotation, HandObject.transform.rotation, Mathf.Clamp01(InventoryLerpSpeed * Time.deltaTime));
        }
        if(IsGrounded && !Sliding){
            HandleInput();
        }
        ApplySlideForce();
        SlidingMechanics();
        Jumping();
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
        void Jumping(){
        IsGrounded = false;
        foreach(Collider i in Physics.OverlapSphere(JumpObject.transform.position, jumprad)){
            if(i.transform.tag != "Player"){
                IsGrounded = true;
                break;
            }
        }
        if(IsGrounded){
            if(Input.GetKeyDown(KeyCode.Space) && !Sliding && Jumpable == true){
                StartCoroutine(JumpDelay());
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
        Jumpable = true;
    }

    void SlidingMechanics()
    {
        if (Input.GetKey(KeyCode.C) && IsGrounded)
        {
            if (Input.GetKeyDown(KeyCode.C) && !Sliding)
            {
                Sliding = true;
            }

            Sliding = true;
            GetComponentInChildren<CapsuleCollider>().material.dynamicFriction = 0;
            GetComponentInChildren<CapsuleCollider>().material.staticFriction = 0f;
            CapsuleHeight = Mathf.Lerp(CapsuleHeight, 1, 0.05f);
            rb.drag = 0.2f;
            rb.mass = 10;
        }
        else
        {
            Sliding = false;
            GetComponentInChildren<CapsuleCollider>().material.dynamicFriction = 0.6f;
            GetComponentInChildren<CapsuleCollider>().material.staticFriction = 0.6f;
            GetComponentInChildren<CapsuleCollider>().height = CapsuleHeight;
            rb.mass = 1;
            CapsuleHeight = Mathf.Lerp(CapsuleHeight, 2, 0.05f);
            hasAppliedForce = false;
        }

        GetComponentInChildren<CapsuleCollider>().height = CapsuleHeight;
    }

    void ApplySlideForce()
    {
        if (Sliding && !hasAppliedForce && SlidAble && rb.velocity.magnitude > 1)
        {
            Debug.Log("Force");
            StartCoroutine(SlideForceDelay());
            rb.AddForce(transform.forward * 20 * Time.deltaTime, ForceMode.VelocityChange);
            hasAppliedForce = true;
        }
    }

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

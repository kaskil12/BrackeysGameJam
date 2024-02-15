using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PaperScript : MonoBehaviour
{
    [Header(" Components")]
    public Animator Anim;
    public PlayerMovement player;
    [Header(" Audio")]
    public AudioSource IdleSound;
    public AudioSource ChaseSound;
    public AudioSource AttackSound;
    public AudioSource DeathSound;
    public AudioSource HurtSound;
    public Rigidbody rb;
    [Header(" Stats")]
    public float Health;
    public float Damage;
    public float Speed;
    public float AttackRange;
    public float ChaseRange;
    public float AttackTimer;
    public float rotationSpeed;
    bool CanAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        rb.useGravity = false;
        Health = 20;
        // Anim = GetComponentsInChildren<Animator>()[0];
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthVoid();
        // if(Vector3.Distance(transform.position, player.transform.position) <= AttackRange){
        //     Attack();
        // }
        if(Vector3.Distance(transform.position, player.transform.position) <= ChaseRange && Health >1){
            Chase();
        }else{
            Idle();
            if(Health > 0){
            rb.isKinematic = true;
            }else{
                rb.isKinematic = false;
            }
            // GetComponentInChildren<Collider>().isTrigger = false;
        }
        

    }
    public void HealthVoid(){
        if(Health <= 0){
            // Anim.SetBool("Dead", true);
            DeathSound.Play();
            StartCoroutine(Delay());
        }
    }
    // public void Attack(){
    //     if(Vector3.Distance(transform.position, player.transform.position) <= AttackRange && CanAttack){
    //         Anim.SetBool("Attack", true);
    //         if(!AttackSound.isPlaying)AttackSound.Play();
    //         player.TakeDamage(Damage);
    //         //Delay between attacks
    //         StartCoroutine(AttackDelay());
    //     }
    // }
    // IEnumerator AttackDelay(){
    //     CanAttack = false;
    //     yield return new WaitForSeconds(AttackTimer);
    //     Anim.SetBool("Attack", false);
    //     CanAttack = true;
    // }
    public void Chase(){
        if(Vector3.Distance(transform.position, player.transform.position) <= ChaseRange && Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, ChaseRange) && hit.transform.tag == "Player"){
            // Anim.SetBool("Chase", true);
            // GetComponentInChildren<Collider>().isTrigger = true;
            rb.isKinematic = false;
            rb.AddForce((player.transform.position - transform.position).normalized * Speed, ForceMode.VelocityChange);
            //Lerp rotation
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity + transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            // if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit2, 2)){
            //     if(hit2.transform != null){
            //         rb.AddForce(Vector3.up * 1, ForceMode.Impulse);
            //     }
            // }
        }
    }
    public void PlayRunSound(){
        ChaseSound.Play();
    }
    public void Idle(){
        // Anim.SetBool("Chase", false);
        IdleSound.Play();
    }
    public void TakeDamage(float damage){
        Health -= damage;
        HurtSound.Play();
    }
    IEnumerator Delay(){
        rb.useGravity = true;
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonitorScript : MonoBehaviour
{
    [Header(" Components")]
    public Animator Anim;
    public NavMeshAgent Agent;
    public PlayerMovement player;
    [Header(" Audio")]
    public AudioSource IdleSound;
    public AudioSource ChaseSound;
    public AudioSource AttackSound;
    public AudioSource DeathSound;
    public AudioSource HurtSound;
    public ParticleSystem ExplodeEffect;
    public GameObject EnergySphere;
    public bool Died = false;
    [Header(" Stats")]
    public float Health;
    public float Damage;
    public float Speed;
    public float AttackRange;
    public float ChaseRange;
    public float AttackTimer;
    bool CanAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        Died = false;
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        Agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        HealthVoid();
        if(Vector3.Distance(transform.position, player.transform.position) <= AttackRange){
            if(transform.parent != null)transform.parent = null;
            if(!Agent.enabled)Agent.enabled = true;
            Attack();
        }else if(Vector3.Distance(transform.position, player.transform.position) <= ChaseRange){
            if(transform.parent != null)transform.parent = null;
            if(!Agent.enabled)Agent.enabled = true;
            Chase();
        }else{
            Idle();
        }

    }
    public void HealthVoid(){
        if(Health <= 0){
            Anim.SetBool("Dead", true);
            Agent.isStopped = true;
            Agent.velocity = Vector3.zero;
            Agent.enabled = false;
            DeathSound.Play();
            StartCoroutine(Delay());
        }
    }
    public void Attack(){
        if(Vector3.Distance(transform.position, player.transform.position) <= AttackRange && CanAttack){
            Anim.SetBool("Attack", true);
            if(!AttackSound.isPlaying)AttackSound.Play();
            player.TakeDamage(Damage);
            //Delay between attacks
            Agent.speed = 0;
            StartCoroutine(AttackDelay());
        }
    }
    IEnumerator AttackDelay(){
        CanAttack = false;
        yield return new WaitForSeconds(AttackTimer);
        Anim.SetBool("Attack", false);
        Agent.speed = Speed;
        CanAttack = true;
    }
    public void Chase(){
        if(Vector3.Distance(transform.position, player.transform.position) <= ChaseRange && Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, ChaseRange) && hit.transform.tag == "Player"){
            Agent.speed = Speed;
            Agent.SetDestination(player.transform.position);
            Anim.SetBool("Chase", true);
        }
    }
    public void PlayRunSound(){
        ChaseSound.Play();
    }
    public void Idle(){
        Agent.speed = 0;
        Anim.SetBool("Chase", false);
        IdleSound.Play();
    }
    public void TakeDamage(float damage){
        Health -= damage;
        HurtSound.Play();
    }
    IEnumerator Delay(){
        if(!Died){
        Died = true;
        ExplodeEffect.Play();
        // GameObject Sphere = Instantiate(EnergySphere, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        }
    }
}

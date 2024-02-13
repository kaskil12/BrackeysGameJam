using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienScript : MonoBehaviour
{
    [Header("Alien Components")]
    public Animator AlienAnim;
    public NavMeshAgent AlienAgent;
    public PlayerMovement player;
    [Header("Alien Audio")]
    public AudioSource AlienIdleSound;
    public AudioSource AlienChaseSound;
    public AudioSource AlienAttackSound;
    public AudioSource AlienDeathSound;
    public AudioSource AlienHurtSound;
    [Header("Alien Stats")]
    public float AlienHealth;
    public float AlienDamage;
    public float AlienSpeed;
    public float AlienAttackRange;
    public float AlienChaseRange;
    public float AlienAttackTimer;
    bool CanAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Health();
        if(AlienAgent.enabled){
            if(Vector3.Distance(transform.position, player.transform.position) <= AlienAttackRange){
                Attack();
            }else if(Vector3.Distance(transform.position, player.transform.position) <= AlienChaseRange){
                Chase();
            }else{
                Idle();
            }
        }

    }
    public void Health(){
        if(AlienHealth <= 0){
            AlienAnim.SetBool("Dead", true);
            AlienAgent.isStopped = true;
            AlienAgent.velocity = Vector3.zero;
            AlienAgent.enabled = false;
            AlienDeathSound.Play();
            StartCoroutine(Delay());
        }
    }
    public void Attack(){
        if(Vector3.Distance(transform.position, player.transform.position) <= AlienAttackRange && CanAttack){
            AlienAnim.SetBool("Attack", true);
            AlienAttackSound.Play();
            player.TakeDamage(AlienDamage);
            //Delay between attacks
            AlienAgent.speed = 0;
            StartCoroutine(AttackDelay());
        }
    }
    IEnumerator AttackDelay(){
        CanAttack = false;
        yield return new WaitForSeconds(AlienAttackTimer);
        AlienAnim.SetBool("Attack", false);
        AlienAgent.speed = AlienSpeed;
        CanAttack = true;
    }
    public void Chase(){
        if(Vector3.Distance(transform.position, player.transform.position) <= AlienChaseRange){
            AlienAgent.speed = AlienSpeed;
            AlienAgent.SetDestination(player.transform.position);
            AlienAnim.SetBool("Chase", true);
            AlienChaseSound.Play();
        }
    }
    public void Idle(){
        AlienAgent.speed = 0;
        AlienAnim.SetBool("Chase", false);
        AlienIdleSound.Play();
    }
    public void TakeDamage(float damage){
        AlienHealth -= damage;
        AlienHurtSound.Play();
    }
    IEnumerator Delay(){
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}

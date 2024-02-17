using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Reflection;
using UnityEngine.SceneManagement;
public class StaplerScript : MonoBehaviour
{
    public Animator StaplerAnim;
    public AudioSource StaplerFireSound;
    public AudioSource StaplerReloadSound;
    public AudioSource StaplerEquipSound;
    public AudioSource StaplerUnequipSound;
    public float StaplerAmmo;
    public float StaplerDamage;
    public float StaplerReloadTime;
    public float StaplerFireRate;
    public float StaplerRange;
    public bool StaplerEquipped;
    public bool StaplerReloading;
    public bool StaplerFiring;
    public Camera MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        StaplerAmmo = 10;
        StaplerEquipped = false;
        StaplerReloading = false;
        StaplerFiring = false;
        MainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!StaplerEquipped)return;
   
        if(Input.GetKeyDown(KeyCode.R)){
            Reload();
        }
        
        if(Input.GetMouseButtonDown(0) && StaplerAmmo > 0 && !StaplerFiring && !StaplerReloading){
            Fire();
        }
    }
    public void Equip(){
        StaplerEquipped = true;
    }
    void Fire(){
        if(StaplerAmmo > 0 && !StaplerFiring){
            StaplerFiring = true;
            StaplerAnim.SetTrigger("Fire");
            StaplerFireSound.Play();
            RaycastHit hit;
            if(Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hit, StaplerRange)){
                if(hit.transform.tag == "Enemy"){
                    MonoBehaviour[] hitScripts = hit.transform.GetComponents<MonoBehaviour>();
                    foreach (var script in hitScripts)
                    {
                        MethodInfo equipMethod = script.GetType().GetMethod("TakeDamage");
                        if (equipMethod != null)
                        {
                            equipMethod.Invoke(script, new object[] { StaplerDamage });
                            break;
                        }
                    }
                }
            }
            StaplerAmmo--;
            StartCoroutine(FireDelay());
        }
    }
    IEnumerator FireDelay(){
        yield return new WaitForSeconds(StaplerFireRate);
        StaplerFiring = false;
    }
    void Reload(){
        if(StaplerAmmo < 10 && !StaplerReloading){
            StartCoroutine(ReloadDelay());
        }
    }
    IEnumerator ReloadDelay(){
        StaplerReloading = true;
        StaplerAnim.SetTrigger("Reload");
        StaplerReloadSound.Play();
        yield return new WaitForSeconds(StaplerReloadTime);
        StaplerAmmo = 10;
        StaplerReloading = false;
    }
    public void Unequip(){
        StaplerEquipped = false;
    }
}

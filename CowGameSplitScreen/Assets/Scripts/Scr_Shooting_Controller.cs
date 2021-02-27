using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Shooting_Controller : MonoBehaviour
{
    [SerializeField] private GameObject shooterObj;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform temporaryStorage;
    public float projectileSpeed;

    int currentGun;


    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void Shoot(float percent)
    {

        switch (currentGun)
        {
            case 0:
                print("shot");
                ShootA(percent);
                break;
            case 1:
                ShootB(percent);
                break;
            case 2:
                ShootC(percent);
                break;
        }
    }

    public void ShootA(float percent)
    {
        StartCoroutine(ShootACoroutine(percent));
    }

    IEnumerator ShootACoroutine(float percent) //3 weak but rapid shots
    {
        int shots = (int)(percent * 3.2f);
        print("percent: " + percent);
        print(shots);

        while (shots > 0)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
            Rigidbody rb = newProjectile.GetComponent<Rigidbody>();


            newProjectile.GetComponent<Projectile>().damage = GunsData.instance.gunADamage;

            newProjectile.transform.localScale = Vector3.one * GunsData.instance.gunASize;

            rb.velocity = newProjectile.transform.forward * GunsData.instance.gunASpeed;

            yield return new WaitForSeconds(.3f);
            shots -= 1;
            yield return null;
        }
        yield return null;







        //yield return new WaitForSeconds(0.3f);

        //newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
        //rb = newProjectile.GetComponent<Rigidbody>();


        //newProjectile.GetComponent<Projectile>().damage = GunsData.instance.gunADamage;

        //newProjectile.transform.localScale = Vector3.one * GunsData.instance.gunASize;

        //rb.velocity = newProjectile.transform.forward * GunsData.instance.gunASpeed; ;

        //yield return new WaitForSeconds(0.3f);

        //newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
        //rb = newProjectile.GetComponent<Rigidbody>();


        //newProjectile.GetComponent<Projectile>().damage = GunsData.instance.gunADamage;

        //newProjectile.transform.localScale = Vector3.one * GunsData.instance.gunASize;

        //rb.velocity = newProjectile.transform.forward * GunsData.instance.gunASpeed; 


    }

    void ShootB(float percent)
    {
        print("b");
        GameObject newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();


        newProjectile.GetComponent<Projectile>().damage = GunsData.instance.gunBDamage;

        newProjectile.GetComponent<Projectile>().size = GunsData.instance.gunBSize;

        rb.velocity = newProjectile.transform.forward * GunsData.instance.gunBSpeed; ;
    }

    void ShootC(float percent)
    {
        print("c");
        GameObject newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();


        newProjectile.GetComponent<Projectile>().damage = GunsData.instance.gunCDamage;

        newProjectile.GetComponent<Projectile>().size = GunsData.instance.gunCSize;

        rb.velocity = newProjectile.transform.forward * GunsData.instance.gunCSpeed; ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 11)
        {
            currentGun = (int)other.GetComponent<GunPickup>().currentGun;
        }
    }


    IEnumerator WaitToShoot(float waitTime, float percent)
    {
        yield return new WaitForSeconds(waitTime);
        Shoot(percent - .33333f);
        yield return null;
    }
}

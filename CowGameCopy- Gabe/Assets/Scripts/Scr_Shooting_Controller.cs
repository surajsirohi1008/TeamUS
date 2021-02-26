using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Shooting_Controller : MonoBehaviour
{
    [SerializeField] private GameObject shooterObj;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform temporaryStorage;
    public float projectileSpeed;

    // Update is called once per frame
    void Update()
    {

    }
    public void Shoot(float percent)
    {
        GameObject newProjectile = Instantiate(projectilePrefab, shooterObj.transform.position, shooterObj.transform.rotation, temporaryStorage);
        Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
        rb.velocity = newProjectile.transform.forward * projectileSpeed;
        float shotLevel = Mathf.Clamp((percent * 3) + 1, 1, 3);
        if (shotLevel > 2)
        {
            StartCoroutine(WaitToShoot(.1f,percent));
        }
    }
    IEnumerator WaitToShoot(float waitTime, float percent)
    {
        yield return new WaitForSeconds(waitTime);
        Shoot(percent - .33333f);
        yield return null;
    }
}

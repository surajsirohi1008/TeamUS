using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectMover : MonoBehaviourPun, IPunObservable
{
    private Animator _animator;

    [SerializeField]
    private float _moveSpeed, _missileSpeed;

    [SerializeField]
    private GameObject gun, _bulletPrefab;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(transform.position);
        //    stream.SendNext(transform.rotation);
        //}
        //else if (stream.IsReading)
        //{
        //    transform.position = (Vector3)stream.ReceiveNext();
        //    transform.rotation = (Quaternion)stream.ReceiveNext();
        //}.
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (base.photonView.IsMine)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            transform.position += new Vector3(x, y, 0f) * _moveSpeed;

            UpdateMovingBoolean((x != 0f || y != 0f));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("ShootMissile", RpcTarget.All);
            }
        }

    }

    private void UpdateMovingBoolean(bool moving)
    {
        _animator.SetBool("moving", moving);
    }

    [PunRPC]
    void ShootMissile()
    {
        GameObject tempMissile = MasterManager.NetworkInstantiate(_bulletPrefab, gun.transform.position, Quaternion.identity);

        tempMissile.GetComponent<Rigidbody>().velocity = new Vector3(_missileSpeed, 0, 0);

    }
}

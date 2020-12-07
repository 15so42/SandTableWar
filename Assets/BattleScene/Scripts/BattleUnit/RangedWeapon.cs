
    using Photon.Pun;
    using UnityEngine;

    public class RangedWeapon : Weapon
    {
        public Transform shootPos;
        public GameObject bullet;

        public float bulletDamageRate=1f;
        public override void Attack()
        {
            PhotonView.Get(this).RPC("FireBullet",RpcTarget.All);
        }

        [PunRPC]
        public void FireBullet()
        {
            //todo 使用对象池
            GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
            iBullet.GetComponent<Bullet>().SetWeapon(this);
            iBullet.GetComponent<Rigidbody>().AddForce(iBullet.transform.forward*800f);
        }
        
    }

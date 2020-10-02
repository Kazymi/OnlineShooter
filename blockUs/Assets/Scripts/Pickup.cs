using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Kawaiisun.SimpleHostile
{
    public class Pickup : MonoBehaviourPunCallbacks
    {
        public Gun weapon;
        public float cooldown;
        public GameObject gunDisplay;
        public List<GameObject> targets;
        [SerializeField] GameObject Effect;


        private void Start()
        {
            foreach (Transform t in gunDisplay.transform) Destroy(t.gameObject);

            GameObject newDisplay = Instantiate(weapon.display, gunDisplay.transform.position, gunDisplay.transform.rotation) as GameObject;
            newDisplay.transform.SetParent(gunDisplay.transform);
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.attachedRigidbody == null) return;

            if(other.attachedRigidbody.gameObject.tag.Equals("Player"))
            {
                Weapon weaponController = other.attachedRigidbody.gameObject.GetComponent<Weapon>();
                weaponController.photonView.RPC("PickupWeapon", RpcTarget.All, weapon.name);
                photonView.RPC("Disable", RpcTarget.All);
            }
        }

       IEnumerator Enebled()
        {
            yield return new WaitForSeconds(cooldown);
            photonView.RPC("Enable", RpcTarget.All);
        } 

        [PunRPC]
        public void Disable ()
        {
            Effect.SetActive(false);
            StartCoroutine(Enebled());
            foreach (GameObject a in targets) a.SetActive(false);
        } 
        [PunRPC]
        private void Enable ()
        {

            Effect.SetActive(true);

            foreach (GameObject a in targets) a.SetActive(true);
        } 
    }
}

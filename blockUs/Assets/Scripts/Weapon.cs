using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

namespace Com.Kawaiisun.SimpleHostile
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        public bool MinigunItAit;
        #region Variables
        private float MinigunTimer;
        [SerializeField] Camera CameraMain;
        public List<Gun> loadout;
        [HideInInspector] public Gun currentGunData;
        public GameObject DecalFly;
        public GameObject SoundFire;
        public Transform weaponParent;
        public GameObject bulletholePrefab;
        public GameObject DecalPlayer;

        public LayerMask canBeShot;
        public AudioSource sfx;
        public AudioClip hitmarkerSound;
        public bool isAiming = false;

        private float currentCooldown;
        private int currentIndex;
        private GameObject currentWeapon;

        private Image hitmarkerImage;
        private float hitmarkerWait;

        private bool isReloading;

        private Color CLEARWHITE = new Color(1, 1, 1, 0);
        Transform t_spawn;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            foreach (Gun a in loadout) a.Initialize();

            hitmarkerImage = GameObject.Find("HUD/Hitmarker/Image").GetComponent<Image>();
            hitmarkerImage.color = CLEARWHITE;
            Equip(0);
        }

        void Update()
        {
            if (Pause.paused && photonView.IsMine) return;

            if (currentWeapon.GetComponent<ScoopeSetting>())
            {
                if (isAiming)
                {
                    GetComponent<Look>().xSensitivity = float.Parse(SettingSafeLoad.MouseLookSpeedScope);
                    GetComponent<Look>().ySensitivity = float.Parse(SettingSafeLoad.MouseLookSpeedScope);
                }
                else
                {
                    GetComponent<Look>().xSensitivity = float.Parse(SettingSafeLoad.MouseLookSpeed);
                    GetComponent<Look>().ySensitivity = float.Parse(SettingSafeLoad.MouseLookSpeed);
                }
            }
            else
            {
                GetComponent<Look>().xSensitivity = float.Parse(SettingSafeLoad.MouseLookSpeed);
                GetComponent<Look>().ySensitivity = float.Parse(SettingSafeLoad.MouseLookSpeed);
            }
            if (currentWeapon != null)
            {
                if (currentWeapon.GetComponent<GunScope>())
                {
                    if (currentWeapon.GetComponent<GunScope>().Minigun)
                    {
                        currentWeapon.GetComponent<GunScope>().MininubTransform.transform.Rotate(0, 0, MinigunTimer);
                    }
                }
                if (isAiming && currentWeapon.GetComponent<GunScope>())
                {
                    CameraMain.gameObject.SetActive(false);
                    currentWeapon.GetComponent<GunScope>().cameras.gameObject.SetActive(true);
                }
                else
                {
                    if (currentWeapon.GetComponent<GunScope>())
                    {
                        CameraMain.gameObject.SetActive(true);
                        currentWeapon.GetComponent<GunScope>().cameras.gameObject.SetActive(false);
                    }
                    else
                    {
                        CameraMain.gameObject.SetActive(true);
                    }
                }
                if (currentWeapon.GetComponent<GunScope>())
                {
                    if (currentWeapon.GetComponent<GunScope>().Minigun && isAiming)
                    {
                        MinigunItAit = true;
                        if (MinigunTimer <= 12)
                        {
                            MinigunTimer += 8 * Time.deltaTime;
                        }
                        else MinigunTimer = 12;
                    }
                    else
                    {
                        MinigunItAit = false;
                        if (MinigunTimer > 0)
                        {
                            MinigunTimer -= 3 * Time.deltaTime;
                        }
                        else MinigunTimer = 0;
                    }
                }
            }


            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 0); }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2)) { photonView.RPC("Equip", RpcTarget.All, 1); }

            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {
                    if(!isReloading)
                    if (loadout[currentIndex].burst != 1)
                    {
                        if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                        {
                            if (currentWeapon.GetComponent<GunScope>())
                            {
                                if (currentWeapon.GetComponent<GunScope>().Minigun)
                                {
                                    if (isAiming)
                                    {

                                        if (loadout[currentIndex].FireBullet()) { photonView.RPC("Shoot", RpcTarget.All);
                                               

                                            }
                                        else StartCoroutine(Reload(loadout[currentIndex].reload));
                                    }
                                }
                                else
                                {
                                        if (loadout[currentIndex].FireBullet())
                                        {
                                            photonView.RPC("Shoot", RpcTarget.All); 
                                         

                                        }
                                        else StartCoroutine(Reload(loadout[currentIndex].reload));
                                }
                            }
                            else
                            {
                                if (loadout[currentIndex].FireBullet())
                                    {
                                        photonView.RPC("Shoot", RpcTarget.All); 
                                        

                                    }
                                    else StartCoroutine(Reload(loadout[currentIndex].reload));
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButton(0) && currentCooldown <= 0)
                        {
                            if (currentWeapon.GetComponent<GunScope>())
                            {
                                if (currentWeapon.GetComponent<GunScope>().Minigun)
                                {
                                    if(isAiming)
                                        if (loadout[currentIndex].FireBullet())
                                            {
                                                {
                                                    photonView.RPC("Shoot", RpcTarget.All); 
                                                    

                                                }
                                            }
                                        else StartCoroutine(Reload(loadout[currentIndex].reload));
                                }
                                else
                                {
                                    if (loadout[currentIndex].FireBullet())
                                        {
                                            photonView.RPC("Shoot", RpcTarget.All); 
                                            

                                        }
                                        else StartCoroutine(Reload(loadout[currentIndex].reload));
                                }
                            }
                            else
                            {
                                if (loadout[currentIndex].FireBullet())
                                    {
                                        photonView.RPC("Shoot", RpcTarget.All); 
                                        

                                        

                                    }
                                    else StartCoroutine(Reload(loadout[currentIndex].reload));
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R)) photonView.RPC("ReloadRPC", RpcTarget.All);

                    //cooldown
                    if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
                }

                //weapon position elasticity
                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            }

            if(photonView.IsMine)
            {
                if(hitmarkerWait > 0)
                {
                    hitmarkerWait -= Time.deltaTime;
                }
                else if(hitmarkerImage.color.a > 0)
                {
                    hitmarkerImage.color = Color.Lerp(hitmarkerImage.color, CLEARWHITE, Time.deltaTime * 2f);
                }
            }
        }

        #endregion

        #region Private Methods

        [PunRPC]
        private void ReloadRPC ()
        {
            StartCoroutine(Reload(loadout[currentIndex].reload));
        }

        IEnumerator Reload (float p_wait)
        {
            isReloading = true;

            if(currentWeapon.GetComponent<Animator>())
                currentWeapon.GetComponent<Animator>().Play("Reload", 0, 0);
            else
                currentWeapon.SetActive(false);

            yield return new WaitForSeconds(p_wait);

            loadout[currentIndex].Reload();
            currentWeapon.SetActive(true);
            isReloading = false;
        }

        [PunRPC]
        void Equip(int p_ind)
        {
            if (currentWeapon != null)
            {
                if(isReloading) StopCoroutine("Reload");
                Destroy(currentWeapon);
            }

            currentIndex = p_ind;

            GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newWeapon.transform.localPosition = Vector3.zero;
            t_newWeapon.transform.localEulerAngles = Vector3.zero;
            t_newWeapon.GetComponent<Sway>().isMine = photonView.IsMine;

            if (photonView.IsMine) ChangeLayersRecursively(t_newWeapon, 10);
            else ChangeLayersRecursively(t_newWeapon, 0);

            t_newWeapon.GetComponent<Animator>().Play("Equip", 0, 0);

            currentWeapon = t_newWeapon;
            currentGunData = loadout[p_ind];
        }

        [PunRPC]
        void PickupWeapon(string name)
        {
            Gun newWeapon = GunLibrary.FindGun(name);
            newWeapon.Initialize();

            if (loadout.Count >= 2)
            {
                loadout[currentIndex] = newWeapon;
                Equip(currentIndex);
            }
            else
            {
                loadout.Add(newWeapon);
                Equip(loadout.Count - 1);
            }
        }

        private void ChangeLayersRecursively (GameObject p_target, int p_layer)
        {
            p_target.layer = p_layer;
            foreach (Transform a in p_target.transform) ChangeLayersRecursively(a.gameObject, p_layer);
        }

       
        
        public bool Aim(bool p_isAiming)
        {
            {



                if (!currentWeapon) return false;
                t_spawn = transform.Find("Cameras/Normal Camera");
                RaycastHit hit;
                if (Physics.Raycast(t_spawn.transform.position, t_spawn.transform.forward, out hit, 2f))
                {
                    p_isAiming = false;
                    isAiming = false;
                    return false;
                } else
                
                {
                    if (isReloading) p_isAiming = false;

                    isAiming = p_isAiming;
                    Transform t_anchor = currentWeapon.transform.Find("Root");
                    Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
                    Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

                    if (p_isAiming)
                    {
                        //aim
                        t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                    }
                    else
                    {
                        //hip
                        t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                    }

                    return p_isAiming;
                }
            }
        }

        [PunRPC]
        void Shoot ()
        {
            t_spawn = transform.Find("Cameras/Normal Camera");
            if(isAiming && currentWeapon.GetComponent<GunScope>()) t_spawn = currentWeapon.GetComponent<GunScope>().cameras;
            //cooldown
            currentCooldown = loadout[currentIndex].firerate;

            for (int i = 0; i < Mathf.Max(1, currentGunData.pellets); i++)
            {
                //bloom
                GameObject Sp1 = Instantiate(currentWeapon.GetComponent<GunGameObject>().FireEffect, currentWeapon.GetComponent<GunGameObject>().Dulo.transform);
                Sp1.transform.localPosition = Vector3.zero;
                Sp1.transform.parent = null;
                Destroy(Sp1, 3f);
                Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
                t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
                t_bloom -= t_spawn.position;
                t_bloom.Normalize();

                //raycast
                RaycastHit t_hit = new RaycastHit();
                if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
                {
                    if (t_hit.transform.tag == "Player")
                    {
                        GameObject t_newHole = Instantiate(DecalPlayer, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                        t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                        Destroy(t_newHole, 5f);

                    }
                    else
                    {
                        //GameObject FlyDecal = Instantiate(DecalFly);
                        //DecalFly.transform.position = currentWeapon.GetComponent<Gun>().FireDulo.transform.position;
                        //DecalFly.transform.rotation = Quaternion.Euler(currentWeapon.GetComponent<Gun>().FireDulo.transform.rotation.x, currentWeapon.GetComponent<Gun>().FireDulo.transform.rotation.y, currentWeapon.GetComponent<Gun>().FireDulo.transform.rotation.z);
                        //Destroy(DecalFly, 0.4f);
                        GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                        t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                        Destroy(t_newHole, 5f);
                    }
                    if (photonView.IsMine)
                    {
                        //shooting other player on network
                        if (t_hit.collider.gameObject.layer == 11)
                        {
                            bool applyDamage = false;

                            if (GameSettings.GameMode == GameMode.FFA)
                            {
                                applyDamage = true;
                            }

                            if (GameSettings.GameMode == GameMode.TDM)
                            {
                                if (t_hit.collider.transform.root.gameObject.GetComponent<Player>().awayTeam != GameSettings.IsAwayTeam)
                                {
                                    applyDamage = true;
                                }
                            }

                            if (applyDamage)
                            {
                                //give damage
                                t_hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage, PhotonNetwork.LocalPlayer.ActorNumber);

                                //check for kill
                                //if (t_hit.collider.transform.root.gameObject.GetComponent<Player>().CheckKill(loadout[currentIndex].damage))
                                //{
                                //    manager.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 0, 1);
                                //}

                                //show hitmarker
                                hitmarkerImage.color = Color.white;
                                sfx.PlayOneShot(hitmarkerSound);
                                hitmarkerWait = 1f;
                            }
                        }


                        //shooting target
                        if (t_hit.collider.gameObject.layer == 12)
                        {
                            //destroy
                            Destroy(t_hit.collider.gameObject);

                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            sfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;
                        }
                    }
                }
            }

            //sound
            GameObject Sound = Instantiate(SoundFire);
            Sound.transform.position = transform.position;
            Sound.GetComponent<AudioSource>().clip = currentGunData.gunshotSound;
            Sound.GetComponent<AudioSource>().Play();
            Destroy(Sound, 6f);
            Debug.Log("PLAY SOUND " + currentGunData.gunshotSound.name);

            //gun fx
            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
            if(currentGunData.recovery) currentWeapon.GetComponent<Animator>().Play("Recovery", 0, 0);
        }

        [PunRPC]
        private void TakeDamage (int p_damage, int p_actor)
        {
            GetComponent<Player>().TakeDamage(p_damage, p_actor);
        }

        #endregion

        #region Public Methods

        public void RefreshAmmo (Text p_text)
        {
            int t_clip = loadout[currentIndex].GetClip();
            int t_stache = loadout[currentIndex].GetStash();

            p_text.text = t_clip.ToString("D2") + " / " + t_stache.ToString("D2");
        }

        #endregion
    }
}

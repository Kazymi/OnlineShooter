using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;

namespace Com.Kawaiisun.SimpleHostile
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] GameObject Setting;
        public static bool paused = false;
        private bool disconnecting = false;

        public void TogglePause()
        {
            if (disconnecting) return;

            paused = !paused;
            transform.GetChild(0).gameObject.SetActive(paused);
            Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Confined;
            Cursor.visible = paused;
        }

        public void Quit()
        {
            disconnecting = true;
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
        public void OpenSetting()
        {
            Setting.GetComponent<Canvas>().enabled = true;
        }
        private void Update()
        {
            if (!paused) Setting.GetComponent<Canvas>().enabled = false;
        }

    }
}
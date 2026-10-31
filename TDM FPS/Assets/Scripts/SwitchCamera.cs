using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Camera to Assign")]
    public GameObject AimCam; // 相机引用：用于获取瞄准相机的位置和旋转角度
    public GameObject AimCanvas; // 相机引用：用于获取瞄准相机的位置和旋转角度
    public GameObject ThirdPersonCam; // 相机引用：用于获取第三人称相机的位置和旋转角度
    public GameObject ThirdPersonCanvas; // 相机引用：用于获取第三人称相机的位置和旋转角度

    private void Update()
    {
        if (Input.GetButton("Fire2") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // 检测是否按下Fire2键（W键或上箭头键）
        {
            ThirdPersonCam.SetActive(false); // 禁用第三人称相机
            ThirdPersonCanvas.SetActive(false); // 禁用第三人称UI画布
            AimCam.SetActive(true); // 启用瞄准相机
            AimCanvas.SetActive(true); // 启用瞄准UI画布
        }

        else if (Input.GetButton("Fire2"))
        {
            ThirdPersonCam.SetActive(false); // 禁用第三人称相机
            ThirdPersonCanvas.SetActive(false); // 禁用第三人称UI画布
            AimCam.SetActive(true); // 启用瞄准相机
            AimCanvas.SetActive(true); // 启用瞄准UI画布
        }
        else
        {
            ThirdPersonCam.SetActive(true); // 启用第三人称相机
            ThirdPersonCanvas.SetActive(true); // 启用第三人称UI画布
            AimCam.SetActive(false); // 禁用瞄准相机
            AimCanvas.SetActive(false); // 禁用瞄准UI画布      
        }
    }
}

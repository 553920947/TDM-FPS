using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Camera to Assign")]
    public GameObject AimCam; // 相机引用：用于获取瞄准相机的位置和旋转角度
    public GameObject AimCanvas; // 相机引用：用于获取瞄准相机的位置和旋转角度
    public GameObject ThirdPersonCam; // 相机引用：用于获取第三人称相机的位置和旋转角度
    public GameObject ThirdPersonCanvas; // 相机引用：用于获取第三人称相机的位置和旋转角度

    [Header("Camera Animator")]
    public Animator animator; // 动画控制器引用：用于控制相机的动画状态

    private void Update()
    {
        // Fire2是鼠标右键，用于切换相机，W键或上箭头键用于瞄准相机
        if (Input.GetButton("Fire2") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // 检测是否按下Fire2键（W键或上箭头键）
        {
            animator.SetBool("Idle", false); // 设置动画状态为非空闲状态
            animator.SetBool("IdleAim", true); // 设置动画状态为瞄准状态
            animator.SetBool("AimWalk", true); // 设置动画状态为瞄准行走状态
            animator.SetBool("Walk", false); // 设置动画状态为非行走状态

            ThirdPersonCam.SetActive(false); // 禁用第三人称相机
            ThirdPersonCanvas.SetActive(false); // 禁用第三人称UI画布
            AimCam.SetActive(true); // 启用瞄准相机
            AimCanvas.SetActive(true); // 启用瞄准UI画布
        }

        else if (Input.GetButton("Fire2"))
        {
            animator.SetBool("Idle", false); // 设置动画状态为非空闲状态
            animator.SetBool("IdleAim", true); // 设置动画状态为瞄准状态
            animator.SetBool("AimWalk", false); // 设置动画状态为非瞄准行走状态
            animator.SetBool("Walk", false); // 设置动画状态为非行走状态

            ThirdPersonCam.SetActive(false); // 禁用第三人称相机
            ThirdPersonCanvas.SetActive(false); // 禁用第三人称UI画布
            AimCam.SetActive(true); // 启用瞄准相机
            AimCanvas.SetActive(true); // 启用瞄准UI画布
        }
        else
        {
            animator.SetBool("Idle", true); // 设置动画状态为空闲状态
            animator.SetBool("IdleAim", false); // 设置动画状态为非瞄准状态
            animator.SetBool("AimWalk", false); // 设置动画状态为非瞄准行走状态

            ThirdPersonCam.SetActive(true); // 启用第三人称相机
            ThirdPersonCanvas.SetActive(true); // 启用第三人称UI画布
            AimCam.SetActive(false); // 禁用瞄准相机
            AimCanvas.SetActive(false); // 禁用瞄准UI画布      
        }
    }
}

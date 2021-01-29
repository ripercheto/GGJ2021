
using UnityEngine;

partial class GameSettings
{
    public CameraSettings camera;
}

public class CameraSettings
{ 

}

public class CameraController : MonoBehaviour
{
    CameraSettings CameraSettings => Game.Settings.camera;


}

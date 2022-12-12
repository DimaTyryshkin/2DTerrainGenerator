using NaughtyAttributes;
using SiberianWellness.NotNullValidation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MicroMachines
{ 
	public class SceneInit : MonoBehaviour
	{
		public enum CarIndex
		{
			Car0 = 0,
			Car1 = 1,
			Shape,
		}

		[SerializeField, IsntNull] FollowCamera followCamera;
		[SerializeField, IsntNull] Canvas mainCanvas;
		[SerializeField, IsntNull] ThirdPersonFollowCamera thirdPersonFollowCamera;
		
		[SerializeField, IsntNull] SpeedometerPresenter speedometerPresenter;
		[SerializeField] int targetIndex;
		[SerializeField] int cameraIndex;
		
		[SerializeField, IsntNull] CarRoot[] targets;
		[SerializeField, IsntNull] GameObject[] cameras;


		void Start()
		{
			mainCanvas.gameObject.SetActive(true);
			followCamera.Init();
			thirdPersonFollowCamera.Init();
			
			SelectCar(targetIndex);
			SetCamera(cameraIndex);
		}

		public void Restart()
		{
			SceneManager.LoadScene(0);
		}

		void SelectCar(int index)
		{
			CarRoot target = targets[index];
			speedometerPresenter.SetTarget(target.transform);
			
			var simpleCarController = target.GetComponent<SimpleCarController>();
			if(simpleCarController)
				speedometerPresenter.SetTarget(simpleCarController);
			
			followCamera.SetTarget(target.cameraTarget);
			thirdPersonFollowCamera.SetTarget(target.thirdPersonCameraTarget);

			for (int i = 0; i < targets.Length; i++)
			{
				targets[i].gameObject.SetActive(index == i);
			}
		}
		
		
		[Button()]
		void ChangeCamera()
		{
			if(!Application.isPlaying)
				return;
			
			cameraIndex = (cameraIndex + 1) % cameras.Length;
			SetCamera(cameraIndex);
		}
		
		void SetCamera(int index)
		{ 
			for (int i = 0; i < cameras.Length; i++)
			{
				cameras[i].SetActive(index == i);
			}
		}
	}
}
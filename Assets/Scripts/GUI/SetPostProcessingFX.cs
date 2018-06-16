using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SetPostProcessingFX : MonoBehaviour {

	Toggle toggle;
	public PostProcessProfile profile;
	public PostProcessProfile profile2;

	PostProcessVolume volume1;
	PostProcessVolume volume2;


	void Start () {
		toggle = GetComponent<Toggle>();	
		volume1 = GameObject.Find("PostProcessGlobal").GetComponent<PostProcessVolume>();
		volume2 = GameObject.Find("PostProcessGlobalLevel").GetComponent<PostProcessVolume>();
		toggle.onValueChanged.AddListener(ActivatePosProcessing);
	}

	void ActivatePosProcessing(bool state){
		Bloom bloom;
		ColorGrading colorGrading;
		AmbientOcclusion occlusion;
		DepthOfField depthOfField;
		Vignette vignette;
		AutoExposure autoExposure;

		volume1.profile.TryGetSettings(out bloom);
		volume1.profile.TryGetSettings(out occlusion);
		volume1.profile.TryGetSettings(out depthOfField);
		volume1.profile.TryGetSettings(out vignette);
		volume1.profile.TryGetSettings(out autoExposure);
		volume1.profile.TryGetSettings(out colorGrading);
		SetSettingSpace(bloom, state);
		SetSettingSpace(occlusion, state);
		SetSettingSpace(depthOfField, state);
		SetSettingSpace(vignette, state);
		SetSettingSpace(autoExposure, state);
		SetSettingSpace(colorGrading, state);

		volume2.profile.TryGetSettings(out occlusion);
		SetSettingSpace(occlusion, state);
	}

	void SetSettingSpace(PostProcessEffectSettings settings, bool active){
		if(settings)
			settings.enabled.value = active;
	}
}

using UnityEngine;
using System.Collections;

public class MissionToolTip : MonoBehaviour {
	
	public Mission mission;
	
	void OnTooltip (bool show)
    {
		if(show)
		{
			UITooltip.ShowText(mission.GetDescription());
		}
		else
		{
			UITooltip.ShowText(null);
		}
    }
}

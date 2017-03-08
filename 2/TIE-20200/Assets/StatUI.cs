using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatUI : MonoBehaviour {
	//private StatType statType;
	public Image statImage;
	private Sprite statSprite;
	public Text statAmountText;
	private Stat stat;
	private Stat Stat{set{
			if (value!=null){

				value.OnStatChange+=Refresh;
			}
			if (stat!=null){

				stat.OnStatChange-=Refresh;
			}

			stat=value;
		}}
	void OnEnable(){
		if (stat != null) {

			stat.OnStatChange+=Refresh;
			Refresh(stat);
		}

	}
	void OnDisable(){
		if (stat != null) {

			stat.OnStatChange-=Refresh;
		}
	}
	public void Refresh(Stat _stat){
		Stat = _stat;
		statSprite = DataBase.instance.GetSprite(stat.type.ToString());
		if (!stat.useAdditionalValue) {
			statAmountText.text = stat.Value.ToString();
		} else {
			statAmountText.text = stat.AdditionalValue + "/" +  stat.Value;
		}
		if (statSprite!=null){
			statImage.sprite = statSprite;
		}

	}
}

using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.Events;

namespace AFArcade {

public class CharacterManager : MonoBehaviour {
	
	public static string UNLOCKED_IDS_KEY = "unlocked_chars";
	public static string OWNED_IDS_KEY = "owned_chars";
	public static string SELECTED_CHAR_KEY = "selected_char";
	public static string THRESHOLD_COUNTER_PREFIX = "threshold_counter";

	public static CharacterManager instance;

	public Character[] characters { get; private set; }		// ArtikFlowConfiguration 

	public class StringEvent : UnityEvent<string> {}

	public StringEvent eventCharacterUnlocked = new StringEvent();
	public StringEvent eventCharacterPurchased = new StringEvent();

	private Dictionary<string, Character> _characters;
	private HashSet<string> _unlocked_characters_ids;
	private HashSet<string> _owned_characters_ids;
	private string _selected_character_id;

	void Awake(){
		if( instance == null ){
			instance = this;
			_characters = new Dictionary<string, Character>();
			_unlocked_characters_ids = new HashSet<string>();
			_owned_characters_ids = new HashSet<string>();
			_selected_character_id = "Default"; // TODO: Take this id from configuration.
		}
	}

	void Start(){
		characters = ArtikFlowArcade.instance.configuration.characters;
		
		initializeCharacters();

		portOldCharactersData();

		foreach(Character character in characters){
			_characters.Add(character.internalName, character);
		}

		foreach(string unlocked_id in SaveGameSystem.instance.loadStringFromTag(UNLOCKED_IDS_KEY).Split('*')){
			_unlocked_characters_ids.Add(unlocked_id);
		}

		foreach(string owned_id in SaveGameSystem.instance.loadStringFromTag(OWNED_IDS_KEY).Split('*')){
			_owned_characters_ids.Add(owned_id);
		}

		_selected_character_id = SaveGameSystem.instance.loadStringFromTag(SELECTED_CHAR_KEY);
		if( _selected_character_id == "" ){
			_selected_character_id = "Default";
		}
	}

	void portOldCharactersData(){
		// This *MUST* be called after initializeCharacters()
		if( ES2.Exists("save.artik?tag=chars") ){
			string[] old_chars_indexes = SaveGameSystem.instance.loadStringFromTag("chars").Split('*');
			foreach(string index_str in old_chars_indexes){
				int index = int.Parse(index_str);
				_owned_characters_ids.Add(characters[index].internalName);
			}

			string[] ids_to_save = new string[_owned_characters_ids.Count];
			_owned_characters_ids.CopyTo(ids_to_save);
			string ids_string = string.Join("*", ids_to_save);
			SaveGameSystem.instance.saveStringToTag(ids_string, OWNED_IDS_KEY);			
		
			ES2.Delete("save.artik?tag=chars");
		}
		if( ES2.Exists("save.artik?tag=selectedChar") ){
			int old_selected_char = ES2.Load<int>("save.artik?tag=selectedChar");
			setSelectedCharacter(characters[old_selected_char]);
			
			ES2.Delete("save.artik?tag=selectedChar");
		}
	}

	void initializeCharacters(){
		DateTime now = DateTime.Now;
		Character holidayChar = null;

		for(int i = 0; i < characters.Length; i++){
			Character a_character = characters[i];
			a_character.id = i;

			if( ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM && a_character.price < 0 ){
				a_character.price = 100;      // hardcoded :(
			}

			// Check for holiday character
			if( a_character.promoDaysLength < 1 || a_character.promoDaysLength > 366 || a_character.promoStartMonth < 1 || a_character.promoStartMonth > 12 || a_character.promoStartDay < 1 || a_character.promoStartDay > 31 ){
				continue;
			}

			DateTime start = new DateTime(now.Year, a_character.promoStartMonth, a_character.promoStartDay);
			DateTime end = start + new TimeSpan(a_character.promoDaysLength, 0, 0, 0);

			if( now >= start && now <= end ){
				Debug.Log("[ArtikFlow] Holiday character! " + i + "." + a_character.internalName);
				purchaseCharacter(a_character);
				holidayChar = a_character;
			} else {	// Try previous year, should fix errors when checking for new year holidays

				start = start.AddYears(-1);
				end = end.AddYears(-1);

				if( now >= start && now <= end ){
					Debug.Log("[ArtikFlow] Holiday character! " + i + "." + a_character.internalName);
					purchaseCharacter(a_character);
					holidayChar = a_character;
				}
			}
		}

		if( holidayChar != null ){
			ArtikFlowArcade.instance.setCharacter(holidayChar);
		}
	}

	public Character getCharacter(int id){
		if( id < 0 ){
			id = 0;
		}
		return characters[id];
	}

	public Character getCharacter(string internal_name){
		Character result = _characters[internal_name];
		return result;
	}

	/// Returns if the player can buy a character with the money she has
	public bool canBuyACharacter(){
		foreach(Character c in characters){
			if( isUnlocked(c) && !isOwned(c) && c.price > 0 && SaveGameSystem.instance.getCoins() >= c.price ){
				return true;
			}
		}
		return false;
	}

	public void unlockCharacter(Character c){
		_unlocked_characters_ids.Add(c.internalName);

		string[] ids_to_save = new string[_unlocked_characters_ids.Count];
		_unlocked_characters_ids.CopyTo(ids_to_save);
		string ids_string = string.Join("*", ids_to_save);
		SaveGameSystem.instance.saveStringToTag(ids_string, UNLOCKED_IDS_KEY);
		
		eventCharacterUnlocked.Invoke(c.internalName);
	}

	public List<Character> getUnlockedCharacters(){
		List<Character> result = new List<Character>();
		Character to_be_added;
		foreach(string key in _unlocked_characters_ids){
			if( _characters.TryGetValue(key, out to_be_added) ){
				result.Add(to_be_added);
			} else {
				Debug.Log("==== Tried to return an unlocked skin but is not available. Skin key: " + key);
			}
		}
		return result;
	}

	public void purchaseCharacter(Character c){
		_owned_characters_ids.Add(c.internalName);
		
		string[] ids_to_save = new string[_owned_characters_ids.Count];
		_owned_characters_ids.CopyTo(ids_to_save);
		string ids_string = string.Join("*", ids_to_save);
		SaveGameSystem.instance.saveStringToTag(ids_string, OWNED_IDS_KEY);
		
		eventCharacterPurchased.Invoke(c.internalName);
	}

	public List<Character> getOwnedCharacters(){
		List<Character> result = new List<Character>();
		Character to_be_added;
		foreach(string key in _owned_characters_ids){
			if( _characters.TryGetValue(key, out to_be_added) ){
				result.Add(to_be_added);
			} else {
				Debug.Log("==== Tried to return an owned skin but is not available. Skin key: " + key);
			}
		}
		return result;
	}

	public bool isUnlocked(Character c){
		bool result = _unlocked_characters_ids.Contains(c.internalName) || isOwned(c);
		return result;
	}

	public bool isOwned(Character c){
		bool result = _owned_characters_ids.Contains(c.internalName);
		return result;
	}

	public bool hasAllCharacters(){
		bool result = true;
		foreach(string key in _characters.Keys){
			if( !_owned_characters_ids.Contains(key) ){
				result = false;
				break;
			}
		}
		return result;
	}

	public void setSelectedCharacter(Character c){
		_selected_character_id = c.internalName;
		SaveGameSystem.instance.saveStringToTag(_selected_character_id, SELECTED_CHAR_KEY);
	}

	public Character getSelectedCharacter(){
		Character result = _characters[_selected_character_id];
		return result;
	}

	public void increaseThresholdCounter(Character c){
		// Increases the counter that takes acount of the steps to reach the character unlock threshold
		int counter = 0;
		if( ES2.Exists("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName) ){
			counter = ES2.Load<int>("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
		}
		counter++;
		ES2.Save<int>(counter, "save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);

		if( counter >= c.unlockThreshold ){
			unlockCharacter(c);
		}
	}

	public void decreaseThresholdCounter(Character c){
		if( ES2.Exists("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName) ){
			int counter = ES2.Load<int>("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
			counter--;
			ES2.Save<int>(counter, "save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
		}
	}

	public void resetThresholdCounter(Character c){
		ES2.Save<int>(0, "save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
	}

	public int getThresholdCounter(Character c){
		int counter = 0;
		if( ES2.Exists("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName) ){
			counter = ES2.Load<int>("save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
		} else {
			ES2.Save<int>(counter, "save.artik?tag=" + THRESHOLD_COUNTER_PREFIX + c.internalName);
		}
		return counter;
	}
}

}
using UnityEngine;
using System.Collections;
using SmartLocalization;
using AFBase;

public class Language : MonoBehaviour
{
	private UIFont[] referenceFonts;	// ArtikFlowConfiguration
	private UIFont[] bitmapFonts;		// ArtikFlowConfiguration
	private UIFont[] dynamicFonts;		// ArtikFlowConfiguration

	private string forceLanguage;       // ArtikFlowConfiguration

	static INIParser artikflowIni = new INIParser();
	string languageCode;

	void Awake ()
	{
		// Make sure ArtikFlow's Awake executes before this:
		referenceFonts = ArtikFlowBase.instance.configuration.referenceFonts;
		bitmapFonts = ArtikFlowBase.instance.configuration.bitmapFonts;
		dynamicFonts = ArtikFlowBase.instance.configuration.dynamicFonts;
		forceLanguage = ArtikFlowBase.instance.configuration.forceLanguage;

		if (forceLanguage != null && forceLanguage.Length >= 1)
			setLanguage(forceLanguage);
		else
		{
			SmartCultureInfo systemLanguage = LanguageManager.Instance.GetDeviceCultureIfSupported();
			string lang = PlayerPrefs.GetString("selectedLanguage");
			if (lang == "")
			{
				if (systemLanguage != null)
					setLanguage(systemLanguage.languageCode);
				else
					setLanguage("en");
			}
			else
			{
				setLanguage(lang);
			}
		}

	}

	void Start()
	{
		if (languageCode != null)
			LanguageManager.Instance.ChangeLanguage(languageCode);		// Fixes a bug in Smart Localization
	}
	
	public void setLanguage(string code)
	{
		artikflowIni.Close();
        artikflowIni.Open(Resources.Load("ArtikFlowLocalization/" + code) as TextAsset);
		LanguageManager.Instance.ChangeLanguage(code);
		
		PlayerPrefs.SetString("selectedLanguage", code);
		languageCode = code;

		if (code == "ar" || code == "zh-CHT" || code == "zh-CHS" || code == "hi" || code == "ja" || code == "ko" || code == "ru")
		{
			for (int i = 0; i < referenceFonts.Length; i++)
				referenceFonts[i].replacement = dynamicFonts[i];
		}
		else
		{
			for (int i = 0; i < referenceFonts.Length; i++)
				referenceFonts[i].replacement = bitmapFonts[i];
		}

		print("LANGUAGE: Set to " + code);

	}
	
	/// <summary>
	/// Helper method, returns the string with caps on.
	/// </summary>
	/// <returns></returns>
	public static string get(string identifier, bool caps=true)
	{
		string str = null;
		if(artikflowIni.IsKeyExists("ArtikFlow", identifier))
			str = artikflowIni.ReadValue("ArtikFlow", identifier, "");
		else
			str = LanguageManager.Instance.GetTextValue(identifier);

		if (str == null)
			return "";

		if (caps)
			return str.ToUpper();
		else
			return str;
    }

}

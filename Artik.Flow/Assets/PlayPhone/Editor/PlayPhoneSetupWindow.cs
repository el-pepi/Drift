using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using PlayPhone;

public class PlayPhoneSetupWindow : EditorWindow
{
	#region Utils
	private static string FullPathTo(string asset)
	{
		return Path.GetFullPath(Path.Combine(Application.dataPath, asset));
	}
	
	private void ShowMessages(List<string> messages, MessageType type)
	{
		foreach (var m in messages)
		{
			EditorGUILayout.HelpBox(m, type, true);
		}
	}
	#endregion
	
	#region Fields
	abstract class Field
	{
		public string label = "";
		public string metaDataName = "";
		public bool defaulted;
		
		public abstract void OnGUI();
		public abstract void SetRequirement(AndroidManifest.Requirements requirements);
		public abstract void Refresh(AndroidManifest manifest);
	}
	
	class StringField : Field
	{
		public string defaultValue = "";
		public string userInput = "";
		public string metaDataValue = "";
		
		public override void OnGUI()
		{
			userInput = EditorGUILayout.TextField(label + ":", userInput);
		}
		
		public override void SetRequirement(AndroidManifest.Requirements requirements)
		{
			if (requirements.metaDatas.ContainsKey(metaDataName))
				requirements.metaDatas[metaDataName] = userInput;
			else
				requirements.metaDatas.Add (metaDataName, userInput);
		}
		
		public override void Refresh(AndroidManifest manifest)
		{
			if (!manifest.HasMetaData(metaDataName))
			{
				if (!defaulted)
				{
					defaulted = true;
					userInput = defaultValue;
				}
				return;
			}
			var value = manifest.GetMetaData(metaDataName);
			if (userInput == metaDataValue)
				userInput = value;
			metaDataValue = value;
		}
	}
	
	class BooleanField : Field
	{
		public string trueValue = "true";
		public string falseValue = "false";
		
		public bool defaultValue;
		public bool userInput;
		public bool metaDataValue;
		
		public override void OnGUI()
		{
			userInput = EditorGUILayout.Toggle(label, userInput);
		}
		
		public override void SetRequirement(AndroidManifest.Requirements requirements)
		{
			var value = userInput ? trueValue : falseValue;
			if (requirements.metaDatas.ContainsKey(metaDataName))
				requirements.metaDatas[metaDataName] = value;
			else
				requirements.metaDatas.Add(metaDataName, value);
		}
		
		public override void Refresh(AndroidManifest manifest)
		{
			if (!manifest.HasMetaData(metaDataName))
			{
				if (!defaulted)
				{
					defaulted = true;
					userInput = defaultValue;
				}
				return;
			}
			var value = manifest.GetMetaData(metaDataName);
			var boolValue = (value == trueValue);
			if (userInput == metaDataValue)
				userInput = boolValue;
			metaDataValue = boolValue;
		}
	}

	class GravityEnumField : Field
	{
		public Gravity defaultValue;
		public Gravity userInput;
		public string metaDataValue = "";

		public override void OnGUI()
		{
			userInput = (Gravity)EditorGUILayout.EnumMaskField(label + ":", userInput);
		}

		public override void SetRequirement(AndroidManifest.Requirements requirements)
		{
			if (requirements.metaDatas.ContainsKey(metaDataName))
				requirements.metaDatas[metaDataName] = GravityConverter.ToInt(userInput).ToString();
			else
				requirements.metaDatas.Add (metaDataName, GravityConverter.ToInt(userInput).ToString());
		}

		public override void Refresh(AndroidManifest manifest)
		{
			if (!manifest.HasMetaData(metaDataName))
			{
				if (!defaulted)
				{
					defaulted = true;
					userInput = defaultValue;
				}
				return;
			}
			var value = manifest.GetMetaData(metaDataName);
			if (((int)userInput == 0) && string.IsNullOrEmpty(metaDataValue))
			{
				userInput = (Gravity)GravityConverter.FromInt(int.Parse(value));
			}
			metaDataValue = value;
		}
	}
	#endregion
	
	#region Lifecycle
	[MenuItem("PlayPhone/Setup")]
	static void ShowWindow()
	{
		((PlayPhoneSetupWindow)EditorWindow.GetWindow(typeof(PlayPhoneSetupWindow))).Refresh(true);
	}
	
	private void Refresh(bool force = false)
	{
		if (EditorApplication.timeSinceStartup < lastRefreshTime + refreshInterval && !force)
			return;
		lastRefreshTime = (float)EditorApplication.timeSinceStartup;
		
		CheckManifest(false);
	}
	
	private float lastRefreshTime = 0.0f;
	private const float refreshInterval = 1.0f;
	#endregion
	
	#region Manifest Setup
	private AndroidManifest.Requirements manifestRequirements = new AndroidManifest.Requirements();
	private bool manifestExists = false;
	private List<string> manifestErrors = new List<string>();
	private List<string> manifestWarnings = new List<string>();
	
	private void CheckManifest(bool fix)
	{
		if (fix)
		{
			foreach (var f in allFields)
			{
				f.SetRequirement(manifestRequirements);
			}
		}
		
		var manifest = new AndroidManifest(manifestRequirements);
		manifest.Check(manifestErrors, manifestWarnings, fix);
		manifestExists = manifest.Exists;
		
		foreach (var f in allFields)
		{
			f.Refresh(manifest);
		}
		
		Summarize();
	}
	
	private List<Field> allFields;
	#endregion
	
	private StringField secretKey = new StringField 
	{
		label = "Secret Key",
		metaDataName = "com.playphone.psgn.SECRET_KEY",
		defaultValue = "",
	};
	
	private GravityEnumField iconGravity = new GravityEnumField
	{
		label = "Icon Gravity",
		metaDataName = "com.playphone.psgn.GRAVITY",
		defaultValue = Gravity.Top | Gravity.Right, 
	};
	
	private BooleanField debug = new BooleanField 
	{ 
		label = "Debug", 
		metaDataName = "com.playphone.psgn.DEBUG", 
		defaultValue = true,
		trueValue = "1",
		falseValue = "0",
	};

	private StringField marketPubKey = new StringField 
	{ 
		label = "Public License Key", 
		metaDataName = "com.playphone.psgn.MARKET_PUB_KEY", 
		defaultValue = "",
	};
	
	public PlayPhoneSetupWindow()
	{
		allFields = new List<Field>() { secretKey, iconGravity, marketPubKey, debug};
		
		manifestRequirements.manifestFilename = FullPathTo("Plugins/Android/AndroidManifest.xml");
		manifestRequirements.defaultManifestFilename = FullPathTo("PlayPhone/Editor/DefaultAndroidManifest.xml");
		manifestRequirements.targetSdkVersion = "14";
		
		manifestRequirements.metaDataConstraints.Add(secretKey.metaDataName, new AndroidManifest.MetaDataRequirements
		{
			notEmpty = true,
		});
		manifestRequirements.metaDataConstraints.Add(iconGravity.metaDataName, new AndroidManifest.MetaDataRequirements
		{
			notEmpty = true,
			isInteger = true,
		});
		manifestRequirements.metaDataConstraints.Add(debug.metaDataName, new AndroidManifest.MetaDataRequirements
		{
			possibleValues = new HashSet<string> { "0", "1" },
		});
		manifestRequirements.metaDataConstraints.Add(marketPubKey.metaDataName, new AndroidManifest.MetaDataRequirements
		{
		});


		
		manifestRequirements.usesPermissions.Add("android.permission.START_BACKGROUND_SERVICE", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.GET_ACCOUNTS", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.WAKE_LOCK", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.INTERNET", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.ACCESS_NETWORK_STATE", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.READ_PHONE_STATE", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("com.android.vending.BILLING", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.GET_TASKS", AndroidManifest.Requirement.Any);
	    manifestRequirements.usesPermissions.Add("android.permission.WRITE_EXTERNAL_STORAGE", AndroidManifest.Requirement.Any);
		
		manifestRequirements.contexts.Add("com.playphone.psgn.vzw.BillingActivity", new AndroidManifest.ContextRequirements
		{
			type = "activity",
			label = "PlayPhone Unity Example",
		});
		
		manifestRequirements.contexts.Add("com.playphone.psgn.PSGNService", new AndroidManifest.ContextRequirements
		{
			type = "service",
			process = ":remote",
			actions = new List<string> { "com.playphone.psgn.PSGNService" },
		});
		
		manifestRequirements.contexts.Add("com.playphone.psgn.PSGNProxyActivity", new AndroidManifest.ContextRequirements
		{
			type = "activity",
			theme = "@android:style/Theme.NoTitleBar",
			configChanges = new List<string> { "orientation", "keyboardHidden" },
			actions = new List<string> { "android.intent.action.VIEW" },
			categories = new List<string>
			{ 
				"android.intent.category.DEFAULT",
				"android.intent.category.BROWSABLE",
			},
		});
		
		manifestRequirements.contexts.Add("com.playphone.psgn.android.BillingService", new AndroidManifest.ContextRequirements
		{
			type = "service",
		});
		
		manifestRequirements.contexts.Add("com.playphone.psgn.android.BillingReceiver", new AndroidManifest.ContextRequirements
		{
			type = "receiver",
			actions = new List<string>
			{
				"com.android.vending.billing.RESPONSE_CODE",
				"com.android.vending.billing.IN_APP_NOTIFY",
				"com.android.vending.billing.PURCHASE_STATE_CHANGED",
			},
		});
        
		manifestRequirements.contexts.Add("com.playphone.psgn.InstallReferrerReceiver", new AndroidManifest.ContextRequirements
		{
			type = "receiver",
			exported = "true",
			actions = new List<string> { "com.android.vending.INSTALL_REFERRER" },
		});
	}
	
	private string summary = "";
	
	private void Summarize()
	{
		if (!manifestExists)
		{
			summary = "Not configured, manifest doesn't exist";
		}
		else
		{
			var mode = (debug.metaDataValue ? "debug" : "release");
			var secret = (secretKey.metaDataValue.Length == 0) ? 
				"without a secret key" :
				("with a secret key " + secretKey.metaDataValue);
			var errors = (manifestErrors.Count > 0) ? 
				"contains some errors, check the messages below, and use update button to resolve them" : 
				"contains no errors";
			summary = "Manifest is configured for " + mode + " " + secret + " and " + errors + ".";

		}
	}
	
	void OnGUI()
	{
		Refresh();
		
		EditorGUILayout.HelpBox(summary, MessageType.Info, true);
		
		secretKey.OnGUI();
		iconGravity.OnGUI();
		marketPubKey.OnGUI();
		debug.OnGUI();
		
		if (GUILayout.Button("Update AndroidManifest.xml"))
		{
			GUIUtility.keyboardControl = 0;
			CheckManifest(true);
		}
		
		ShowMessages(manifestErrors, MessageType.Error);
		ShowMessages(manifestWarnings, MessageType.Warning);
	}
}

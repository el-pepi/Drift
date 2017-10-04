using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace PlayPhone
{
	public class AndroidManifest
	{
		public enum Requirement
		{
			None,
			Required,
			NotRequired,
			Any,
		}
		
		public enum ProtectionLevel
		{
			None,
			Signature,
			Any,
		}
		
		private static bool Forced(AndroidManifest.Requirement r)
        {
            return r != AndroidManifest.Requirement.Any;
        }
		
		private static string Value(AndroidManifest.Requirement r)
        {
            if (r == AndroidManifest.Requirement.NotRequired)
				return "false";
			else if (r == AndroidManifest.Requirement.Required)
				return "true";
			return null;
        }
		
		private static bool Forced(AndroidManifest.ProtectionLevel pl)
        {
            return pl != AndroidManifest.ProtectionLevel.Any;
        }
		
		private static string Value(AndroidManifest.ProtectionLevel r)
        {
            if (r == AndroidManifest.ProtectionLevel.Signature)
				return "signature";
			return null;
        }
		
		public class ContextRequirements
		{
			public string type;
			public string label;
			public string permission;
			public string process;
			public string theme;
			public string exported;
			public List<string> configChanges = new List<string>();
			public List<string> actions = new List<string>();
			public List<string> categories = new List<string>();
		}
		
		public class MetaDataRequirements
		{
			public HashSet<string> possibleValues;
			public bool notEmpty;
			public bool isInteger;
		}
		
		public class Requirements
		{
			public string defaultManifestFilename;
			public string manifestFilename;
			public string targetSdkVersion;
			public Dictionary<string, MetaDataRequirements> metaDataConstraints = new Dictionary<string, MetaDataRequirements>();
			public Dictionary<string, string> metaDatas = new Dictionary<string, string>();
			public Dictionary<string, ProtectionLevel> permissions = new Dictionary<string, ProtectionLevel>();
			public Dictionary<string, Requirement> usesPermissions = new Dictionary<string, Requirement>();
			public Dictionary<string, Requirement> usesFeatures = new Dictionary<string, Requirement>();
			public Dictionary<string, ContextRequirements> contexts = new Dictionary<string, ContextRequirements>();
		}
		
		public AndroidManifest(Requirements r)
		{
			requirements = r;
		}
		
		private Requirements requirements;
		private XmlDocument doc;
		
		private const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
	
		private const string AndroidNameAttribute = "android:name";
		private const string AndroidRequiredAttribute = "android:required";
		private const string AndroidValueAttribute = "android:value";
		private const string AndroidLabelAttribute = "android:label";
		private const string AndroidPermissionAttribute = "android:permission";
		private const string AndroidConfigChangesAttribute = "android:configChanges";
		private const string AndroidProtectionLevelAttribute = "android:protectionLevel";
		private const string AndroidThemeAttribute = "android:theme";
		private const string AndroidProcessAttribute = "android:process";
		private const string AndroidExportedAttribute = "android:exported";
		private const string AndroidTargetSdkVersionAttribute = "android:targetSdkVersion";
		
		#region Attributes
		private string Scheme(string name)
		{
			return name.Split(':')[0];
		}
		
		private string Name(string name)
		{
			return name.Split(':')[1];
		}
		
		private string GetNodeAttribute(XmlNode node, string name)
		{
			var nodeAttribute = node.Attributes.GetNamedItem(name);
			return (nodeAttribute == null) ? null : nodeAttribute.Value;
		}
		
		private void SetNodeAttribute(XmlNode node, string name, string value)
		{
			var attribute = (XmlAttribute)node.Attributes.GetNamedItem(name);
			if (value == null)
			{
				if (attribute != null)
				{
					node.Attributes.RemoveNamedItem(name);
				}
			}
			else
			{
				if (attribute != null)
				{
					attribute.Value = value;
				}
				else
				{
					attribute = (XmlAttribute)doc.CreateNode(XmlNodeType.Attribute, Scheme(name), Name(name), AndroidXmlNamespace);
					attribute.Value = value;
					node.Attributes.Append(attribute);
				}
			}
		}
		#endregion
		
		#region Permission
		private void SetPermission(string name, string protectionLevel, bool forceProtectionLevel)
		{
			var permission = FindPermission(name) ?? CreatePermission(name);
			if (forceProtectionLevel)
				SetNodeAttribute(permission, AndroidProtectionLevelAttribute, protectionLevel);
		}
		
		private XmlNode FindPermission(string name)
		{
			var permissions = doc.SelectNodes("/manifest/permission");
			for (var i = 0; i < permissions.Count; ++i)
			{
				var permission = permissions.Item(i);
				if (GetNodeAttribute(permission, AndroidNameAttribute) == name)
					return permission;
			}
			return null;
		}
		
		private XmlNode CreatePermission(string name)
		{		
			var permission = doc.CreateNode(XmlNodeType.Element, "permission", "");
			SetNodeAttribute(permission, AndroidNameAttribute, name);
	
			var manifest = doc.SelectSingleNode("/manifest");
			if (manifest != null)
				manifest.AppendChild(permission);
			
			return permission;
		}
		#endregion
		
		#region UsesFeature
		private void SetUsesFeature(string name, string required, bool forceRequired)
		{
			var feature = FindUsesFeature(name) ?? CreateUsesFeature(name);
			if (!forceRequired)
				SetNodeAttribute(feature, AndroidRequiredAttribute, required);
		}
		
		private XmlNode FindUsesFeature(string name)
		{
			var features = doc.SelectNodes("/manifest/uses-feature");
			for (var i = 0; i < features.Count; ++i)
			{
				var feature = features.Item(i);
				if (GetNodeAttribute(feature, AndroidNameAttribute) == name)
					return feature;
			}
			return null;
		}
		
		private XmlNode CreateUsesFeature(string name)
		{
			var feature = doc.CreateNode(XmlNodeType.Element, "uses-feature", "");
			SetNodeAttribute(feature, AndroidNameAttribute, name);
			
			var manifest = doc.SelectSingleNode("/manifest");
			if (manifest != null)
				manifest.AppendChild(feature);
			
			return feature;
		}
		#endregion
		
		#region UsesPermission
		private void SetUsesPermission(string name, string required, bool forceRequired)
		{
			var permission = FindUsesPermission(name) ?? CreateUsesPermission(name);
			if (forceRequired)
				SetNodeAttribute(permission, AndroidRequiredAttribute, required);
		}
		
		private XmlNode FindUsesPermission(string name)
		{
			var permissions = doc.SelectNodes("/manifest/uses-permission");
			for (var i = 0; i < permissions.Count; ++i)
			{
				var permission = permissions.Item(i);
				if (GetNodeAttribute(permission, AndroidNameAttribute) == name)
					return permission;
			}
			return null;
		}
		
		private XmlNode CreateUsesPermission(string name)
		{
			var permission = doc.CreateNode(XmlNodeType.Element, "uses-permission", "");
			SetNodeAttribute(permission, AndroidNameAttribute, name);
	
			var manifest = doc.SelectSingleNode("/manifest");
			if (manifest != null)
				manifest.AppendChild(permission);
			
			return permission;
		}
		#endregion
		
		#region MetaData
		public bool HasMetaData(string name)
		{
			if (doc == null)
				return false;
			
			var metaDatas = doc.SelectNodes("/manifest/application/meta-data");
			for (var i = 0; i < metaDatas.Count; ++i)
			{
				var metaData = metaDatas.Item(i);
				if (GetNodeAttribute(metaData, AndroidNameAttribute) == name)
				{
					return true;
				}
			}
			return false;
		}
		
		public string GetMetaData(string name)
		{
			var metaDatas = doc.SelectNodes("/manifest/application/meta-data");
			for (var i = 0; i < metaDatas.Count; ++i)
			{
				var metaData = metaDatas.Item(i);
				if (GetNodeAttribute(metaData, AndroidNameAttribute) == name)
				{
					return GetNodeAttribute(metaData, AndroidValueAttribute) ?? "";
				}
			}
			return "";
		}
		
		public void SetMetaData(string name, string value)
		{
			var metaData = FindMetaData(name);
			if (metaData != null)
				SetNodeAttribute(metaData, AndroidValueAttribute, value);
			else
				CreateMetaData(name, value);
		}
		
		private XmlNode FindMetaData(string name)
		{
			var metaDatas = doc.SelectNodes("/manifest/application/meta-data");
			for (var i = 0; i < metaDatas.Count; ++i)
			{
				var metaData = metaDatas.Item(i);
				if (GetNodeAttribute(metaData, AndroidNameAttribute) == name)
					return metaData;
			}
			return null;
		}
		
		private XmlNode CreateMetaData(string name, string value)
		{
			var metaData = doc.CreateNode(XmlNodeType.Element, "meta-data", "");
			SetNodeAttribute(metaData, AndroidNameAttribute, name);
			SetNodeAttribute(metaData, AndroidValueAttribute, value);
			
			var app = doc.SelectSingleNode("/manifest/application");
			if (app != null)
				app.AppendChild(metaData);
			
			return metaData;
		}
		#endregion
		
		#region Contexts
		private XmlNode FindContext(string type, string name)
		{
			var contexts = doc.SelectNodes("/manifest/application/" + type);
			for (var i = 0; i < contexts.Count; ++i)
			{
				var context = contexts.Item(i);
				if (GetNodeAttribute(context, AndroidNameAttribute) == name)
					return context;
			}
			return null;
		}
		
		private XmlNode CreateContext(string type, string name)
		{
			var context = doc.CreateNode(XmlNodeType.Element, type, "");
			SetNodeAttribute(context, AndroidNameAttribute, name);
			
			var app = doc.SelectSingleNode("/manifest/application");
			if (app != null)
				app.AppendChild(context);
			
			return context;
		}
		#endregion
		
		#region Android SDK Version
		private void SetTargetSdkVersion(string sdkVersion)
		{
			var usesSdk = doc.SelectSingleNode ("/manifest/uses-sdk");
			if (usesSdk == null)
			{
				usesSdk = doc.CreateNode (XmlNodeType.Element, "uses-sdk", "");

				var manifest = doc.SelectSingleNode("/manifest");
				if (manifest != null)
					manifest.AppendChild(usesSdk);
			}
			SetNodeAttribute (usesSdk, AndroidTargetSdkVersionAttribute, requirements.targetSdkVersion);
		}
		#endregion

		private bool HasName(XmlNodeList nodes, string name)
		{
			for (var i = 0; i < nodes.Count; ++i)
			{
				var node = nodes.Item(i);
				if (GetNodeAttribute(node, AndroidNameAttribute) == name)
					return true;
			}
			return false;
		}
		
		private void Check(string name, MetaDataRequirements requirements, List<string> errors, List<string> warnings, bool fix)
		{
			if (!HasMetaData(name))
			{
				warnings.Add("meta data " + name + " is not specified");
				return;
			}
			
			var value = GetMetaData(name);
			if (requirements.possibleValues != null && !requirements.possibleValues.Contains(value))
			{
				var possibleValues = "";
				foreach(var p in requirements.possibleValues)
				{
					if (possibleValues.Length != 0)
						possibleValues += ", ";
					possibleValues += (p.Length == 0) ? "<empty>" : p;
				}
				warnings.Add("meta data " + name + " should have one of the following values:\n" + possibleValues);
				return;
			}
			if (requirements.notEmpty && string.IsNullOrEmpty(value))
			{
				warnings.Add("meta data " + name + " should not be empty");
				return;
			}
			int intValue;
			if (requirements.isInteger && !int.TryParse(value, out intValue))
			{
				warnings.Add("meta data " + name + " should be an integer value");
				return;
			}
		}
		
		private void Check(string type, string name, ContextRequirements requirements, List<string> errors, List<string> warnings, bool fix)
		{
			var context = FindContext(type, name);
			
			if (context == null)
			{
				if (fix)
				{
					context = CreateContext(type, name);
				}
				else
				{
					errors.Add(type + " " + name + " is not defined");
					return;
				}
			}
			
			var configChanges = GetNodeAttribute(context, AndroidConfigChangesAttribute) ?? "";
			var existingConfigChanges = new HashSet<string>(configChanges.Split ('|'));
			
			var actions = context.SelectNodes("intent-filter/action");
			var categories = context.SelectNodes("intent-filter/category");
			
			if (fix)
			{	
				if (!string.IsNullOrEmpty(requirements.label) &&
					string.IsNullOrEmpty(GetNodeAttribute(context, AndroidLabelAttribute)))
					SetNodeAttribute(context, AndroidLabelAttribute, requirements.label);
				
				if (!string.IsNullOrEmpty(requirements.theme) &&
					string.IsNullOrEmpty(GetNodeAttribute(context, AndroidThemeAttribute)))
					SetNodeAttribute(context, AndroidThemeAttribute, requirements.theme);
				
				if (!string.IsNullOrEmpty(requirements.permission))
					SetNodeAttribute(context, AndroidPermissionAttribute, requirements.permission);
				
				if (!string.IsNullOrEmpty(requirements.process))
					SetNodeAttribute(context, AndroidProcessAttribute, requirements.process);
				
				if (!string.IsNullOrEmpty(requirements.exported))
					SetNodeAttribute(context, AndroidExportedAttribute, requirements.exported);
				
				var configChangesUpdated = false;
				foreach (var cc in requirements.configChanges)
				{
					if (existingConfigChanges.Contains(cc))
						continue;
					configChangesUpdated = true;
					if (configChanges.Length > 0)
						configChanges += "|";
					configChanges += cc;
					existingConfigChanges.Add(cc);
				}
				
				if (configChangesUpdated)
					SetNodeAttribute(context, AndroidConfigChangesAttribute, configChanges);
				
				var intentFilter = context.SelectSingleNode("intent-filter");
				if (intentFilter == null && (requirements.actions.Count > 0 || requirements.categories.Count > 0))
				{
					intentFilter = doc.CreateNode(XmlNodeType.Element, "intent-filter", "");
					context.AppendChild(intentFilter);
				}
				
				foreach (var a in requirements.actions)
				{
					if (HasName(actions, a))
						continue;
					var action = doc.CreateNode(XmlNodeType.Element, "action", "");
					SetNodeAttribute(action, AndroidNameAttribute, a);
					intentFilter.AppendChild(action);
				}
				
				foreach (var c in requirements.categories)
				{
					if (HasName(categories, c))
						continue;
					var category = doc.CreateNode(XmlNodeType.Element, "category", "");
					SetNodeAttribute(category, AndroidNameAttribute, c);
					intentFilter.AppendChild(category);
				}

				actions = context.SelectNodes("intent-filter/action");
				categories = context.SelectNodes("intent-filter/category");
			}
			
			if (!string.IsNullOrEmpty(requirements.permission) && 
				GetNodeAttribute(context, AndroidPermissionAttribute) != requirements.permission)
				errors.Add(type + " " + name + " doesn't have required permission attribute");
			
			if (!string.IsNullOrEmpty(requirements.process) && 
				GetNodeAttribute(context, AndroidProcessAttribute) != requirements.process)
				errors.Add(type + " " + name + " doesn't have required process attribute");
			
			if (!string.IsNullOrEmpty(requirements.exported) && 
				GetNodeAttribute(context, AndroidExportedAttribute) != requirements.exported)
				errors.Add(type + " " + name + " doesn't have required exported attribute");
			
			if (!string.IsNullOrEmpty(requirements.theme) && 
				GetNodeAttribute(context, AndroidThemeAttribute) != requirements.theme)
				warnings.Add(type + " " + name + " has theme attribute different from required");
			
			foreach (var cc in requirements.configChanges)
			{
				if (!existingConfigChanges.Contains(cc))
				{
					errors.Add(type + " " + name + " doesn't have all required configChanges");
					break;
				}
			}
			
			foreach (var a in requirements.actions)
			{
				if (!HasName(actions, a))
					errors.Add(type + " " + name + " doesn't have " + a + " action");
			}
			
			foreach (var c in requirements.categories)
			{
				if (!HasName(categories, c))
					errors.Add(type + " " + name + " doesn't have " + c + " category");
			}
		}
		
		public bool Exists { get { return doc != null; } }
		
		public bool Check(List<string> errors, List<string> warnings, bool fix)
		{
			errors.Clear();
			warnings.Clear();
			doc = null;
			
			if (!File.Exists(requirements.manifestFilename))
			{
				if (fix)
				{
					File.Copy(requirements.defaultManifestFilename, requirements.manifestFilename, true);
				}
				else
				{
					errors.Add("AndroidManifest.xml file not found");
					return false;
				}
			}
			
			doc = new XmlDocument();
			doc.Load(requirements.manifestFilename);
			
			if (requirements.targetSdkVersion != null)
			{
				if (fix)
					SetTargetSdkVersion (requirements.targetSdkVersion);
			}

			foreach (var md in requirements.metaDatas)
			{
				if (fix)
					SetMetaData(md.Key, md.Value);
			}
			
			foreach (var mdc in requirements.metaDataConstraints)
			{
				Check(mdc.Key, mdc.Value, errors, warnings, fix);
			}
			
			foreach (var p in requirements.permissions)
			{
				if (fix)
					SetPermission(p.Key, Value(p.Value), Forced(p.Value));
				
				var permission = FindPermission(p.Key);
				if (permission == null)
					errors.Add("Permission " + p.Key + " is not defined");
				else if (p.Value == ProtectionLevel.Signature &&
					GetNodeAttribute(permission, AndroidProtectionLevelAttribute) != "signature")
					errors.Add("Permission " + p.Key + " has incorrect protection level");
			}
			
			foreach (var up in requirements.usesPermissions)
			{
				if (fix)
					SetUsesPermission(up.Key, Value(up.Value), Forced(up.Value));
				
				var usesPermission = FindUsesPermission(up.Key);
				if (usesPermission == null)
					errors.Add("Permission usage " + up.Key + " is not defined");
			}
			
			foreach (var uf in requirements.usesFeatures)
			{
				if (fix)
					SetUsesFeature(uf.Key, Value(uf.Value), Forced(uf.Value));
				
				var usesFeature = FindUsesPermission(uf.Key);
				if (usesFeature == null)
					errors.Add("Feature usage " + uf.Key + " is not defined");
			}
			
			foreach (var c in requirements.contexts)
			{
				Check(c.Value.type, c.Key, c.Value, errors, warnings, fix);
			}
			
			if (fix)
			{
				doc.Save(requirements.manifestFilename);
			}
			
			return errors.Count == 0;
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CX;
using System.Text;
using System;

namespace CXEditor
{

	[CustomEditor(typeof(ImageAtlas))]
	public class ImageAtlasInspector : Editor 
	{
		const string AtlasIDClassName = "_ID";

		class AtlasIDFiled
		{
			public int id;
			public string rawName;
			public string name
			{
				get
				{
					return RawNameToName(this.rawName);
				}
			}

			public AtlasIDFiled (int id, string rawName)
			{
				this.id = id;
				this.rawName = rawName;
			}

			public string Line 
			{
				get
				{
					return "public const int " + this.name + " = " + this.id + ";";
				}
			}

			public static string RawNameToName (string rawName)
			{
				// 不允许有路径符号
				// 不允许有空格
				// 不允许有数字开头

				string name = rawName.Replace("\\", "/").Replace("/", "_").Replace(" ", "_").Replace("-", "_");

				while(name.Contains("__"))
				{
					name = name.Replace("__", "_");
				}

				// 将a_H 变为aH
				if (name.Contains("_"))
				{
					int len = name.Length;
					List<char> charList = new List<char>(len);
					for(int i = 0; i < len; ++i)
					{
						if (i > 0 && i + 1 < len && name[i] == '_' && char.IsLower(name[i - 1]) && char.IsUpper(name[i + 1]))
						{
							charList.Add(char.ToUpper(name[i + 1]));
							++i;
						}
						else
						{
							charList.Add(name[i]);
						}
					}

					name = new string(charList.ToArray());
				}

				if (string.IsNullOrEmpty(name))
				{
					throw new Exception("name 不能为空");
				}

				if (char.IsNumber(name[0]))
				{
					name = "_" + name;
				}

				return name;
			}

			/// public const int imageName = number; 
			/// 解析成功返回实例, 否则返回null
			public static AtlasIDFiled ParseLine (string line)
			{
				if (ClassParser.SingleLineIsNote(line))
				{
					return null;
				}

				string[] keyWords = ClassParser.SingleLineToKeyWords(line);

				if (keyWords == null || keyWords.Length != 6)
				{
					return null;
				}

				if (keyWords[0] != "public" || keyWords[1] != "const" || keyWords[2] != "int" || keyWords[4] != "=")
				{
					return null;
				}

				int id = 0;
				if (!int.TryParse(keyWords[5], out id))
				{
					return null;
				}

				return new AtlasIDFiled(id, keyWords[3]);
			}

		}
		class AtlasIDClass
		{
			public string filePath;
			public string className;
			public List<AtlasIDFiled> fields = new List<AtlasIDFiled>();

			const string note = 
				"/// \n" +
				"/// =================================================================\n" +
				"/// \n" +
				"/// readme: 此类完全自动生成, 请不要轻易修改此类, 若要修改, 请仔细阅读如下信息:\n" +
				"/// 为神马要使用此类: 因为此类的ID是基于数组ID, 也就是程序里最快的查找方式\n" +
				"/// 使用方法: 可以在脚本中引用此类的字段, 用于高速访问ImageInfo\n" +
				"/// 注意事项: 请不要保存此类字段的值, 因为此值会在Atlas打包时刷新\n" +
				"/// \n" +
				"/// \n" +
				"/// =================================================================\n" +
				"/// \n" +
				"/// 此类生成时: CS文件位于Atlas同级目录\n" +
				"/// 此类生成时: 类名为Atlas名称+" + _ImageInfoID + "\n" +
				"/// \n" +
				"/// \n" +
				"/// =================================================================\n" +
				"/// \n" +
				"/// [新旧Atlas更新时, image需要变更名称时的处理]\n" +
				"/// \t请同时将该类相应的字段与相应的ImageInfo对象的名称改为新名称\n" +
				"/// \n" +
				"/// \n" +
				"/// =================================================================\n" +
				"/// \n";

			public string classTextContent
			{
				get
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(note);sb.Append('\n');
					sb.Append("public class ");sb.Append(this.className);sb.Append('\n');
					sb.Append('{');sb.Append('\n');

					foreach(AtlasIDFiled field in this.fields)
					{
						sb.Append('\t');sb.Append(field.Line);sb.Append('\n');
					}
					sb.Append('}');

					return sb.ToString();
				}
			}

			public static AtlasIDClass ParseFormFilePath (string filePath)
			{
				if (!File.Exists(filePath))
				{
					return null;
				}

				AtlasIDClass c = new AtlasIDClass();
				c.filePath = filePath;
				c.ParseLines(File.ReadAllLines(filePath));

				return c;
			}

			private void ParseLines (string[] lines)
			{
				foreach(string line in lines)
				{
					if (!ClassParser.SingleLineIsNote(line))
					{
						// class line
						if (line.Contains(" class "))
						{
							string[] keyWords = ClassParser.SingleLineToKeyWords(line);
							for (int i = 0, count = keyWords.Length; i < count; ++i)
							{
								// public class className
								if (keyWords[i] == "class")
								{
									if (i + 1 < count)
									{
										this.className = keyWords[i + 1];
										break;
									}
								}
							}
						}
						// filed line
						else if (line.Contains (" int "))
						{
							AtlasIDFiled filed = AtlasIDFiled.ParseLine(line);
							if (filed != null)
							{
								this.fields.Add(filed);
							}
						}
					}
				}
			}
				
			public static AtlasIDClass NewFromAtlas (ImageAtlas atlas)
			{
				AtlasIDClass c = new AtlasIDClass();

				string atlasPath = AssetDatabase.GetAssetPath(atlas.GetInstanceID());
				c.filePath = atlasPath.Substring(0, atlasPath.Length - ".prefab".Length) + _ImageInfoID + ".cs";
				c.className = atlas.name + _ImageInfoID;

				ImageInfo[] imageInfos = atlas.ImageInfoArray;
				for(int i = 0, count = imageInfos.Length; i < count; ++i)
				{
					c.fields.Add(new AtlasIDFiled(i, imageInfos[i].name));
				}

				return c;
			}

			public static void SyncIDClass (ImageAtlas atlas)
			{
				AtlasIDClass idClass = NewFromAtlas(atlas);
				idClass.WriteToFile();
			}

			public static bool FieldsSame (AtlasIDClass c1, AtlasIDClass c2)
			{
				if (c1.fields.Count != c2.fields.Count)
				{
					return false;
				}

				foreach(AtlasIDFiled c1f in c1.fields)
				{
					bool found = false;
					foreach(AtlasIDFiled c2f in c2.fields)
					{
						if (c2f.name == c1f.name)
						{
							if (c2f.id == c1f.id)
							{
								found = true;
								break;
							}
							else
							{
								return false;
							}
						}
					}

					if (!found)
					{
						return false;
					}
				}

				return true;
			}

			public void ApplyNewClass (AtlasIDClass classNew)
			{
				List<AtlasIDFiled> tmpList = new List<AtlasIDFiled>();

				foreach(AtlasIDFiled oldField in this.fields)
				{
					bool found = false;
					foreach(AtlasIDFiled newField in classNew.fields)
					{
						if (oldField.name == newField.name)
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						oldField.id = -1;
						tmpList.Add(oldField);
					}
				}

				classNew.fields.AddRange(tmpList);
				this.fields = classNew.fields;
			}

			public void WriteToFile ()
			{
				this.fields.Sort(delegate(AtlasIDFiled x, AtlasIDFiled y) {return x.name.CompareTo(y.name);});
				File.WriteAllText(this.filePath, this.classTextContent, Encoding.Unicode);
			}
		}

		private static string GetOldCSAssetPath (ImageAtlas _this)
		{
			string atlasPath = AssetDatabase.GetAssetPath(_this.GetInstanceID());
			// ...Atlas.prefab
			string oldCSPath = atlasPath.Substring(0, atlasPath.Length - ".prefab".Length) + _ImageInfoID + ".cs";

			TextAsset t = AssetDatabase.LoadAssetAtPath<TextAsset>(oldCSPath);
			return t == null? "" : oldCSPath;
		}

		const string _ImageInfoID = "_ImageInfoID";

		private AtlasIDClass GetOldIDClass()
		{
			AtlasIDClass oldClass = null;

			string oldCSAssetPath = GetOldCSAssetPath(_this);
			if (!string.IsNullOrEmpty(oldCSAssetPath))
			{
				oldClass = AtlasIDClass.ParseFormFilePath(CXEditor.Tools.AssetsParentPath + oldCSAssetPath);
			}

			return oldClass;
		}



		private static void UpdateAtlasIDFile (ImageAtlas _this)
		{
			AtlasIDClass oldClass = null;

			string oldCSAssetPath = GetOldCSAssetPath(_this);
			if (!string.IsNullOrEmpty(oldCSAssetPath))
			{
				oldClass = AtlasIDClass.ParseFormFilePath(CXEditor.Tools.AssetsParentPath + oldCSAssetPath);
			}

			AtlasIDClass newClass = AtlasIDClass.NewFromAtlas(_this);

			if (oldClass != null)
			{
				if (!AtlasIDClass.FieldsSame(oldClass, newClass))
				{
					oldClass.ApplyNewClass(newClass);
					newClass = oldClass;
				}
				else
				{
					Debug.Log(_this.name + "的AtlasIDClass 新旧相同, 无需更新");
					return;
				}
			}

			newClass.WriteToFile();

			AssetDatabase.Refresh();
		}

		private List<ImageAtlas> FindAllAtlas ()
		{
			const string prefabSuffix = ".prefab";
			List<ImageAtlas> atlasList = new List<ImageAtlas>();

			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

			foreach(string path in allAssetPaths)
			{
				if (path.EndsWith(prefabSuffix, System.StringComparison.OrdinalIgnoreCase))
				{
					ImageAtlas atlas = AssetDatabase.LoadAssetAtPath<ImageAtlas>(path);
					if (atlas != null && !atlasList.Contains(atlas))
					{
						atlasList.Add(atlas);
					}
				}
			}

			atlasList.Sort(delegate(ImageAtlas o1, ImageAtlas o2) { return o1.name.CompareTo(o2.name); });
			return atlasList;
		}

		private ImageAtlas _this;
		private CXInspectorLayoutColorFrame m_ImageInfoArrayFrame = new CXInspectorLayoutColorFrame("ImageInfo Array");
		private bool m_DataIsFull {get { return string.IsNullOrEmpty(m_DataNotFullMessage);} }
		private string m_DataNotFullMessage;

		//private string m_PrefabPath;
		//private string m_PrefabFolder;

		//private string m_ImageAtlasWorkspacePath = "";
		//private string m_ProjectionName;
		//private string m_ProjectPath;


		void Awake ()
		{
			_this = this.target as ImageAtlas;
			m_ImageInfoArrayFrame.NoSwitch = true;
			m_DataNotFullMessage = this.CheckDataIsFull();

			//m_PrefabPath = AssetDatabase.GetAssetPath(_this.GetInstanceID());
			//m_PrefabFolder = System.IO.Path.GetDirectoryName(m_PrefabPath);

			//m_ImageAtlasWorkspacePath = CXEditor.Tools.AssetsParentPath + "/ImageAtlasWorkspace";
			//m_ProjectionName = System.IO.Path.GetFileName(m_PrefabFolder);
			//m_ProjectPath = m_ImageAtlasWorkspacePath + "/Projections/" + m_ProjectionName;
		}

		void CheckMissingImageInfo ()
		{
			ImageInfo[] imageInfos = _this.ImageInfoArray;
			if (imageInfos != null)
			{
				bool hasMissing = false;
				foreach(ImageInfo imageInfo in imageInfos)
				{
					if (imageInfo == null)
					{
						hasMissing = true;
						break;
					}
				}

				if (hasMissing)
				{
					List<ImageInfo> imageInfoList = new List<ImageInfo>();
					foreach(ImageInfo imageInfo in imageInfos)
					{
						if (imageInfo != null)
						{
							imageInfoList.AddRange(imageInfoList);
						}
					}

					EasyReflect.SetField(_this, "m_ImageInfos", imageInfoList.ToArray());
				}
			}
		}

		/// 检查数据完整性 
		string CheckDataIsFull ()
		{
			ImageInfo[] imageInfos = _this.ImageInfoArray;
			AtlasIDClass oldIDClass = this.GetOldIDClass();

			if (oldIDClass == null)
			{
				return "丢失 ImageInfo ID Class";
			}

			StringBuilder sb = new StringBuilder();

			if (oldIDClass != null)
			{
				if (oldIDClass.className != (_this.name + _ImageInfoID))
				{
					sb.AppendLine(" The class name is not matching : atlas.name : " + _this.name + ", IDClass name : " + oldIDClass.className);
				}
			}

			int count_Infos = imageInfos == null? 0 : imageInfos.Length;
			int count_fields = oldIDClass == null? 0 : oldIDClass.fields.Count;

			List<ImageInfo> diffImageInfoList = new List<ImageInfo>();
			List<AtlasIDFiled> diffAtlasIDFiledList = new List<AtlasIDFiled>();

			if (count_Infos > 0)
			{
				foreach(ImageInfo imageInfo in imageInfos)
				{
					bool noFound = true;
					if (count_fields > 0)
					{
						foreach(AtlasIDFiled field in oldIDClass.fields)
						{
							if (field.name == AtlasIDFiled.RawNameToName(imageInfo.name))
							{
								noFound = false;
								break;
							}
						}
					}

					if (noFound)
					{
						diffImageInfoList.Add(imageInfo);
					}
				}
			}

			if (count_fields > 0)
			{
				foreach(AtlasIDFiled field in oldIDClass.fields)
				{
					bool noFound = true;
					if (count_Infos > 0)
					{
						foreach(ImageInfo imageInfo in imageInfos)
						{
							if (field.name == AtlasIDFiled.RawNameToName(imageInfo.name))
							{
								noFound = false;
								break;
							}
						}
					}

					if (noFound)
					{
						diffAtlasIDFiledList.Add(field);
					}
				}
			}


			foreach(ImageInfo imageInfo in diffImageInfoList)
			{
				sb.AppendLine("ImageInfo : " + imageInfo.name + " no matching id in IDClass");
			}

			foreach(AtlasIDFiled field in diffAtlasIDFiledList)
			{
				sb.AppendLine("IDClass : " + field.name + " no matching imageInfo in Atlas");
			}

			return sb.ToString();
		}

		class ReferencedCode
		{
			public string assetPath;
			public int line;
			public ImageInfo imageInfo;

			public ReferencedCode (string assetPath, int line, ImageInfo imageInfo)
			{
				this.assetPath = assetPath;
				this.line = line;
				this.imageInfo = imageInfo;
			}

			public void Open ()
			{
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (assetPath, line);
			}
		}

		/// TexturePacker 打包生成的txt
		/// 当新数据里没有老数据的同名ImageInfo时, 会对所有Prefab的进行引用检测, 并弹出Warning
		public void SetAtlasTPTxt(string text)
		{
			// {frames{textue{},texture{}},mega{}}
			Dictionary<string, object> jsonDic = MiniJSON2.Json.Deserialize(text) as Dictionary<string, object>;
			if (jsonDic == null)
			{
				return;
			}

			Dictionary<string, object> frames = jsonDic["frames"] as Dictionary<string, object>;
			Dictionary<string, object> meta = jsonDic["meta"] as Dictionary<string, object>;

			if (frames == null || meta == null)
			{
				return;
			}

			if (this.GetOldIDClass() != null)
			{
				m_DataNotFullMessage = this.CheckDataIsFull();
				if (!string.IsNullOrEmpty(m_DataNotFullMessage))
				{
					EditorUtility.DisplayDialog("Data is not full !\nOnly full data can update !", m_DataNotFullMessage, "ok");
					return;
				}
			}

			ImageInfo[] oldInfos = _this.ImageInfoArray;
			if (oldInfos == null)
			{
				oldInfos = new ImageInfo[0];
			}

			string[] newInfoNames = new string[frames.Keys.Count];
			frames.Keys.CopyTo(newInfoNames, 0);
			for(int i = 0, len = newInfoNames.Length; i < len; ++i)
			{
				newInfoNames[i] = RemoveFileNameEndSuffix(newInfoNames[i]);
			}
			//string imageInfoName = RemoveFileNameEndSuffix(keyValue.Key);


			// found lost imageInfo
			List<ImageInfo> loseInfoList = new List<ImageInfo>();
			foreach(ImageInfo oldInfo in oldInfos)
			{
				bool noFoundInNew = true;
				foreach(string newInfoName in newInfoNames)
				{
					if (newInfoName == oldInfo.name)
					{
						noFoundInNew = false;
						break;
					}
				}

				if (noFoundInNew)
				{
					loseInfoList.Add(oldInfo);
				}
			}

			// found new imageInfo
			List<string> newInfoList = new List<string>();
			foreach(string newInfoName in newInfoNames)
			{
				bool noFoundInOld = true;
				foreach(ImageInfo oldInfo in oldInfos)
				{
					if (oldInfo.name == newInfoName)
					{
						noFoundInOld = false;
						break;
					}
				}

				if (noFoundInOld)
				{
					newInfoList.Add(newInfoName);
				}
			}

			// if has lost imageInfo, notice warning
			if (loseInfoList.Count > 0)
			{
				// found the lost imageInfo in prefab ref component
				List<MonoBehaviour> refComponentList = new List<MonoBehaviour>();
				List<ImageInfo> refInPrefabInfoList = new List<ImageInfo>();
				FindRefInAllPrefabs(loseInfoList, refComponentList, refInPrefabInfoList);

				List<ReferencedCode> refCodeList = new List<ReferencedCode>();
				FindRefInAllCode(loseInfoList, _this.name + _ImageInfoID, refCodeList);

				StringBuilder sb = new StringBuilder();
				sb.Append(loseInfoList.Count);
				sb.Append(" imageInfos will losed in the new ImageAtlas, \n");
				if (refInPrefabInfoList.Count == 0)
				{
					sb.Append("No lost ImageInfo was referenced in prefab.\n");
				}
				else
				{
					sb.Append("▶");
					sb.Append(refInPrefabInfoList.Count);
					sb.Append(" lost imageInfo(s) was referenced in prefab :\n");
				}

				if (refCodeList.Count == 0)
				{
					sb.Append("No lost ImageInfo was referenced in code.\n");
				}
				else
				{
					sb.Append("▶");
					sb.Append(refCodeList.Count);
					sb.Append(" lost imageInfo(s) was referenced in code :\n");
				}

				const int MaxShowCount = 30;
				int showCount = 0;
				foreach(ImageInfo loseInfo in loseInfoList)
				{
					if (++showCount > MaxShowCount)
					{
						break;
					}

					bool refInPrefab = false;
					foreach(ImageInfo inPrefab in refInPrefabInfoList)
					{
						if (inPrefab == loseInfo)
						{
							refInPrefab = true;
							break;
						}
					}

					bool refInCode = false;
					foreach(ReferencedCode inCode in refCodeList)
					{
						if (inCode.imageInfo == loseInfo)
						{
							refInCode = true;
							break;
						}
					}

					sb.Append(refInPrefab || refInCode? "▶": "  ");
					sb.Append(loseInfo.name);

					if (refInPrefab)
					{
						sb.Append("    [referenced in prefab]");
					}
					if (refInCode)
					{
						sb.Append("    [referenced in code]");
					}
					sb.Append('\n');
				}

				if (showCount > MaxShowCount)
				{
					sb.AppendLine("         ...more...");
				}

				sb.Append("! if force update, the lost ImageInfo(s) will be delete !");
				sb.Append("! the ImageInfo ID referenced in Code need manual check in code editor!");

				if (refInPrefabInfoList.Count > 0)
				{
					int selectValue = UnityEditor.EditorUtility.DisplayDialogComplex("Update Atlas Warning !", sb.ToString(), "View prefab reference details", "Force update", "Cancel Update");
					// default Enter is 0
					// ok 0
					// cancel 1;
					// alt 2;
					if (selectValue == 0)
					{
						ImageAtlasUpdateWarningDetailsWindow.Show(refComponentList, refInPrefabInfoList);
						return;
					}
					else if (selectValue == 2)
					{
						return;
					}
				}
				else if (refCodeList.Count > 0)
				{
					int selectValue = UnityEditor.EditorUtility.DisplayDialogComplex("Update Atlas Warning !", sb.ToString(), "View the first code reference", "Force update", "Cancel Update");
					// default Enter is 0
					// ok 0
					// cancel 1;
					// alt 2;
					if (selectValue == 0)
					{
						refCodeList[0].Open();
						return;
					}
					else if (selectValue == 2)
					{
						return;
					}
				}
				else
				{
					if (UnityEditor.EditorUtility.DisplayDialog("Update Atlas Warning !", sb.ToString(), "Cancel Update", "Force update"))
					{
						return;
					}
				}
			}

			Dictionary<string, object> size = meta["size"] as Dictionary<string, object>;
			int textureW = (int)(long)size["w"];
			int textureH = (int)(long)size["h"];

			int spriteCount = frames.Keys.Count;
			ImageInfo[] imageInfos = new ImageInfo[spriteCount];

			// check the imageInfos folder exists
			string newAssetParentFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_this.GetInstanceID())) + "/ImageInfos";
			if (!Directory.Exists(CXEditor.Tools.AssetsParentPath + newAssetParentFolder))
			{
				Directory.CreateDirectory(newAssetParentFolder);
			}

			int arrayIndex = 0;
			foreach(KeyValuePair<string, object> keyValue in frames)
			{
				string imageInfoName = RemoveFileNameEndSuffix(keyValue.Key);

				ImageInfo current = FindImageInfoInArrayWithName(oldInfos, imageInfoName);
				if (current == null)
				{
					string assetPath = newAssetParentFolder + "/" + imageInfoName + ".asset";
					string assetPathFolder = Path.GetDirectoryName(assetPath);
					if (!Directory.Exists(assetPathFolder))
					{
						Directory.CreateDirectory(assetPathFolder);
					}

					current = ScriptableObject.CreateInstance<ImageInfo>();
					AssetDatabase.CreateAsset(current, newAssetParentFolder + "/" + imageInfoName + ".asset");
				}

				current.atlas = _this;
				current.id = arrayIndex;

				ImageInfoSetJsonObject(current, keyValue.Key, keyValue.Value as Dictionary<string, object>, textureW, textureH);

				UnityEditor.EditorUtility.SetDirty(current);

				imageInfos[arrayIndex] = current;
				arrayIndex++;
			}

			System.Array.Sort(imageInfos, delegate(ImageInfo o1, ImageInfo o2) { return o1.name.CompareTo(o2.name); });

			EasyReflect.SetField(_this, "m_ImageInfos", imageInfos);

			AtlasIDClass.SyncIDClass(_this);

			foreach(ImageInfo lostImageInfo in loseInfoList)
			{
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(lostImageInfo.GetInstanceID()));
			}

			AssetDatabase.Refresh();
		}
	

		ImageInfo FindImageInfoInArrayWithName (ImageInfo[] imageInfoArray, string name)
		{
			foreach(ImageInfo info in imageInfoArray)
			{
				if (info.name == name)
				{
					return info;
				}
			}

			return null;
		}

		void FindRefInAllCode (List<ImageInfo> imageInfoList, string className, List<ReferencedCode> refCodeList)
		{
			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			int curIndex = 0;
			foreach(string assetPath in allAssetPaths)
			{
				EditorUtility.DisplayProgressBar("Find in code", "Find all lost ImageInfo in Code, please wait...", curIndex++ / (float)allAssetPaths.Length);

				if (assetPath.EndsWith(".cs"))
				{
					string csFullPath = CXEditor.Tools.AssetsParentPath + assetPath;
					string[] lines = File.ReadAllLines(csFullPath);

					int lineIndex = 0;
					foreach(string line in lines)
					{
						++lineIndex;
						foreach(ImageInfo imageInfo in imageInfoList)
						{
							string key = className + "." + imageInfo.name;
							if (line.Contains(key))
							{
								refCodeList.Add(new ReferencedCode(assetPath, lineIndex, imageInfo));
							}
						}
					}
				}
			}

			EditorUtility.ClearProgressBar();
		}


			
		void FindRefInAllPrefabs (List<ImageInfo> imageInfoList, List<MonoBehaviour> refComponentList, List<ImageInfo> refImageInfoList)
		{
			FindRefInComponent(Resources.FindObjectsOfTypeAll<MonoBehaviour>(), imageInfoList, refComponentList, refImageInfoList);

			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			int curIndex = 0;
			foreach(string assetPath in allAssetPaths)
			{
				EditorUtility.DisplayProgressBar("Find in prefab", "Find all lost ImageInfo in Prefab, please wait...", curIndex++ / (float)allAssetPaths.Length);

				GameObject o = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				if (o != null)
				{
					MonoBehaviour[] components = o.GetComponentsInChildren<MonoBehaviour>();
					FindRefInComponent(components, imageInfoList, refComponentList, refImageInfoList);
				}
			}

			EditorUtility.ClearProgressBar();
		}

		void FindRefInComponent (MonoBehaviour[] components, List<ImageInfo> imageInfoList, List<MonoBehaviour> refComponentList, List<ImageInfo> refImageInfoList)
		{
			foreach(MonoBehaviour comp in components)
			{
				if (comp == null)
				{
					continue;
				}
				System.Reflection.FieldInfo[] fields = comp.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				foreach(System.Reflection.FieldInfo field in fields)
				{
					if (field.FieldType == typeof(ImageInfo))
					{
						ImageInfo fieldValue = field.GetValue(comp) as ImageInfo;
						foreach(ImageInfo info in imageInfoList)
						{
							if (info == fieldValue)
							{
								if (!refComponentList.Contains(comp))
								{
									refComponentList.Add(comp);
								}

								if (!refImageInfoList.Contains(info))
								{
									refImageInfoList.Add(info);
								}

								break;
							}
						}
					}
				}
			}
		}


		static ImageInfo ImageInfoSetJsonObject (ImageInfo imageInfo, string textureName, Dictionary<string, object> json, int textureW, int textureH)
		{
			imageInfo.name = RemoveFileNameEndSuffix(textureName);

			Dictionary<string, object> sourceSize = json["sourceSize"] as Dictionary<string, object>;
			imageInfo.width = (int)(long)sourceSize["w"];
			imageInfo.height = (int)(long)sourceSize["h"];

			Dictionary<string, object> frame = json["frame"] as Dictionary<string, object>;
			int x = (int)(long)frame["x"];//left
			int y = (int)(long)frame["y"];//top
			int w = (int)(long)frame["w"];
			int h = (int)(long)frame["h"];

			float x0 = x                  / (float)textureW;
			float y0 = (textureH - y - h) / (float)textureH;
			float x1 = (x + w)            / (float)textureW;
			float y1 = (textureH - y)     / (float)textureH;

			imageInfo.uvXMin = x0;
			imageInfo.uvXMax = x1;
			imageInfo.uvYMin = y0;
			imageInfo.uvYMax = y1;

			//imageInfo.uv0 = new Vector2(x0, y0);
			//imageInfo.uv1 = new Vector2(x1, y0);
			//imageInfo.uv2 = new Vector2(x1, y1);
			//imageInfo.uv3 = new Vector2(x0, y1);

			imageInfo.trimmed = (bool)json["trimmed"];

			Dictionary<string, object> spriteSourceSize = json["spriteSourceSize"] as Dictionary<string, object>;
			int trimmed_x = (int)(long)spriteSourceSize["x"];//left
			int trimmed_y = (int)(long)spriteSourceSize["y"];//top
			int trimmed_w = (int)(long)spriteSourceSize["w"];
			int trimmed_h = (int)(long)spriteSourceSize["h"];

			//trimmed_x = trimmed_x;
			trimmed_y = imageInfo.height - trimmed_h - trimmed_y; // 上下对调

			imageInfo.trimLft = trimmed_x;
			imageInfo.trimRgt = imageInfo.width - trimmed_w - trimmed_x;
			imageInfo.trimBtm = trimmed_y;
			imageInfo.trimTop = imageInfo.height - trimmed_h - trimmed_y;


			//imageInfo.visualX = trimmed_x / (float)imageInfo.width;
			//imageInfo.visualY = trimmed_y / (float)imageInfo.height;
			//imageInfo.visualWidth = trimmed_w / (float)imageInfo.width;
			//imageInfo.visualHeight = trimmed_h / (float)imageInfo.height;

			return imageInfo;
		}


		static string RemoveFileNameEndSuffix(string textureName)
		{
			int dotIndex = textureName.Length;
			while((--dotIndex) >= 0)
			{
				if (textureName[dotIndex] == '.')
				{
					break;
				}
			}

			if (dotIndex > 0)
			{
				return textureName.Substring(0, dotIndex);
			}

			return textureName;
		}

		void DrawImageInfoArray ()
		{
			ImageInfo[] array = _this.ImageInfoArray;
			if (array == null)
			{
				return;
			}

			m_ImageInfoArrayFrame.Begin();

			for(int index = 0, len = array.Length; index < len; ++index)
			{
				ImageInfo imageInfo = array[index];
				EditorUILayout.ObjectField("     ◆ " +index + ": ", imageInfo, typeof(ImageInfo));
			}

			m_ImageInfoArrayFrame.End();
		}

		void DrawTexturePackTxt ()
		{
			TextAsset textAsset = (TextAsset)EditorUILayout.ObjectField("TP Txt", null, typeof(TextAsset));
			if (textAsset != null)
			{
				this.SetAtlasTPTxt (textAsset.text);
				//UpdateAtlasIDFile(_this);

				// 存储Prefab数据
				if (PrefabUtility.GetPrefabType(_this) == PrefabType.Prefab)
				{
					EditorUtility.SetDirty(_this);
				}
			}
		}

		const float ButtonHeight = 30;

		public override void OnInspectorGUI ()
		{
			this.CheckMissingImageInfo();

			GUILayout.Space(10f);
			if (!m_DataIsFull)
			{
				EditorGUILayout.HelpBox("Data is not full :\n" + m_DataNotFullMessage, MessageType.Warning);
				if (GUILayout.Button("Force Sync IDClass", GUILayout.Height(ButtonHeight)))
				{
					AtlasIDClass.SyncIDClass(_this);
					AssetDatabase.Refresh();
				}
			}

			_this.Texture = EditorUILayout.TextureField("Texture", _this.Texture);

			if (_this.Texture != null)
			{
				this.DrawTexturePackTxt();
				this.DrawImageInfoArray();
			}
		

			//if (GUILayout.Button("Force Refresh ImageInfo ID Class", GUILayout.Height(ButtonHeight)))
			//{
			//UpdateAtlasIDFile(_this);
			//}
		}
	}

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.SS.UserModel;
using Script.Table;
using UnityEditor;
using UnityEngine;

namespace Editor.Table.ExcelToSO
{
    public class TableEditorToolWindow : BaseEditorWindow
    {
        private static readonly Type[] m_DataTypeArr = new Type[]
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(bool),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(int[]),
            typeof(bool[]),
            typeof(float[]),
            typeof(double[]),
            typeof(decimal[]),
            typeof(string[])
        };

        private enum ECheckDataType
        {
            String,
            Int,
            Long,
            Bool,
            Float,
            Double,
            Decimal,
            IntArray,
            BoolArray,
            FloatArray,
            DoubleArray,
            DecimalArray,
            StringArray,

            ArrayStart = IntArray,
            ArrayEnd = StringArray,

            NONE
        }

        private const string EXCEL_FOLDER_PATH = "Assets/Excels";
        private const string OUTPUT_PATH = "Assets/Addressables/Table/TableDatas.asset";
        private const string PARSER_OUTPUT_PATH = "Assets/Script/TableParser/{0}.cs";
        private const string XLSM = ".xlsm";
        private const string XLSX = ".xlsx";
    
        private const int NAME_IDX = 0;
        private const int DATA_START_HORIZONTAL_INDEX = 0;
        private const int DATA_LOAD_START_LINE_INDEX = 1;
        private const int TABLE_NAME_ROW_INDEX = 0;

        // 테이블 파서를 뽑거나 테이블 데이터를 뽑아내는 과정에서 얻게된 정보들을 담아놓고 어느정보까지 보여줘야할까..? 고민중...
        private static readonly List<string> s_WorkingList = new List<string>();
        private readonly GUILayoutOption r_GUIOption = GUILayout.Height(50f);

        [MenuItem("Table/Table Help")]
        public static void OpenWindow()
        {
            GetWindow<TableEditorToolWindow>("Table Tools");
        }

        private void OnEnable()
        {
            m_GuiDraw = GUIDraw;
        }

        private void GUIDraw()
        {
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("테이블 데이터 뽑기", r_GUIOption) == true)
                ScriptableObjectAllCreate();

            if (GUILayout.Button("테이블 파서 뽑기", r_GUIOption) == true)
                SelectExcelParserMake();

            if (GUILayout.Button("테이블 파서 뽑기 (전체)", r_GUIOption) == true)
                AllExcelParserMake();

            EditorGUILayout.EndVertical();
        }

        private static void ScriptableObjectAllCreate()
        {
            s_WorkingList.Clear();
            var _loadedExcelArr = LoadAllAssetsAtPath(EXCEL_FOLDER_PATH);

            var _newTableObj = CreateInstance<TableScriptObject>();
            _newTableObj.hideFlags = HideFlags.None;

            var _isError = false;
            var _len = _loadedExcelArr.Length;
            const char UnderBar = '_';
            for (var i = 0; i < _len; i++)
            {
                var _curPath = AssetDatabase.GetAssetPath(_loadedExcelArr[i]);

                using var _fs = File.Open(_curPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                
                var _workBook = WorkbookFactory.Create(_fs);
                var _sheetCnt = _workBook.NumberOfSheets;
                var _firstSheetName = string.Empty;

                for (var sheetIndex = 0; sheetIndex < _sheetCnt; sheetIndex++)
                {
                    var _sheet = _workBook.GetSheetAt(sheetIndex);
                    var _sheenName = _sheet.SheetName;

                    if (string.IsNullOrEmpty(_sheenName) || _sheenName[0] == UnderBar)
                        continue;
                    
                    if (string.IsNullOrEmpty(_firstSheetName) == true)
                        _firstSheetName = _sheenName;

                    var _classType = GetType(_sheet.SheetName);

                    if (_classType != null)
                    {
                        var _targetSO = ScriptableObjectMake(_sheet);

                        if (_targetSO != null)
                        {
                            if (_newTableObj.nodes.Find(data => string.Equals(data.name, _targetSO.name)) == false)
                                _newTableObj.nodes.Add(_targetSO);
                            else
                            {
                                s_WorkingList.Add($"{_targetSO.name} :: 중복된 테이블이 있습니다.");
                                _isError = true;
                            }
                        }     
                        else
                            _isError = true;
                    }
                    else
                    {
                        var _firstAsset = _newTableObj.nodes.Find(data => string.Equals(data.name, _firstSheetName));

                        if (_firstAsset == null)
                            continue;

                        var _fieldInfo = new List<FieldInfo>(_firstAsset.GetType().GetFields());
                        var _checkDataList = $"{_sheenName}DataList";

                        FieldInfo _targetFieldInfo = null;
                        foreach (var t in _fieldInfo.Where(t => string.Equals(t.Name, _checkDataList) == true))
                            _targetFieldInfo = t;

                        if(_targetFieldInfo != null)
                            CreateListInstance(_sheet, _targetFieldInfo, ref _firstAsset);
                    }
                }
            }

            AssetDatabase.CreateAsset(_newTableObj, OUTPUT_PATH);
            _newTableObj.nodes.ForEach(node =>
            {
                AssetDatabase.AddObjectToAsset(node, _newTableObj);
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (_isError == false || s_WorkingList.Count <= 0)
            {
                EditorUtility.DisplayDialog("테이블 뽑기 완료!!", "테이블이 정상적으로 뽑혔습니다.", "확인");
                s_WorkingList.Clear();
            }
            else
            {
                string _errStr = string.Empty;

                for (int i = 0; i < s_WorkingList.Count; i++)
                    _errStr = string.Format("{0}\n{1}", _errStr, s_WorkingList[i]);

                EditorUtility.DisplayDialog("테이블 에러", _errStr, "확인");

                s_WorkingList.Clear();
            }
        }

        private static ScriptableObject ScriptableObjectMake(ISheet sheet)
        {
            var _className = sheet.SheetName;

            var _classType = GetType(_className);

            if (_classType == null)
                return null;

            var _dataType = _classType.BaseType?.GenericTypeArguments[0];
            if (_dataType == null)
                return null;
            
            var _newSo = CreateInstance(_classType);

            _newSo.name = _className;

            var _dataList = new List<object>();
            var _nameRow = sheet.GetRow(NAME_IDX);

            var _nameList = new List<string>();
            for (var i = DATA_START_HORIZONTAL_INDEX; i < _nameRow.Count(); i++)
                _nameList.Add(Convert.ToString(_nameRow.GetCell(i)));

            const char SemiColon = ';';
            
            for (var i = DATA_LOAD_START_LINE_INDEX; i < sheet.LastRowNum + 1; i++)
            {
                var _row = sheet.GetRow(i);

                if (_row == null) 
                    continue;

                var _loadCheckStr = Convert.ToString(_row.GetCell(DATA_START_HORIZONTAL_INDEX));
                if (string.IsNullOrEmpty(_loadCheckStr) || _loadCheckStr[0] == SemiColon)
                    continue;

                var _data = Activator.CreateInstance(_dataType);
                var _fieldInfoList = new List<FieldInfo>(_dataType.GetFields());

                foreach (var _targetFieldInfo in _fieldInfoList)
                {
                    var _checkType = _targetFieldInfo.FieldType;
                    var _info = _targetFieldInfo;
                    var _targetIdx = _nameList.FindIndex(data => string.Equals(data, _info.Name));

                    if (_targetIdx < 0)
                        continue;

                    _targetIdx += DATA_START_HORIZONTAL_INDEX;

                    var _cell = _row.GetCell(_targetIdx);

                    if (_cell == null)
                        continue;

                    var _newData = string.Empty;

                    try
                    {
                        if (_cell.CellType == CellType.Formula)
                        {
                            _newData = _cell.CachedFormulaResultType switch
                            {
                                CellType.String => _cell.StringCellValue,
                                CellType.Numeric => _cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
                                CellType.Boolean => _cell.BooleanCellValue.ToString(),
                                _ => _newData
                            };
                        }
                        else
                            _newData = Convert.ToString(_cell);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Excel Cell Parsing Error : {i} : {_targetFieldInfo.Name} : ");
                        Debug.LogError(e);
                    } 

                    if (string.IsNullOrEmpty(_newData)) 
                        continue;
                    try
                    {
                        _targetFieldInfo.SetValue(_data, GetParsingData(_checkType, _newData));
                    }
                    catch
                    {

                        Debug.LogError($"테이블 파서 에러 :: 데이터 형식이 테이블과 맞지 않습니다!  :: {_className} ::{_targetFieldInfo.Name}:: {_checkType}:: {_newData}");
                    }
                }

                _dataList.Add(_data);
            }

            MethodInfo _method = _classType.GetMethod("SetTableData");
            if (_method == null)
                return null;
            
            _method.Invoke(_newSo, new object[] { _dataList });

            return _newSo;
        }

        private static void CreateListInstance(ISheet sheet, FieldInfo targetListFieldInfo, ref ScriptableObject targetAsset)
        {
            var _dataType = targetAsset.GetType().BaseType?.GenericTypeArguments[0];

            if (_dataType == null)
                return;

            var _dataList = new List<object>();
            var _nameRow = sheet.GetRow(NAME_IDX);

            var _nameRowList = new List<string>();
            for (var i = DATA_START_HORIZONTAL_INDEX; i < _nameRow.Count(); i++)
                _nameRowList.Add(Convert.ToString(_nameRow.GetCell(i)));

            const char SemiColon = ';';
            
            for (var i = DATA_LOAD_START_LINE_INDEX; i < sheet.LastRowNum + 1; i++)
            {
                var _row = sheet.GetRow(i);

                if (_row == null)
                    continue;

                var loadCheckString = Convert.ToString(_row.GetCell(DATA_START_HORIZONTAL_INDEX));
                if (string.IsNullOrEmpty(loadCheckString) || loadCheckString[0] == SemiColon)
                    continue;

                var data = Activator.CreateInstance(_dataType);
                List<FieldInfo> fieldInfos = new List<FieldInfo>(_dataType.GetFields());

                for (int j = 0; j < fieldInfos.Count; j++)
                {
                    FieldInfo targetFieldInfo = fieldInfos[j];
                    Type checkType = targetFieldInfo.FieldType;
                    int targetIndex = _nameRowList.FindIndex(data => string.Equals(data, targetFieldInfo.Name));
                    targetIndex += DATA_START_HORIZONTAL_INDEX;

                    ICell cell = _row.GetCell(targetIndex);

                    if (cell == null) continue;

                    string newData = string.Empty;

                    try
                    {
                        if (cell.CellType == CellType.Formula)
                        {
                            switch (cell.CachedFormulaResultType)
                            {
                                case CellType.String:
                                    newData = cell.StringCellValue;
                                    break;
                                case CellType.Numeric:
                                    newData = cell.NumericCellValue.ToString();
                                    break;
                                case CellType.Boolean:
                                    newData = cell.BooleanCellValue.ToString();
                                    break;
                            }
                        }
                        else
                            newData = Convert.ToString(cell);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Excel Cell Parsing Error : {i} : {targetFieldInfo.Name} : ");
                        Debug.LogError(e);
                    }

                    if (string.IsNullOrEmpty(newData) == true) continue;

                    try
                    {
                        targetFieldInfo.SetValue(data, GetParsingData(checkType, newData));
                    }
                    catch
                    {
                        Debug.LogError($"테이블 파서 에러 :: 데이터 형식이 테이블과 맞지 않습니다!  :: {_dataType.Name} :: {newData}");
                    }
                }

                _dataList.Add(data);
            }

            var _list = targetListFieldInfo.GetValue(targetAsset) as IList;
            if (_list == null)
                return;
            
            for (var i = 0; i < _dataList.Count; i++)
                _list.Add(_dataList[i]);
        }

        private static object GetParsingData(Type changeType, string _checkData)
        {
            object _result = null;
            var _dataType = ECheckDataType.NONE;

            for (int i = 0; i < m_DataTypeArr.Length; i++)
            {
                if (changeType == m_DataTypeArr[i])
                {
                    _dataType = (ECheckDataType)i;
                    break;
                }
            }

            if (_dataType == ECheckDataType.NONE)
            {
                if (Enum.TryParse(changeType, _checkData, out _result) == true)
                    return _result;
            }

            if (_dataType is >= ECheckDataType.ArrayStart and <= ECheckDataType.ArrayEnd)
                _checkData = _checkData.Remove(_checkData.Length - 1).Remove(0, 1);

            switch (_dataType)
            {
                case ECheckDataType.String:
                {
                    _result = _checkData;
                    break;
                }
                case ECheckDataType.Int:
                {
                    _result = int.Parse(_checkData);
                    break;
                }
                case ECheckDataType.Long:
                {
                    _result = long.Parse(_checkData);
                    break;
                }
                case ECheckDataType.Bool:
                {
                    _result = bool.Parse(_checkData);
                    break;
                }
                case ECheckDataType.Float:
                {
                    _result = float.Parse(_checkData);
                    break;
                }
                case ECheckDataType.Double:
                {
                    _result = float.Parse(_checkData);
                    break;
                }
                case ECheckDataType.Decimal:
                {
                    _result = decimal.Parse(_checkData);
                    break;
                }
                case ECheckDataType.IntArray:
                {
                    _result = _checkData.Split(',').Select(int.Parse).ToArray();
                    break;
                }
                case ECheckDataType.BoolArray:
                {
                    _result = _checkData.Split(',').Select(bool.Parse).ToArray();
                    break;
                }
                case ECheckDataType.FloatArray:
                {
                    _result = _checkData.Split(',').Select(float.Parse).ToArray();
                    break;
                }
                case ECheckDataType.DoubleArray:
                {
                    _result = _checkData.Split(',').Select(double.Parse).ToArray();
                    break;
                }
                case ECheckDataType.DecimalArray:
                {
                    _result = _checkData.Split(',').Select(decimal.Parse).ToArray();
                    break;
                }
                case ECheckDataType.StringArray:
                {
                    _result = _checkData.Split(',').ToArray();
                    break;
                }
            }

            return _result;
        }

        private void AllExcelParserMake()
        {
            var _assetArr = LoadAllAssetsAtPath(EXCEL_FOLDER_PATH);
            const char UnderBar = '_';
                        
            foreach (var t in _assetArr)
            {
                var _targetExcelPath = AssetDatabase.GetAssetPath(t);
                using var _stream = File.Open(_targetExcelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                
                var _tableWorkBook = WorkbookFactory.Create(_stream);
                var _sheetCnt = _tableWorkBook.NumberOfSheets;

                for (var sheetIndex = 0; sheetIndex < _sheetCnt; sheetIndex++)
                {
                    var _sheet = _tableWorkBook.GetSheetAt(sheetIndex);
                    var _sheetName = _sheet.SheetName;

                    if (string.IsNullOrEmpty(_sheetName) || _sheetName[0] == UnderBar)
                        continue;

                    CreateParser(_sheet, false);
                }
            }
        }

        private void SelectExcelParserMake()
        {
            var _panelDefaultPath = $"{Application.dataPath}/TableExcels";
            var _path = EditorUtility.OpenFilePanel("Find Exel File Path", _panelDefaultPath,"*");

            if (!_path.Contains(XLSM) && !_path.Contains(XLSX))
                return;

            using var _stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            const char UnderBar = '_';
            
            var _workBook = WorkbookFactory.Create(_stream);
            var _sheetCnt = _workBook.NumberOfSheets;

            for (var i = 0; i < _sheetCnt; i++)
            {
                var _sheet = _workBook.GetSheetAt(i);
                var _sheetName = _sheet.SheetName;

                if (string.IsNullOrEmpty(_sheetName) || _sheetName[0] == UnderBar)
                    continue;

                CreateParser(_sheet);
            }
        }

        private static void CreateParser(ISheet sheet, bool remakeCheckBool = true)
        {
            var _nameRow = sheet.GetRow(TABLE_NAME_ROW_INDEX);
            const char UnderBar = '_';
            
            if(_nameRow == null)
            {
                Debug.Log($"Name Row Error :: {sheet.SheetName}");
                return;
            }

            var _cnt = _nameRow.Cells.Count;
            var _nameStrList = new List<string>();

            for (var i = 0; i < _cnt; i++)
            {
                var _nameStr = Convert.ToString(_nameRow.Cells[i]);
                if (string.IsNullOrEmpty(_nameStr) == false && _nameStr[0] != UnderBar)
                    _nameStrList.Add(_nameStr);
            }

            var _codeMaker = new TableCodeMake();
            var _code = _codeMaker.Generate(sheet.SheetName, _nameStrList, remakeCheckBool);

            if (string.IsNullOrEmpty(_code) != true)
            {
                const string Window = "\n";
                const string Unix = "\r\n";
                const string Mac = "\r";

                if (_code.Contains(Unix))
                    _code = _code.Replace(Unix, Window);

                if (_code.Contains(Mac))
                    _code = _code.Replace(Mac, Window);

                var _output = string.Format(PARSER_OUTPUT_PATH, sheet.SheetName);

                File.WriteAllText(_output, _code);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static UnityEngine.Object[] LoadAllAssetsAtPath(string path)
        {
            if (path.EndsWith("/"))
                path = path.TrimEnd('/');

            var _guidArr = AssetDatabase.FindAssets("", new [] { path });

            var _objArr = new UnityEngine.Object[_guidArr.Length];
            for (var index = 0; index < _guidArr.Length; index++)
            {
                string guid = _guidArr[index];

                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;

                _objArr[index] = asset;
            }

            return _objArr;
        }

        private static Type GetType(string typeName)
        {
            var _type = Type.GetType($"Script.TableParser.{typeName}, assembly-csharp.dll");

            if (_type == null)
            {
                if (typeName.Contains("."))
                {
                    string assemName = typeName.Substring(0, typeName.IndexOf('.'));

                    Assembly assembly = Assembly.Load(assemName);

                    if (assembly == null)
                        return null;

                    if ((_type = assembly.GetType(typeName)) != null)
                        return _type;
                }
            }
            else
                return _type;

            return null;
        }  

        #region Table Parser

        private class TableCodeMake
        {
            private const string codeTemplate = 
@"using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record $CLASSDATA
    {
        $ROW_MEMBER_CODE
    }
    //EndDataRecord//

    [Serializable]
    public class $CLASS : TableNode<$CLASSDATA>, IBaseTableNode
    {
        public override void OnLoadComplete()
        {
            
        }
    
        public override void ClearTable()
        {
            
        }
    
        public $CLASSDATA GetData(int key)
        {
            $CLASSDATA _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Debug.LogError($""No Key. Table : {nameof($CLASS)} Key : {key.ToString()}"");
    
            return _result;
        }
    
        public List<$CLASSDATA> GetDataList()
        {
            return TableDataList;
        }
    
        public $CLASSDATA GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}";

            private const string RE_MAKE_MEMBER_TEMPLATE = 
@"//StartDataRecord//
    [Serializable]
    public record $CLASSDATA
    {
        $ROW_MEMBER_CODE
    }
    //EndDataRecord//";

            private const string DIRECTORY_CHECK_PATH = "/Script/TableParser";                   //Parser Script 생성 경로 존재 여부파악 할떄 사용할 경로
            private const string REMAKE_PARSER_PATH = "Assets/Script/TableParser/{0}.cs";

            private const string FORMAT_KEY_0 = "\"{0}\"";
            private const string FORMAT_KEY_1 = "\"{0}{1}\"";

            private const string PRIVATE_FIRST_DATA_STRING = "[SerializeField] public {0} {1};\n";
            private const string PRIVATE_DATA_STRING = "\t\t[SerializeField] public {0} {1};\n";
            private const string PRIVATE_LAST_DATA_STRING = "\t\t[SerializeField] public {0} {1};";

            private const string RECORD_START_STRING = "//StartDataRecord//";
            private const string RECORD_END_STRING = "//EndDataRecord//";
            
            public string Generate(string className, List<string> checkDataArr, bool remakeCheckBool = true)
            {
                var _checkPath = $"{Application.dataPath}{DIRECTORY_CHECK_PATH}/{className}.cs";
                var _checkFileInfo = new FileInfo(_checkPath);

                if (_checkFileInfo.Exists)
                {
                    if(remakeCheckBool)
                        return EditorUtility.DisplayDialog($"{className} :: 파서가 존재합니다.", "새로뽑으시겠습니까?", "OK", "NO") ? 
                            ReMakeParser(className, checkDataArr) : 
                            string.Empty;
                    
                    return ReMakeParser(className, checkDataArr);
                }

                var _memberCode = "";
                var _cnt = checkDataArr.Count;
                
                for (var i = 0; i < _cnt; i++)
                {
                    if (i == 0)
                        _memberCode += string.Format(PRIVATE_FIRST_DATA_STRING, "int", checkDataArr[i]);
                    else if(i == _cnt - 1)
                        _memberCode += string.Format(PRIVATE_LAST_DATA_STRING, "string", checkDataArr[i]);
                    else
                        _memberCode += string.Format(PRIVATE_DATA_STRING, "string", checkDataArr[i]);
                }

                var _code = codeTemplate;

                _code = _code.Replace("$CLASSDATA", $"{className}Data");
                _code = _code.Replace("$CLASS", className);
                _code = _code.Replace("$ROW_MEMBER_CODE", _memberCode);
                _code = _code.Replace("$FORMATKEY0", FORMAT_KEY_0);
                _code = _code.Replace("$FORMATKEY1", FORMAT_KEY_1);

                return _code;
            }        

            private string ReMakeParser(string className, List<string> checkDatas)
            {
                var _checkPath = string.Format(REMAKE_PARSER_PATH, className);
                var _dataName = $"{className}Data";
                var _dataType = TableEditorToolWindow.GetType(_dataName);
                var _fieldInfoList = new List<FieldInfo>(_dataType.GetFields());

                var _parserAsset = AssetDatabase.LoadAssetAtPath(_checkPath, typeof(TextAsset)) as TextAsset;
                if (_parserAsset == null)
                    return string.Empty;
                
                var _parserStr = _parserAsset.text;

                var _startIdx = _parserStr.IndexOf(RECORD_START_STRING, StringComparison.Ordinal);
                var _endIdx = _parserStr.IndexOf(RECORD_END_STRING, StringComparison.Ordinal);

                var _memberCode = "";
                var _cnt = checkDatas.Count;
                for (var i = 0; i < _cnt; i++)
                {
                    var _targetData = checkDatas[i];
                    var _checkFileInfo = _fieldInfoList.Find(data => string.Equals(data.Name, _targetData));

                    var _dataTypeStr = _checkFileInfo == null ? "string" : GetTypeString(_checkFileInfo.FieldType.Name);

                    if (i == 0)
                        _memberCode += string.Format(PRIVATE_FIRST_DATA_STRING, _dataTypeStr, _targetData);
                    else if (i == _cnt - 1)
                        _memberCode += string.Format(PRIVATE_LAST_DATA_STRING, _dataTypeStr, _targetData);
                    else
                        _memberCode += string.Format(PRIVATE_DATA_STRING, _dataTypeStr, _targetData);
                }

                var _code = RE_MAKE_MEMBER_TEMPLATE;

                _code = _code.Replace("$CLASSDATA", $"{className}Data");
                _code = _code.Replace("$ROW_MEMBER_CODE", _memberCode);

                _parserStr = _parserStr.Remove(_startIdx, _endIdx + 17 - _startIdx).Insert(_startIdx, _code);

                File.WriteAllText(_checkPath, _parserStr);
                EditorUtility.SetDirty(_parserAsset);

                return string.Empty;
            }

            private string GetTypeString(string targetTypeName)
            {
                var _result = targetTypeName;

                switch(targetTypeName)
                {
                    case "Boolean": _result = "bool"; break;
                    case "Decimal": _result = "decimal"; break;
                    case "Double": _result = "double"; break;
                    case "Single": _result = "float"; break;
                    case "Int32": _result = "int"; break;
                    case "UInt32": _result = "uint"; break;
                    case "Int64": _result = "long"; break;
                    case "UInt64": _result = "ulong"; break;
                    case "Int16": _result = "short"; break;
                    case "UInt16": _result = "ushort"; break;
                    case "String": _result = "string"; break;
                    case "Int32[]": _result = "int[]"; break;
                    case "Single[]": _result = "float[]"; break;
                    case "Double[]": _result = "double[]"; break;
                    case "Decimal[]": _result = "decimal[]"; break;
                    case "Int64[]": _result = "long[]"; break;
                    case "Boolean[]": _result = "bool[]"; break;
                    case "String[]": _result = "string[]"; break;
                }

                return _result;
            }
        }

        #endregion Table Parser
    }
}
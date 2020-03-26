using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

[InitializeOnLoad]
public class KingsImEx : EditorWindow {

    static KingsImEx() {
        initialized = false;
        //Debug.Log("static");
    }

    void mDebug(string txt)
    {
        //Debug.Log(txt);   //uncomment to get more information
    }

    [MenuItem("Window/Kings ImEx")]
    public static void Init() {
        GetWindow<KingsImEx>();
       // Debug.Log("init");
    }

    static bool initialized = false;
    public void mInit()
    {
        s_FieldSeparators = new string[]{ ";", ",", ":" };
        s_FieldSeparatorDescription = new string[]{ "Semicolon ( ; )", "Comma ( , )", "Colon ( : )" };
        //Debug.Log("mInit");
        initialized = true;
    }

    public void OnDestroy()
    {
        initialized = false;
    }

    //data source for importing
    public TextAsset importFile;
    public KingsCardStyleList styleDefinitions;
    

    public string directory = "Kings/ImEx";
    public string file = "cardlist.csv";
    public string importFolder = " <-- Click here to select output Folder";
    public CardStack stack;

    public int fieldSeparatorIndex = 0;
    public bool overrideExistingCards = false;
    public bool mergeExistingCards = false;

    public string[] s_FieldSeparators;
    public string[] s_FieldSeparatorDescription;

    //actual state of Import
    [System.Serializable]
    public enum E_ImportState
    {
        none,
        Analyzed_OK,
        Analyzed_Error,
        Imported_OK,
        Imported_Error
    }
    public E_ImportState importState = E_ImportState.none;

    [System.Serializable]
    public enum E_ExportState
    {
        none,
        Export_OK,
        Export_Fail
    }
    public E_ExportState exportState = E_ExportState.none;

    [HideInInspector] public string importInfo = "";

    //Import structure for adjusting prefabs, images and colors depending on group and cardstyle.
    //Cards of different groups are saved in different folders for more simpler linking to the cardStack.
    //Cards with different styles get different prefabs/colors etc. 
    [System.Serializable]
    public class C_CardImportStructure
    {
        public string cardStyle;
        public GameObject prefab;
        //public string GroupName;
        public string color;
        public string icon;
    }

    public List<C_CardImportStructure> CardImportDefinitions;

    //raw data import, not parsed yet
    string[] importLines;
    [System.Serializable]
    public class C_RowData
    {
        public string[] entries;
        public string rowInfo = "";
        public bool rowError = false;
        public string importFolderPath = "";
        public string importFilePath = "";
    }
    [System.Serializable]
    public class C_RawImportData
    {
        public string[] targets;
        public string[] headers;
        public List<C_RowData> rows = new List<C_RowData>();
        public int styleIndex = -1;
        public int groupIndex = -1;
        public int cardNameIndex = -1;
    }
    public C_RawImportData importData;

    //Display all the data.
    Vector2 importScrollPosition = Vector2.zero;
    Vector2 scriptScrollPosition = Vector2.zero;
    bool showImport = false;
    bool showImportRaw = false;
    bool showExport = false;
    private void OnGUI()
    {

        if (initialized == false || s_FieldSeparators==null) {
            mInit();
        }

        scriptScrollPosition= EditorGUILayout.BeginScrollView(scriptScrollPosition);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Options - Import and Export", EditorStyles.boldLabel);


        if (s_FieldSeparatorDescription != null )
        {
            fieldSeparatorIndex = EditorGUILayout.Popup("Field Separator:",
                fieldSeparatorIndex, s_FieldSeparatorDescription, EditorStyles.popup);
        }

        EditorGUILayout.Space();
        showExport = EditorGUILayout.Foldout(showExport, "Export", true);
        if (showExport)
        {
            //EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            directory = EditorGUILayout.TextField("Directory ", directory);
            file = EditorGUILayout.TextField("File ", file);
            EditorGUILayout.EndHorizontal();

            stack = EditorGUILayout.ObjectField("Card Stack", stack, typeof(CardStack), true) as CardStack;

            if (GUILayout.Button("Export Cards"))
            {
                Export();
            }

            switch (exportState)
            {
                case E_ExportState.Export_OK:
                    EditorGUILayout.HelpBox(exportInfo, MessageType.Info);
                    break;
                case E_ExportState.Export_Fail:
                    EditorGUILayout.HelpBox(exportInfo, MessageType.Error);
                    break;
                default:
                    //show nothing
                    break;
            }
        }

        EditorGUILayout.Space();
        // EditorGUILayout.LabelField("Import", EditorStyles.boldLabel);
        showImport = EditorGUILayout.Foldout(showImport, "Import", true);
        if (showImport)
        {
            importFile = EditorGUILayout.ObjectField("Data File", importFile, typeof(TextAsset), false) as TextAsset;
            styleDefinitions = EditorGUILayout.ObjectField("Style Definitions", styleDefinitions, typeof(KingsCardStyleList), false) as KingsCardStyleList;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Output Folder")) {
                importFolder = EditorUtility.OpenFolderPanel("Select Output Folder", "", "");
            }
            GUI.enabled = false;
            EditorGUILayout.TextField(importFolder);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Analyze Import"))
            {
                AnalyzeImportData();
                showImportRaw = true;
            }

            showImportRaw = EditorGUILayout.Foldout(showImportRaw, "Data");
            if (showImportRaw)
            {
                showImportRawData();
            }
            showImportInfo();

            if (importState == E_ImportState.Analyzed_OK) {
                if (GUILayout.Button("Execute Import"))
                {
                    ExecuteImport();
                    //showImportRaw = true;
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    public void showImportInfo() {
        switch (importState)
        {
            case E_ImportState.none:
                break;
            case E_ImportState.Analyzed_OK:
                //EditorGUILayout.HelpBox(importInfo, MessageType.Info);
                break;
            case E_ImportState.Analyzed_Error:
                EditorGUILayout.HelpBox(importInfo, MessageType.Error);
                break;
            case E_ImportState.Imported_OK:
                EditorGUILayout.HelpBox(importInfo, MessageType.Info);
                break;
            case E_ImportState.Imported_Error:
                EditorGUILayout.HelpBox(importInfo, MessageType.Error);
                break;
            default:
                break;
        }
    }

    public void showImportRawData() {

        GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);

        if (importData != null)
        {


            //target row
            if (importData.targets != null)
            {
                EditorGUILayout.BeginHorizontal();
                foreach (string s in importData.targets)
                {
                    EditorGUILayout.TextField(s, EditorStyles.boldLabel);
                }
                EditorGUILayout.TextField("", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

            }

            //header row
            if (importData.headers != null)
            {
                EditorGUILayout.BeginHorizontal();
                foreach (string s in importData.headers)
                {
                    EditorGUILayout.TextField(s, EditorStyles.boldLabel);
                }
                EditorGUILayout.TextField("Info", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

            }

            //data rows
            importScrollPosition = EditorGUILayout.BeginScrollView(importScrollPosition, GUILayout.Height(200f));

            foreach (C_RowData row in importData.rows)
            {
                EditorGUILayout.BeginHorizontal();

                if (row.rowError == true) {
                    TextFieldStyles.normal.textColor = Color.red;
                }
                else
                {
                    TextFieldStyles.normal.textColor = Color.black;
                }

                foreach (string s in row.entries)
                {
                    EditorGUILayout.TextField(s, TextFieldStyles);
                }

                EditorGUILayout.TextField(row.rowInfo, TextFieldStyles);

                TextFieldStyles.normal.textColor = Color.black;

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();

        }
    }

    public void AnalyzeImportData()
    {
        importData = null;//This is intentional.

        //Open the text asset
        if (importFile == null)
        {
            importState = E_ImportState.Analyzed_Error;
            importInfo = "File for importing is not defined.";
            return;
        }

        if (styleDefinitions == null)
        {
            importState = E_ImportState.Analyzed_Error;
            importInfo = "Style file for importing is not defined.";
            return;
        }

        importData = new C_RawImportData();

        fs = getFS(); //use the configured fieldseparator

        mDebug("Fieldseparator: " + fs);
        mDebug("Input file: " + importFile.text);

        //Sometimes unity is not very nice, therefore using an text read workaround for importFile.text, if it is encoded with "illegal" characters:
        byte[] importBytes = importFile.bytes;
        for (int i = 0; i<importBytes.Length; i++)
        {
            if ((importBytes[i] & 0x80)>0)
            {
                Debug.LogWarning("Import data could contain illegal character: 0x" + System.Convert.ToInt32(importBytes[i]).ToString("X") + ". Please check the resulting text and consider storing the import file in UTF8 encoding. If character is illegal is will be replaced by '�'. The corresponding row will be marked '�!', if no row was marked this was a false alarm.");
            }
        }
        string importText = Encoding.UTF8.GetString(importBytes);

        //split file correctly for \r\n and \n
        importLines = importText.Replace("\r\n", "\n").Split('\n');

        for(int i = 0; i<importLines.Length; i++)
        {
            mDebug("Line " + i.ToString() + ": '"+importLines[i]+"'");
        }

        importData.headers = importUnescape(importLines[0]);
        importData.targets = new string[importData.headers.Length];
        for (int i = 0; i<importData.headers.Length; i++) {
            //string[] hSplit;
            mDebug("importdata.headers " + i.ToString() + ": " + importData.headers[i]);

            if (importData.headers[i].Contains('.'))
            {
                importData.targets[i] = importData.headers[i].Split('.')[0];
                importData.headers[i] = importData.headers[i].Split('.')[1];
            }
        }

        for (int i = 1; i < importLines.Length; i++)
        {
            if (string.IsNullOrEmpty(importLines[i]) == false)
            {
                C_RowData col = new C_RowData();

                if (importLines[i].Contains('\uFFFD'))
                {
                    col.rowError = true;
                    col.rowInfo += "�! ";
                }

                col.entries = importUnescape(importLines[i]);
                importData.rows.Add(col);
            }
            else
            {
                Debug.Log("ImExCards: empty line removed.");
            }
        }

        //search through the import file and get the number of different styles
        //get the column
        importData.styleIndex = -1;
        importData.groupIndex = -1;
        importData.cardNameIndex = -1;
        for (int i = 0; i < importData.headers.Length; i++) {
            if (importData.headers[i] == "StyleName")
            {
                importData.styleIndex = i;
            }
            if (importData.headers[i] == "CardName")
            {
                importData.cardNameIndex = i;
            }
            if (importData.headers[i] == "GroupName")
            {
                importData.groupIndex = i;
            }

            mDebug("Header col. " + i.ToString() + ": " + importData.headers[i]);
        }

        

        //error if no found
        if (importData.styleIndex == -1 || importData.cardNameIndex == -1 || importData.groupIndex == -1)
        {
            importInfo = "";
            if (importData.styleIndex == -1) {
                importInfo += "Could not find 'StyleName' coloumn within the data.\n";
            }
            if (importData.cardNameIndex == -1)
            {
                importInfo += "Could not find 'CardName' coloumn within the data.\n";
            }
            if (importData.groupIndex == -1)
            {
                importInfo += "Could not find 'GroupName' coloumn within the data.\n";
            }

            importState = E_ImportState.Analyzed_Error;
            return;
        }

        //clean the group-and cardName of invalid characters
        foreach (C_RowData row in importData.rows) {
            string s = row.entries[importData.cardNameIndex];
            foreach (var c in Path.GetInvalidFileNameChars()) {
                s = s.Replace(c, '-');
            }
            row.entries[importData.cardNameIndex] = s;

            s = row.entries[importData.groupIndex];
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '-');
            }
            row.entries[importData.groupIndex] = s;
        }


        //make a dictionary out of the styles
        Dictionary<string, string> importStyles = new Dictionary<string, string>();
        int nrOfStyles = 0;
        foreach (C_RowData row in importData.rows) {
            string key = row.entries[importData.styleIndex];
            if (!importStyles.ContainsKey( key )) {
                importStyles.Add(key, "irrelevant");
                nrOfStyles++;
            }
        }

        //test each import row if card style exists
        string missingStyles = "";
        int numberOfMissingStyles = 0;
        int rowCnt = 1;
        foreach (C_RowData row in importData.rows)
        {
            string style = row.entries[importData.styleIndex];
            if (styleDefinitions.HasStyle(style))
            {
                row.rowInfo += "Style OK.";
            }
            else
            {
                row.rowInfo += "Style Error.";
                row.rowError = true;
                
                if (!missingStyles.Contains(" " + style + ";"))
                {
                    numberOfMissingStyles++;
                    missingStyles += " " + style + ";";
                }
            }

            rowCnt++;
        }
        if(numberOfMissingStyles > 0)
        {
            importState = E_ImportState.Analyzed_Error;
            importInfo = "Missing "+numberOfMissingStyles.ToString()+" card styles: " + missingStyles;
            return;
        }

        string styleDefinitionErrors = styleDefinitions.GetCardStyleDefinitionErrors();
        if (styleDefinitionErrors != "") {
            importInfo = styleDefinitionErrors;
            importState = E_ImportState.Imported_Error;
            return;
        }

        importState = E_ImportState.Analyzed_OK;
        importInfo = "OK.";
    }

    public void ExecuteImport() {

        //create the folders
        foreach (C_RowData data in importData.rows)
        {

            string subFolder = data.entries[importData.groupIndex];
            string importFolderPath = importFolder + "/" + subFolder;

            //test if group name is correct
            if (string.IsNullOrEmpty(subFolder))
            {
                importState = E_ImportState.Imported_Error;
                importInfo = "'GroupName' can't be empty. Please reimport correct data set.";
                return;
            }

            //test if card name is correct
            if (string.IsNullOrEmpty(data.entries[importData.cardNameIndex]))
            {
                importState = E_ImportState.Imported_Error;
                importInfo = "'CardName' can't be empty. Please reimport correct data set.";
                return;
            }

            if (importFolderPath.StartsWith(Application.dataPath))
            {
                data.importFolderPath = "Assets" + importFolderPath.Substring(Application.dataPath.Length);
                data.importFilePath = data.importFolderPath + "/" + data.entries[importData.cardNameIndex] + ".prefab";

            }
            else
            {
                importInfo = "The Path '" + importFolderPath + "' is not part of the actual project and can't be used.";
                importState = E_ImportState.Analyzed_Error;
                return;
            }

            //create directory if it doesn't exist yet
            if (!Directory.Exists(importFolderPath))
            {
                //if it doesn't exist, create it
                Directory.CreateDirectory(importFolderPath);
                Debug.Log("Create " + importFolderPath);
            }
        }

        //test if prefab exists
        foreach (C_RowData data in importData.rows)
        {
            string styleName = data.entries[importData.styleIndex];
            GameObject cardPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(data.importFilePath, typeof(GameObject));
            KingsCardStyle cardStyle = styleDefinitions.GetStyle(styleName);

#if UNITY_2018_3_OR_NEWER

            //to get the nested prefabs working a slightly different approach is necessary
            //because actualization is done in between, calling a function would generate an ugly parameterlist
            //=> doing it in this place
            {
                GameObject instanceRoot = null;

                if (cardPrefab == null)
                {
                    if (cardStyle.prefab != null)
                    {
                        // make an instance of the prefab
                        instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(cardStyle.prefab);
                        Debug.Log("Creating card '"+ data.entries[importData.cardNameIndex] + "' based on prefab '" + cardStyle.prefab.name + "' of the cardstyle '"+cardStyle.name+"'.");

                    }
                    else { 
                        Debug.LogError("Actualization/Creation of element failed. The prefab at the cardStyle '"+cardStyle.name+"' is 'null' (missing).");
                    }
                }
                else
                {
                    //an actual prefab exists
                    // make an instance of the already existing prefab
                    instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(cardPrefab);
                    Debug.Log("Actualizing the card prefab '"+cardPrefab.name+"', the linked prefab '" + cardStyle.prefab.name + "' of the cardstyle '" + cardStyle.name + "' is ignored because the prefab already exists.");
                }

                if (instanceRoot != null)
                {
                    // actualize the spawned object
                    ActualizeCard(instanceRoot, data, styleDefinitions, styleName);
                    // create prefab of the changed object
                    bool success = false;
                    UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(instanceRoot, data.importFilePath, InteractionMode.AutomatedAction, out success);
                    
                    if(success == false)
                    {
                        Debug.LogError("The saving as prefab asset was unsuccessful.");
                    }

                    // destroy the spawned object
                    DestroyImmediate(instanceRoot);
                }
                else
                {
                    //some error messages if something failed with the instance Root
                    if (cardStyle.prefab == null)
                    {
                        Debug.LogError("Actualization/Creation of element failed. The prefab at the cardStyle is 'null' (missing).");
                    }
                    else
                    {
                        Debug.LogError("Actualization/Creation of prafab '"+cardStyle.prefab.name+"' failed.");
                    }
                }
            }

#else
            //import precedure for older unity versions    

            //If the name doesn't exist, create the new Prefab
            if (cardPrefab == null)
            { 
                cardPrefab = CreateNewCard(cardStyle.prefab, data.importFilePath);
            }

            ActualizeCard(cardPrefab, data, styleDefinitions, styleName);
#endif

        }

        importState = E_ImportState.Imported_OK;
        importInfo = "Import done.\n" + DateTime.Now.ToShortTimeString();

    }

    static GameObject CreateNewCard(GameObject obj, string localPath)
    {
        Debug.Log("Create '" + localPath + "'");
        //Create a new Prefab at the given path

#if UNITY_2018_1_OR_NEWER
#if UNITY_2018_3_OR_NEWER
        var instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(obj);

        //UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(instanceRoot, localPath);
        UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(instanceRoot, localPath, InteractionMode.AutomatedAction);
        DestroyImmediate(instanceRoot);
        return (GameObject)prefab;
#else
        UnityEngine.Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
        return PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
#else
        Debug.LogError("Import is supported for Unity Version 2018.1 or higher.");
        return null;
#endif

    }
    //actualize the prefab with the data from its row of the imported file
    void ActualizeCard(GameObject obj, C_RowData rowData, KingsCardStyleList cardStyleList, string styleName)
    {
        for (int i = 0; i< importData.targets.Length; i++)
        {
            string s = importData.targets[i];
            if (!string.IsNullOrEmpty(s)) {
                switch (s)
                {
                    case "EventScript":
                        EventScript es = obj.GetComponent<EventScript>();
                        if (es == null)
                        {
                            Debug.LogWarning("GameObject '" + obj.ToString() + "' has no EventScript. Skipping values.");
                        }
                        else
                        {
                            es.SetImportData(importData.headers[i], rowData.entries[i]);
                        }
                        break;
                    case "CardStyle":
                        CardStyle cardStyleScript = obj.GetComponent<CardStyle>();
                        if (cardStyleScript == null)
                        {
                            Debug.LogWarning("GameObject '" + obj.ToString() + "' has no CardStyle Script. Skipping values.");
                        }
                        else
                        {
                            KingsCardStyle cardStyle = styleDefinitions.GetStyle(styleName);
                            if (styleDefinitions.GetOverwriteStyle(styleName))
                            {
                                cardStyleScript.SetStyle(cardStyle);
                                //cardStyleScript.SetStyleName(styleName);
                                cardStyleScript.Refresh();
                            }
                            else {
                                //overwrite is not wanted
                            }
                        }

                        break;
                    default:
                        Debug.LogWarning("Unknown target for import data: '" + s + "'");
                        break;
                }
            }
        }
    }

    string getFS()
    {
        /*switch (options.fieldSeparator)
        {
            case E_FieldSeparatorTypes.semicolon:
                return ";";
            case E_FieldSeparatorTypes.comma:
                return ",";
            case E_FieldSeparatorTypes.colon:
                return ":";
            default:
                return ";";
        }*/
        return s_FieldSeparators[fieldSeparatorIndex];
    }

    string exportEscapeArray(string[] str) {
        string result = "";

        foreach (string s in str) {
            result += exportEscape(s);
        }

        return result;
    }

    //formatter to generate nice escaped csv entries for '"' and ';'
    public static string exportEscape(string str)
    {
        string result = str;
        bool addStringEscape = false;

        if (result.Contains('"'))
        {
            result = result.Replace("\"", "\"\"");
            addStringEscape = true;
        }

        if (result.Contains(";"))
        {
            addStringEscape = true;
        }

        if (addStringEscape == true)
        {
            result = "\"" + result + "\"";
        }

        result += fs;

        return result;
    }

    public string[] importUnescape(string line)
    {
        string[] result;

        fs = getFS(); //use the configured fieldseparator

        // https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain
        result = Regex.Split(line, fs + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        //string can still contain '""' and begin-'"' and end -'"'
        for (int i = 0; i < result.Length; i++)
        {

            if (result[i].Length >= 2)
            {
                if (result[i].Contains('"'))
                {
                    //if it contains somewhere '"' it is also escaped
                    result[i] = result[i].Substring(1, result[i].Length - 2);
                }
            }

            result[i] = result[i].Replace("\"\"", "\"");
        }

        return result;
    }

    //export function
    [HideInInspector] public string exportInfo = "";
    [HideInInspector] public static string fs = ";";
    public void Export()
    {

        exportInfo = "Processing...";
        exportState = E_ExportState.none;

        if (stack == null) {
            exportInfo = "Export failed: Please define card stack location.";
            exportState = E_ExportState.Export_Fail;
            return;
        }

        //get access to the card stack
        //stack = gameObject.GetComponent<CardStack>();

        //create strings
        fs = getFS(); //use the configured fieldseparator (actualize it before exportBeautify is called)
        string header = "";
        string exportString = "";

        //build header
        header += exportEscape("GroupName");
        header += exportEscape("CardName");
        header += exportEscape("StyleName");

        EventScript.eventTexts eventText = new EventScript.eventTexts();
        header += exportEscapeArray( eventText.getCsvHeader());
        //remove last charcter
        header = header.Substring(0, header.Length - 1);

        exportString += (header + "\n");

        //some stats log
        int expSuccess = 0;
        int expMissingStyleScript = 0;
        int expFail = 0;

        //crawl through the cards of the stack and generate card export strings
        // 0. Temporary variables
        EventScript es = null;
        string cardString = "";

        // 1. the group
        foreach (CardStack.cardCategory cc in stack.allCards)
        {
            string GroupName = cc.groupName;
            //2. the cards of the group
            foreach (GameObject go in cc.groupCards)
            {
                cardString = "";
                if (go != null)
                {

                    es = go.GetComponent<EventScript>();
                    CardStyle cardStyleScript = go.GetComponent<CardStyle>();

                    if (es != null)
                    {
                        //eventScript exists. Get the data.  
                        cardString = exportEscape(GroupName);
                        cardString += exportEscape(go.name);
                        if (cardStyleScript != null)
                        {
                            cardString += exportEscape(cardStyleScript.GetStyleName());
                        }
                        else {
                            cardString += exportEscape("default");
                            expMissingStyleScript++;
                        }
                        cardString += exportEscapeArray( es.textFields.getCsvData());

                        //remove last charcter
                        cardString = cardString.Substring(0, cardString.Length - 1);

                        expSuccess++;
                    }
                    else
                    {
                        Debug.LogError("Data export error: GameObject '" + go.ToString() + "' doesn't contain an eventScript.");
                        expFail++;
                    }
                }

                //add the string to the export
                exportString += cardString + "\n";
            }
        }

        //create path
        string directoryPath = Application.dataPath + "/" + directory;
        string filePath = directoryPath + "/" + file;
        if (!Directory.Exists(directoryPath))
        {
            //if it doesn't exist, create it
            Directory.CreateDirectory(directoryPath);
        }

        //save the content as file. 
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(exportString);
        outStream.Close();

        exportInfo = "Export to " + filePath + "\nat " + DateTime.Now.ToShortTimeString() + "\nSuccesss: " + expSuccess.ToString() + " Cards.\nMissing StyleScripts: "+expMissingStyleScript.ToString()+"\nFail: " + expFail.ToString() + " Cards.";
        exportState = E_ExportState.Export_OK;
        //string saveState = rowData.Count.ToString() + " entries saved to 'Kings/TermList.txt' at " + DateTime.Now.ToShortTimeString() + ".";
    }

}

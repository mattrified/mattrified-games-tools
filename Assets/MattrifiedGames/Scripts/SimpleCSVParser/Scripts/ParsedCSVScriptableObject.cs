using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MattrifiedGames.CSVParser
{
    /// <summary>
    /// Scriptable object class for parsing comma-separated value (CSV) files.
    /// </summary>
    [CreateAssetMenu(menuName = "Mattrified Games/Simple CSV Parser/Parsed CSV Scriptable Object")]
    public class ParsedCSVScriptableObject : ScriptableObject
    {
#if UNITY_EDITOR
        [Tooltip("The Text Asset in which the CSV is derived.  Editor Only.")]
        public TextAsset csvTextAsset;
#endif

        [Tooltip("The colums that make up this csv sheet.")]
        public List<CSVColumn> columns = new List<CSVColumn>();

        #region Getters

        /// <summary>
        /// The number of columns in this sheet.
        /// </summary>
        public int ColumnCount => columns == null ? -1 : columns.Count;

        /// <summary>
        /// The number of rows in this sheet.
        /// </summary>
        public int RowCount => (columns == null || columns.Count == 0) ? -1 : (columns[0].rowValues == null ? -1 : columns[0].rowValues.Count);

        public string this[int col, int row]
        {
            get
            {
                try
                {
                    return columns[col].rowValues[row];
                }
                catch
                {
                    Debug.LogWarning($"{col} or {row} is out of range.  Returning an empty string.");
                    return string.Empty;
                }
            }
        }

        public CSVColumn this[int col]
        {
            get
            {
                try
                {
                    return columns[col];
                }
                catch
                {
                    Debug.LogWarning($"{col} is out of range.  Returning null.");
                    return null;
                }
            }
        }

        public string this[string headerName, int row]
        {
            get
            {
                try
                {
                    return this[headerName].rowValues[row];
                }
                catch
                {
                    Debug.LogWarning($"{headerName} now found or {row} is out of range.  Returning an empty string.");
                    return string.Empty;
                }
            }
        }

        public CSVColumn this[string header]
        {
            get
            {
                var result = columns.Find(x => x.header.Equals(header));
                if (result == null)
                {
                    Debug.LogWarning($"No column with header {header} found.  Returning null.");
                }

                return result;
            }
        }

        [System.Serializable()]
        public class CSVColumn
        {
            /// <summary>
            /// The first value for the column
            /// </summary>
            [Tooltip("The header for this colum.")]
            public string header;

            [Tooltip("The value for each row in this colum.")]
            public List<string> rowValues;
        }


        /// <summary>
        /// Returns a copy of a column's values
        /// </summary>
        /// <param name="columnIndex">The column index</param>
        /// <param name="includeHeader">If true, the header will be included</param>
        /// <returns></returns>
        public List<string> GetColumnValues(int columnIndex, bool includeHeader)
        {
            var columnResult = this[columnIndex];
            if (columnResult == null)
            {
                Debug.LogWarning("No column found at the given index.  Returning null.");
                return null;
            }

            List<string> rowValues = new List<string>(columnResult.rowValues);
            if (!includeHeader && rowValues.Count > 0)
                rowValues.RemoveAt(0);

            return rowValues;
        }

        /// <summary>
        /// Returns a copy of a column's values
        /// </summary>
        /// <param name="headerValue">The column header being looked for</param>
        /// <param name="includeHeader">If true, the header value is included</param>
        /// <returns></returns>
        public List<string> GetColumnValues(string headerValue, bool includeHeader)
        {
            var columnResult = this[headerValue];
            if (columnResult == null)
            {
                Debug.LogWarning("No column found at the given index.  Returning null.");
                return null;
            }

            List<string> rowValues = new List<string>(columnResult.rowValues);
            if (!includeHeader && rowValues.Count > 0)
                rowValues.RemoveAt(0);

            return rowValues;
        }

        #endregion

        #region Editor Only Scripts
#if UNITY_EDITOR
        [ContextMenu("Parse CSV")]
        public void ParseCSV()
        {
            if (csvTextAsset == null)
            {
                Debug.LogWarning("No CSV File Found");
                return;
            }

            string csvText = csvTextAsset.text;

            columns = new List<CSVColumn>();

            using (var stringReader = new System.IO.StringReader(csvText))
            {
                string line;

                bool isFirstRow = true;
                while ((line = stringReader.ReadLine()) != null)
                {

                    string[] cells = ParseCsvLineWithRegex(line);
                    for (int i = 0; i < cells.Length; i++)
                    {
                        cells[i] = RemoveSurroundingQuotes(cells[i]);

                        if (isFirstRow)
                        {
                            columns.Add(new CSVColumn() { header = cells[i], rowValues = new List<string>() { cells[i] } });
                        }
                        else
                        {
                            columns[i].rowValues.Add(cells[i]);
                        }
                    }
                    isFirstRow = false;
                }
            }

        }

        /// <summary>
        /// Parses the CSV line using Regular Expressionthe 
        /// </summary>
        /// <param name="line">The line that is being parsed</param>
        /// <returns>The values for each cell in the line</returns>
        static string[] ParseCsvLineWithRegex(string line)
        {
            // Regex pattern to match CSV fields
            string pattern = @"(?:^|,)(?:""((?:[^""]|"""")*)""|([^"",]*))";
            var matches = Regex.Matches(line, pattern);

            var cells = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                // Use the first capturing group (quoted field) or the second group (unquoted field)
                cells[i] = matches[i].Groups[1].Success
                    ? matches[i].Groups[1].Value.Replace("\"\"", "\"") // Handle escaped quotes
                    : matches[i].Groups[2].Value;
            }

            return cells;
        }

        /// <summary>
        /// Removes the surrounding quotes from this cell if present.
        /// </summary>
        /// <param name="input">The cell being cleaned up</param>
        /// <returns></returns>
        static string RemoveSurroundingQuotes(string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                return input.Substring(1, input.Length - 2);
            }
            return input;
        }
#endif
        #endregion
    }

#if UNITY_EDITOR

    /// <summary>
    /// A simple customer editor for the parser
    /// </summary>
    [UnityEditor.CustomEditor(typeof(ParsedCSVScriptableObject))]
    public class ParsedCSVScriptableObjectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Shows a help box with the version number
            UnityEditor.EditorGUILayout.HelpBox("Parsed CSV ScriptableObject Version 0.1.", UnityEditor.MessageType.Info);
            
            UnityEditor.EditorGUILayout.BeginHorizontal();

            var csv = target as ParsedCSVScriptableObject;
            // Opens a preview window for the table itself.
            if (csv && csv.csvTextAsset && GUILayout.Button($"Parse {csv.csvTextAsset?.name}"))
            {
                csv.ParseCSV();
            }

            // Opens a preview window for the table itself.
            if (GUILayout.Button("Open Preview Window"))
            {
                ParsedCSVScriptableObjectWindow.Open(target as ParsedCSVScriptableObject);
            }

            UnityEditor.EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();

            
        }
    }

    /// <summary>
    /// Editor window used to preview Parsed CSVs
    /// </summary>
    public class ParsedCSVScriptableObjectWindow : UnityEditor.EditorWindow
    {
        public ParsedCSVScriptableObject target;

        public static void Open(ParsedCSVScriptableObject csvTarget)
        {
            var newWindow = UnityEditor.EditorWindow.CreateWindow<ParsedCSVScriptableObjectWindow>();
            newWindow.titleContent = new GUIContent(csvTarget?.name);
            newWindow.target = csvTarget;
        }

        Vector2 scroll;

        private void OnGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("Parsed CSV ScriptableObject Version 0.1.", UnityEditor.MessageType.Info);

            var csv = target;

            int columns = csv.ColumnCount;
            int rows = csv.RowCount;

            if (columns < 0 || rows < 0)
            {
                UnityEditor.EditorGUILayout.HelpBox("No column or rows present.", UnityEditor.MessageType.Warning);
                return;
            }

            var bc = GUI.backgroundColor;

            UnityEditor.EditorGUILayout.BeginScrollView(Vector2.zero, false, true);
            GUILayout.BeginHorizontal();
            for (int c = 0; c < columns; c++)
            {
                GUI.backgroundColor = Color.black;
                UnityEditor.EditorGUILayout.SelectableLabel(csv[c, 0], UnityEditor.EditorStyles.textArea);
            }
            GUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.EndScrollView();
            UnityEditor.EditorGUILayout.Space();

            GUI.backgroundColor = Color.white;
            scroll = UnityEditor.EditorGUILayout.BeginScrollView(scroll, true, true);

            GUILayout.BeginHorizontal();
            for (int c = 0; c < columns; c++)
            {
                GUILayout.BeginVertical();
                for (int r = 1; r < rows; r++)
                {
                    GUI.backgroundColor = r == 0 ? Color.black : (r % 2 == 0 ? Color.white : Color.gray);
                    UnityEditor.EditorGUILayout.SelectableLabel(csv[c, r], UnityEditor.EditorStyles.textArea);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.EndScrollView();


            GUI.backgroundColor = bc;
        }
    }

#endif
}
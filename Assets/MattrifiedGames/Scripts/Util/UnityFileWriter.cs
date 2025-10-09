#if UNITY_EDITOR


namespace MattrifiedGames.Utility
{
    public static class UnityFileWriter
    {
        public static void WriteFile(string directory, string fileName, string file)
        {
            // We make sure this is an actual and proper asset directoy
            CheckDirectory(ref directory);

            System.IO.File.WriteAllText(directory + "/" + fileName, file);

            UnityEditor.AssetDatabase.Refresh();
        }

        public static void CheckDirectory(ref string directory)
        {
            if (!directory.StartsWith("Assets/"))
            {
                directory = "Assets/" + directory;
            }

            // End build in a / so if one is added accidentally, we remove it here.
            if (directory.EndsWith("/"))
                directory = directory.Remove(directory.Length - 1);

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }

        public static void BuildCSFile(string directory, string nSpace, string className, string innerText)
        {
            CheckDirectory(ref directory);

            string file = "namespace " + nSpace + "\n{\n";
            file += "\tpublic static class " + className + "\n\t{\n";

            file += innerText;

            file += "\t}\n}";

            System.IO.File.WriteAllText(directory + "/" + className + ".cs", file);

            UnityEditor.AssetDatabase.Refresh();
        }
    }
}

#endif

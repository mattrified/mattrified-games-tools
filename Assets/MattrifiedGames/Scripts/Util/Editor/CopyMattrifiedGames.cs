using UnityEngine;
using UnityEditor;
using System.IO;

public static class CopyMattrifiedGames
{
    [MenuItem("Mattrified Games/Util/Copy Scripts to Base")]
    public static void CopyToMattrifiedBase()
    {
        CopyScriptDirectory("Shared Unity Project");
    }

    [MenuItem("Mattrified Games/Util/Copy Scripts to MerFolk")]
    public static void CopyToMerFight()
    {
        CopyScriptDirectory("Project MerFight");
    }

    [MenuItem("Mattrified Games/Util/Copy Scripts to Fighter")]
    public static void CopyToFighter()
    {
        CopyScriptDirectory("Project 2D");
    }

    [MenuItem("Mattrified Games/Util/Copy Scripts to CupKick")]
    public static void CopyToCupKick()
    {
        CopyScriptDirectory("Project CupKick");
    }

    public static void CopyAll(string sourcePath, string targetPath)
    {
        DirectoryInfo source = new DirectoryInfo(sourcePath);
        DirectoryInfo target = new DirectoryInfo(targetPath);

        CopyAll(source, target);
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        // Check if the target directory exists, if not, create it.
        if (Directory.Exists(target.FullName) == false)
        {
            Directory.CreateDirectory(target.FullName);
        }

        var fileInfo = source.GetFiles();
        int i = 0, len = fileInfo.Length - 1;

        // Copy each file into it’s new directory.
        foreach (FileInfo fi in fileInfo)
        {
            EditorUtility.DisplayProgressBar("Copying Files", string.Format("Copying {0}\\{1}", target.FullName, fi.Name),
                (float)i / len);
            fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }

        EditorUtility.ClearProgressBar();
    }


    static void CopyScriptDirectory(string other)
    {
        CopyAll("Assets/MattrifiedGames/Scripts", string.Format("Assets/../../{0}/Assets/MattrifiedGames/Scripts", other));
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class Changelog : EditorWindow
{
    class PatchNoteEntry
    {
        public string Type;
        public string Description;
        public string Comment;
        public string Author;
        public DateTime DateAdded;
    }

    private string patchNotesFilePath = "Assets/PatchNotes.txt";
    private List<PatchNoteEntry> entries = new List<PatchNoteEntry>();
    private Vector2 scrollPos;

    private string newType = "Add";
    private string newDescription = "";
    private string newComment = "";
    private string userName = "";

    private DateTime exportStartDate = DateTime.Now.Date.AddDays(-7);
    private DateTime exportEndDate = DateTime.Now.Date;

    // Scroll positions for new entry fields
    private Vector2 newDescriptionScroll = Vector2.zero;
    private Vector2 newCommentScroll = Vector2.zero;

    // Single search field
    private string searchQuery = "";

    // Scroll positions for filtered entries (not tied to indices in entries list)
    private List<Vector2> filteredDescriptionScrolls = new List<Vector2>();
    private List<Vector2> filteredCommentScrolls = new List<Vector2>();

    [MenuItem("Project/Editor/Patch Note Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<Changelog>("Dev Log");
        window.minSize = new Vector2(650, 600);
    }

    void OnEnable()
    {
        LoadEntriesFromFile();
    }

    void OnDisable()
    {
        SaveEntriesToFile();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Dev Log", EditorStyles.boldLabel);
        userName = EditorGUILayout.TextField("Your Name / Nickname", userName);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Add New Patch Note Entry", EditorStyles.boldLabel);
        newType = EditorGUILayout.TextField("Type (e.g. Add, Fix, Update)", newType);

        EditorGUILayout.LabelField("Description");
        newDescriptionScroll = EditorGUILayout.BeginScrollView(newDescriptionScroll, GUILayout.Height(120));
        newDescription = EditorGUILayout.TextArea(newDescription, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("Comment (optional)");
        newCommentScroll = EditorGUILayout.BeginScrollView(newCommentScroll, GUILayout.Height(70));
        newComment = EditorGUILayout.TextArea(newComment, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Entry") && !string.IsNullOrWhiteSpace(newDescription))
        {
            AddEntry(newType.Trim(), newDescription.Trim(), newComment.Trim());
            newDescription = "";
            newType = "Add";
            newComment = "";
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Search Entries", EditorStyles.boldLabel);
        searchQuery = EditorGUILayout.TextField("Search", searchQuery);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Existing Patch Notes ({entries.Count})", EditorStyles.boldLabel);

        // Filter entries by search
        IEnumerable<PatchNoteEntry> filteredEntries = entries;
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            filteredEntries = filteredEntries.Where(e =>
                (e.Type != null && e.Type.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (e.Description != null && e.Description.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (e.Comment != null && e.Comment.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (e.Author != null && e.Author.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                e.DateAdded.ToString("yyyy-MM-dd HH:mm:ss").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
            );
        }
        var filteredList = filteredEntries.ToList();

        // Ensure scroll position arrays match filteredList size
        while (filteredDescriptionScrolls.Count < filteredList.Count) filteredDescriptionScrolls.Add(Vector2.zero);
        while (filteredCommentScrolls.Count < filteredList.Count) filteredCommentScrolls.Add(Vector2.zero);
        while (filteredDescriptionScrolls.Count > filteredList.Count) filteredDescriptionScrolls.RemoveAt(filteredDescriptionScrolls.Count - 1);
        while (filteredCommentScrolls.Count > filteredList.Count) filteredCommentScrolls.RemoveAt(filteredCommentScrolls.Count - 1);

        if (filteredList.Count == 0)
        {
            EditorGUILayout.LabelField("No entries found.");
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < filteredList.Count; i++)
            {
                var entry = filteredList[i];

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{entry.Type}]", GUILayout.Width(80));
                entry.Type = EditorGUILayout.TextField(entry.Type);

                // Find index in main entries list for deletion
                int entryIndex = entries.IndexOf(entry);

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (entryIndex >= 0)
                    {
                        entries.RemoveAt(entryIndex);
                        // After deletion, filteredList will be rebuilt next GUI frame, so scroll arrays will be resized accordingly
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break; // break instead of continue, to prevent layout error after list changes
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField($"Author: {entry.Author}");
                EditorGUILayout.LabelField($"Date Added: {entry.DateAdded:yyyy-MM-dd HH:mm:ss}");

                EditorGUILayout.LabelField("Description");
                filteredDescriptionScrolls[i] = EditorGUILayout.BeginScrollView(filteredDescriptionScrolls[i], GUILayout.Height(160));
                entry.Description = EditorGUILayout.TextArea(entry.Description, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();

                EditorGUILayout.LabelField("Comment");
                filteredCommentScrolls[i] = EditorGUILayout.BeginScrollView(filteredCommentScrolls[i], GUILayout.Height(120));
                entry.Comment = EditorGUILayout.TextArea(entry.Comment, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Patch Notes to File"))
        {
            SaveEntriesToFile();
        }
        if (GUILayout.Button("Export Entries"))
        {
            ExportEntriesByDateRange(exportStartDate, exportEndDate);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Export Entries by Date Range", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Start Date (yyyy-MM-dd)", GUILayout.Width(160));
        string startDateStr = EditorGUILayout.TextField(exportStartDate.ToString("yyyy-MM-dd"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("End Date (yyyy-MM-dd)", GUILayout.Width(160));
        string endDateStr = EditorGUILayout.TextField(exportEndDate.ToString("yyyy-MM-dd"));
        EditorGUILayout.EndHorizontal();

        if (DateTime.TryParse(startDateStr, out var parsedStart))
            exportStartDate = parsedStart;
        if (DateTime.TryParse(endDateStr, out var parsedEnd))
            exportEndDate = parsedEnd;
    }

    void AddEntry(string type, string description, string comment)
    {
        var newEntry = new PatchNoteEntry
        {
            Type = string.IsNullOrEmpty(type) ? "Misc" : type,
            Description = description,
            Comment = comment,
            Author = string.IsNullOrWhiteSpace(userName) ? "Anonymous" : userName.Trim(),
            DateAdded = DateTime.Now
        };

        entries.Insert(0, newEntry);

        SaveEntriesToFile(); // Save immediately to preserve DateAdded
    }

    void LoadEntriesFromFile()
    {
        entries.Clear();

        if (!File.Exists(patchNotesFilePath)) return;

        string[] lines = File.ReadAllLines(patchNotesFilePath);

        string currentType = null;
        PatchNoteEntry currentEntry = null;
        bool readingComment = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd();
            string trimmedLine = line.TrimStart();

            if (string.IsNullOrWhiteSpace(line))
            {
                readingComment = false;
                continue;
            }

            if (line.EndsWith(":") && !line.StartsWith("-") && !line.StartsWith("Patch Notes"))
            {
                currentType = line.Substring(0, line.Length - 1);
                continue;
            }

            if (line.StartsWith("- "))
            {
                string desc = line.Substring(2).Trim();
                currentEntry = new PatchNoteEntry
                {
                    Type = currentType ?? "Misc",
                    Description = desc,
                    Comment = "",
                    Author = "Anonymous",
                    DateAdded = DateTime.MinValue
                };
                entries.Add(currentEntry);
                readingComment = false;
                continue;
            }

            // Improved comment parsing logic (fixes author/dateadded in comments)
            if (currentEntry != null && trimmedLine.StartsWith("Comment:"))
            {
                currentEntry.Comment = trimmedLine.Substring("Comment:".Length).Trim();
                readingComment = true;
                continue;
            }
            if (readingComment)
            {
                // Only lines that start with two spaces and are not author/dateadded are part of the comment
                if (line.StartsWith("  ") && !trimmedLine.StartsWith("Author:") && !trimmedLine.StartsWith("DateAdded:"))
                {
                    currentEntry.Comment += "\n" + line.Trim();
                    continue;
                }
                // If we hit Author: or DateAdded: then comment ends
                if (trimmedLine.StartsWith("Author:") || trimmedLine.StartsWith("DateAdded:"))
                {
                    readingComment = false;
                    // Let normal parsing below handle author/dateadded
                }
            }

            // Only parse author/dateadded if not reading comment
            if (currentEntry != null && !readingComment)
            {
                if (trimmedLine.StartsWith("Author:"))
                {
                    currentEntry.Author = trimmedLine.Substring("Author:".Length).Trim();
                    continue;
                }
                else if (trimmedLine.StartsWith("DateAdded:"))
                {
                    string dateString = trimmedLine.Substring("DateAdded:".Length).Trim();

                    if (DateTime.TryParse(dateString, out var parsedDate))
                        currentEntry.DateAdded = parsedDate;
                    else if (currentEntry.DateAdded == DateTime.MinValue)
                        currentEntry.DateAdded = DateTime.Now;

                    continue;
                }
                else
                {
                    currentEntry.Description += "\n" + line.Trim();
                }
            }
        }

        foreach (var entry in entries)
        {
            if (entry.DateAdded == DateTime.MinValue)
            {
                entry.DateAdded = DateTime.Now;
            }
            if (string.IsNullOrWhiteSpace(entry.Author))
            {
                entry.Author = "Anonymous";
            }
        }
    }

    void SaveEntriesToFile()
    {
        string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string headerName = string.IsNullOrWhiteSpace(userName) ? "Anonymous" : userName.Trim();

        var lines = new List<string>
        {
            $"Patch Notes Export - {dateTimeNow} by {headerName}",
            ""
        };

        var grouped = entries.GroupBy(e => e.Type);

        foreach (var group in grouped)
        {
            lines.Add(group.Key + ":");

            foreach (var entry in group)
            {
                string desc = entry.Description.Replace("\n", "\n  ");
                lines.Add($"- {desc}");

                if (!string.IsNullOrWhiteSpace(entry.Comment))
                {
                    var commentLines = entry.Comment.Split(new[] { '\n' }, StringSplitOptions.None);
                    lines.Add("  Comment: " + commentLines[0]);
                    for (int i = 1; i < commentLines.Length; i++)
                        lines.Add("    " + commentLines[i]);
                }

                lines.Add($"  Author: {entry.Author}");
                lines.Add($"  DateAdded: {entry.DateAdded:yyyy-MM-dd HH:mm:ss}");
            }

            lines.Add("");
        }

        File.WriteAllLines(patchNotesFilePath, lines);
        AssetDatabase.Refresh();
    }

    void ExportEntriesByDateRange(DateTime startDate, DateTime endDate)
    {
        var filtered = entries
            .Where(e => e.DateAdded.Date >= startDate.Date && e.DateAdded.Date <= endDate.Date)
            .ToList();

        if (filtered.Count == 0)
        {
            EditorUtility.DisplayDialog("No Entries", "No entries found in the selected date range.", "OK");
            return;
        }

        string exportPath = EditorUtility.SaveFilePanel("Export Patch Notes", "", $"PatchNotes_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.txt", "txt");

        if (string.IsNullOrWhiteSpace(exportPath)) return;

        var lines = new List<string>
        {
            $"Patch Notes Export - From {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            ""
        };

        var grouped = filtered.GroupBy(e => e.Type);

        foreach (var group in grouped)
        {
            lines.Add(group.Key + ":");

            foreach (var entry in group)
            {
                string desc = entry.Description.Replace("\n", "\n  ");
                lines.Add($"- {desc}");

                if (!string.IsNullOrWhiteSpace(entry.Comment))
                {
                    var commentLines = entry.Comment.Split(new[] { '\n' }, StringSplitOptions.None);
                    lines.Add("  Comment: " + commentLines[0]);
                    for (int i = 1; i < commentLines.Length; i++)
                        lines.Add("    " + commentLines[i]);
                }

                lines.Add($"  Author: {entry.Author}");
                lines.Add($"  DateAdded: {entry.DateAdded:yyyy-MM-dd HH:mm:ss}");
            }

            lines.Add("");
        }

        File.WriteAllLines(exportPath, lines);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Export Complete", $"Exported {filtered.Count} entries to:\n{exportPath}", "OK");
    }
}
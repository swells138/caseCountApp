using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace JiraTicketStats
{
    public partial class CaseStats
    {
        private List<TicketRecord> LoadCsv(string filePath)
        {
            var records = new List<TicketRecord>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                string[] headers = parser.ReadFields();

                if (headers == null || headers.Length == 0)
                    throw new InvalidDataException("CSV file contains no header row.");

                // Trim headers and use them for index lookup
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] != null)
                        headers[i] = headers[i].Trim();
                }

                int createdIndex = FindColumnIndex(headers, "Created");
                int resolvedIndex = FindColumnIndex(headers, "Resolved");
                int assigneeIndex = FindColumnIndex(headers, "Assignee");
                int requestTypeIndex = FindColumnIndex(headers, "Custom field (Request Type)");
                int componentIndex = FindColumnIndex(headers, "Custom field (Service Request Component)");
                int reopenedIndex = FindColumnIndex(headers, "Custom field (Re-Opened)");

                while (!parser.EndOfData)
                {
                    string[] fields = null;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch
                    {
                        // Malformed line - skip it
                        continue;
                    }

                    if (fields == null)
                        continue;

                    var record = new TicketRecord
                    {
                        CreatedRaw = GetField(fields, createdIndex),
                        ResolvedRaw = GetField(fields, resolvedIndex),
                        Assignee = GetField(fields, assigneeIndex),
                        RequestType = GetField(fields, requestTypeIndex),
                        ServiceRequestComponent = GetField(fields, componentIndex),
                        Reopened = GetField(fields, reopenedIndex)
                    };

                    records.Add(record);
                }
            }

            return records;
        }

        private static int FindColumnIndex(string[] headers, string name)
        {
            if (headers == null || name == null)
                return -1;

            for (int i = 0; i < headers.Length; i++)
            {
                if (!string.IsNullOrEmpty(headers[i]) &&
                    string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        private static string GetField(string[] fields, int index)
        {
            if (index >= 0 && index < fields.Length)
                return (fields[index] ?? string.Empty).Trim();

            return string.Empty;
        }
    }

    public class TicketRecord
    {
        public string CreatedRaw { get; set; }
        public string ResolvedRaw { get; set; }
        public string Assignee { get; set; }
        public string RequestType { get; set; }
        public string ServiceRequestComponent { get; set; }
        public string Reopened { get; set; }
    }
}
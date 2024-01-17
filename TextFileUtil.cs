using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextUtil
{

    public class TextFileUtil
    {
        public static int FindAndInsert(string filename, string findWord, string toAddState)
        {

            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not found : {filename}");
                return Constants.FAIL;
            }

            List<string> lines = File.ReadAllLines(filename).ToList();
            if (lines == null || lines.Count < 1)
                return Constants.FAIL;

            int count = lines.Count;
            int insertLine = -1;
            for (int i = 0; i < count; i++)
            {
                var line = lines[i];
                if (line.Contains(findWord))
                {
                    insertLine = i + 1;
                    lines.Insert(insertLine, toAddState);
                    break;
                }
            }

            if (insertLine > 0)
            {
                File.WriteAllLines(filename, lines);
                return Constants.SUCCESS;
            }

            return Constants.FAIL;
        }

        public static int FindAndDelete(string filename, string findWord)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not found : {filename}");
                return Constants.FAIL;
            }

            List<string> lines = File.ReadAllLines(filename).ToList();
            if (lines == null || lines.Count < 1)
                return Constants.FAIL;

            int removeCount = lines.RemoveAll(line => line.Contains(findWord));
            
            if (removeCount > 0)
            {
                File.WriteAllLines(filename, lines);
                return Constants.SUCCESS;
            }

            return Constants.FAIL;
        }
    }
}

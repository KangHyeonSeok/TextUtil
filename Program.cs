using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        if(args == null || args.Length < 4) {
            Console.WriteLine("Useses(Insert line) : TextUtil.ext -insertline <filename> <existing statement> <to add statement>");
            return 1;
        }

        if(args[0].ToLower().CompareTo("-insertline") != 0)
        {
            Console.WriteLine("Command not find : " + args[0]);
            return 1;
        }

        return TextFileUtil.FindAndInsert(args[1],args[2],args[3]);
    }
}


public class TextFileUtil {
    public static int FindAndInsert(string filename, string findWord, string toAddState) {

        if(!File.Exists(filename)) {
            Console.WriteLine($"File not found : {filename}");
            return -1;
        }            

        List<string> lines = File.ReadAllLines(filename).ToList();
        if(lines == null || lines.Count < 1)
            return -1;

        int count = lines.Count;
        int insertLine = -1;
        for(int i = 0; i < count; i++) {
            var line = lines[count];
            if(line.Contains(findWord)) {
                insertLine = i+1;
                lines.Insert(insertLine,toAddState);                
                break;
            }
        }

        if(insertLine > 0) {
            File.WriteAllLines(filename,lines,System.Text.Encoding.UTF8);
            return 0;
        }
            
        return 1;
    }
}
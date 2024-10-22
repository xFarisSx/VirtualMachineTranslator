using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMTranslator
{
    public class Parser
    {
        string[] lines;
        List<List<string>> parsedLines = new List<List<string>>() { }; 
        int curI;
        public List<string> curC;
        string curType;
        string[] arithlogs = {"add", 
                              "sub",
                              "neg",
                              "eq",
                              "gt",
                              "lt",
                              "and",
                              "or",
                              "not"};
        string[] memoryacc = { "pop", "push" };
        

        public Parser(string[] lines) {
            this.lines = lines;

            this.ParseAll();

            this.Init();
            

        }

        public void Init() { 
            curI = 0;
            curC = parsedLines[curI];
            curType = CommandType();
        }

        public void ParseAll() {
            foreach (string line in lines) {
                List<string> parsedLine = this.ParseLine(line);
                this.parsedLines.Add(parsedLine);
                
            }
        }

        public List<string> ParseLine(string line)
        {
            List<string> splitted = line.Split(' ').ToList();
            if (this.arithlogs.Contains(splitted[0])) return new List<string> {"C_AR" ,line };
            else if (this.memoryacc.Contains(splitted[0])) {
                return new List<string> { "C_" + splitted[0].ToUpper() }.Concat(splitted).ToList();
            }
            return new List<string> { };
        }

        public bool HasMoreCommands()
        {

            return curI < (lines.Length-1);
        }

        public void Advance() {
            curI++;
            curC = parsedLines[curI];
            curType = CommandType();
        }

        public string CommandType() {
            return curC[0]; }

        public string Arg1() {
            if (curType == "C_RETURN") return "";
            if (curType == "C_AR") return curC[1];
            return curC[2];
        }

        public int Arg2() { 
            return int.Parse(curC[3]);
        }
    }
}

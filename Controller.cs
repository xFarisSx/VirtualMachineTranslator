using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMTranslator
{
    public class Controller
    {
        private string[] files;
        private string outputF;
        private string folderName;
        private string type;
        public Controller(List<string> input, string output, string type,string folderName) {
            this.files = input.ToArray();
            this.outputF = output;
            this.type = type;
            this.folderName=folderName;
        }

        public void Run() {
           
            using (StreamWriter writer = new StreamWriter(outputF, append: true))
            {
               
                Code code = new Code();
                Console.WriteLine(string.Join(", ", this.files));
                Console.WriteLine(Path.GetFullPath("Sys.vm"));
                writer.WriteLine(code.WriteInit(this.files.Contains(Path.GetFullPath(folderName+"\\Sys.vm"))));
                
                foreach (string inputF in files) {
                    code.SetFileName(Path.GetFileName(inputF));
                    string[] lines = File.ReadAllLines(inputF);


                    string[] filteredLines = lines
                        .Where(line => !line.StartsWith("/") && !string.IsNullOrWhiteSpace(line))
                        .Select(line => line.Trim())
                        .ToArray();

                    Parser parser = new Parser(filteredLines);
                    

                    bool finished = false;
                    while (!finished) {
                        string toWrite = "";
                        if (parser.CommandType() == "C_AR")
                        { toWrite = code.WriteAr(parser.Arg1()); }
                        else if (parser.CommandType() == "C_PUSH" || parser.CommandType() == "C_POP")
                        {
                            toWrite = code.WritePushPop(parser.CommandType(), parser.Arg1(), parser.Arg2().ToString());
                        }
                        else if (parser.CommandType() == "C_LABEL")
                        {
                            toWrite = code.WriteLabel(parser.Arg1());
                        }
                        else if (parser.CommandType() == "C_GOTO")
                        {
                            toWrite = code.WriteGoto(parser.Arg1());
                        }
                        else if (parser.CommandType() == "C_IF-GOTO")
                        {
                            toWrite = code.WriteIf(parser.Arg1());
                        }
                        else if (parser.CommandType() == "C_FUNCTION")
                        {
                            toWrite = code.WriteFunction(parser.Arg1(), parser.Arg2());
                        }
                        else if (parser.CommandType() == "C_CALL")
                        {
                            toWrite = code.WriteCall(parser.Arg1(), parser.Arg2());
                        }
                        else if (parser.CommandType() == "C_RETURN")
                        {
                            toWrite = code.WriteReturn();
                        }


                        // Append lines to the file
                        writer.WriteLine(toWrite);


                        if (parser.HasMoreCommands()) { parser.Advance(); }
                        else { finished = true; }
                    }
                }
            
            }
            
        }
    }
}

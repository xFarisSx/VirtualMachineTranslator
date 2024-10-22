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
        private string inputF;
        private string outputF;
        public Controller(string input, string output) {
            this.inputF = input;
            this.outputF = output;
        }

        public void Run() {
            string[] lines = File.ReadAllLines(this.inputF);

           
            string[] filteredLines = lines
                .Where(line => !line.StartsWith("/") && !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim()) 
                .ToArray();


            Parser parser = new Parser(filteredLines);
            Code code = new Code(outputF);

            bool finished = false;
            using (StreamWriter writer = new StreamWriter(outputF, append: true))
            {
                while (!finished) {
                        string toWrite = "";
                        if (parser.CommandType() == "C_AR")
                        { toWrite = code.WriteAr(parser.Arg1()); }
                        else if (parser.CommandType() == "C_PUSH" || parser.CommandType() == "C_POP")
                        {
                            toWrite = code.WritePushPop(parser.CommandType(), parser.Arg1(), parser.Arg2().ToString());
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

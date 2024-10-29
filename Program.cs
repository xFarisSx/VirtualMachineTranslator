using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace VMTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: vmtranslate <filename.vm>");
                return;
            }

            // Get the full path of the VM file
            string path = Path.GetFullPath(args[0]);
            List<string> files = new List<string>();
            string type = "file";

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                Console.WriteLine("Error: Path not found.");
                return;
            }
            if (File.Exists(path) && Path.GetExtension(path) == ".vm")
            {
                type = "file";
                files.Add(path);
            }
            else if (Directory.Exists(path)) {
                type = "folder";
                files = Directory.GetFiles(path, "*.vm", SearchOption.TopDirectoryOnly).ToList();
            }
            try
            {
                string outputFile = "";
                if (type == "file")
                {

                    outputFile = Path.ChangeExtension(path, ".asm");
                }
                else if (type == "folder") {
                    outputFile = path+".asm";
                }
                Controller controller = new Controller(files,outputFile,type, Path.GetFileName(path));
                controller.Run();
                

                Console.WriteLine($"Translation complete. Output file: {outputFile}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine(inputFile);
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

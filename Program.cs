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
            string inputFile = Path.GetFullPath(args[0]);

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Error: File not found.");
                return;
            }
            try
            {
                string outputFile = Path.ChangeExtension(inputFile, ".asm");
                Controller controller = new Controller(inputFile,outputFile);
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

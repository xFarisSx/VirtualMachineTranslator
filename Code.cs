using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VMTranslator
{
    public class Code
    {
        Dictionary<string, string> AVARS = new Dictionary<string, string>
        {
            { "local","LCL" },
            { "argument","ARG"},
            { "this","THIS" },
            { "that","THAT" },
            { "temp", "5" },
        };
        string outputF;
        private string curF = "";
        private int funcC = 0;
        private int labelCounter = 0;

        Dictionary<string, string> arithlogsSym =new Dictionary<string, string> {
            { "add" , "+" },
            { "sub", "-" },
            { "neg", "-" },
            { "eq", "-" },
            { "gt", "-" },
                              { "lt", "-" },
                              { "and", "&" },
                              { "or", "|" },
                              { "not", "!" }};

        public Code() {
            
        }

        public void SetFileName(string fileName)
        {
            outputF = fileName;
        }

        public string WriteInit(bool sys)
        {
            string x = "";
            if (sys)
            {
                
                x = WriteCall("Sys.init", 0);
                
            }
            Console.WriteLine(sys);
            return $@"
{(sys ? @"
@256
D=A
@SP
M=D

"+x : "")}";
        }

        public string WriteLabel(string label) {
            return $@"
({curF}${label})";
        }

        public string WriteGoto(string label)
        {
            return $@"
@{curF}${label}
0;JMP";
        }

        public string WriteIf(string label)
        {
            return $@"
@SP
M=M-1
A=M
D=M
@{curF}${label}
D;JNE";
        }

        public string WriteFunction(string funcName, int varN)
        {
            funcC = 0;
            curF = funcName;
            string toreturn = $@"
({funcName})";
            for (int i = 0; i < varN; i++) {
                toreturn = toreturn + @"
@SP
A=M
M=0
@SP
M=M+1
";
                
            }
            return toreturn;
        }

        public string WriteCall(string funcName, int argN)
        {

            
            string toreturn= $@"
@{curF}$ret.{funcC}                    // Save the current state
D=A
@SP
A=M
M=D
@SP
M=M+1 

@LCL                    // Save the current state
D=M
@SP
A=M
M=D
@SP
M=M+1                  // Save LCL

@ARG                    // Save the current state
D=M
@SP
A=M
M=D
@SP
M=M+1                    // Save ARG

@THIS                    // Save the current state
D=M
@SP
A=M
M=D
@SP
M=M+1                    // Save THIS

@THAT                    // Save the current state
D=M
@SP
A=M
M=D
@SP
M=M+1                    // Save THAT

@SP
D=M
@ARG
M=D
@{5+argN}
D=A
@ARG
M=M-D

@SP
D=M
@LCL
M=D

@{funcName}
0;JMP
({curF}$ret.{funcC})";
            funcC++;


            return toreturn;
        }

        public string WriteReturn()
        {
            

            return $@"
@LCL
D=M
@endFrame
M=D

@5
D=A
@endFrame
D=M-D
A=D
D=M
@retAddr
M=D

@SP
M=M-1
A=M
D=M
@ARG
A=M
M=D
@ARG
D=M
@SP
M=D+1

@1
D=A
@endFrame
D=M-D
A=D
D=M
@THAT
M=D

@2
D=A
@endFrame
D=M-D
A=D
D=M
@THIS
M=D

@3
D=A
@endFrame
D=M-D
A=D
D=M
@ARG
M=D

@4
D=A
@endFrame
D=M-D
A=D
D=M
@LCL
M=D

@retAddr
A=M
0;JMP";
        }


        public string WriteAr(string com)
        {
            labelCounter++;
            string trueLabel = $"EQUAL_TRUE_{labelCounter}";
            string falseLabel = $"EQUAL_FALSE_{labelCounter}";
            string endLabel = $"END_{labelCounter}";
            if (com == "neg" || com == "not") {
                return $@"
@SP
M=M-1
A=M
M={arithlogsSym[com]}M
@SP
M=M+1";
            }
            else if (com == "eq" || com=="gt" || com=="lt")
            {
                return $@"
@SP
M=M-1
A=M
D=M
@RES
M=D
@SP
M=M-1
A=M
D=M
@RES
M=D{arithlogsSym[com]}M
D=M
@{trueLabel}
D;{(com == "eq" ? "JEQ" : com == "gt" ? "JGT" : "JLT")}
@{falseLabel}
0;JMP

({trueLabel})
@SP
A=M
M=-1
@SP
M=M+1
@{endLabel}
0;JMP

({falseLabel})
@SP
A=M
M=0
@SP
M=M+1

({endLabel})";
            }

            Console.WriteLine(com);
            return $@"
@SP
M=M-1
A=M
D=M
@RES
M=D
@SP
M=M-1
A=M
D=M
@RES
M=D{arithlogsSym[com]}M
D=M
@SP
A=M
M=D
@SP
M=M+1";
        }

        public string WritePushPop(string type, string arg1, string arg2)
        {
            labelCounter++;
            if (type == "C_PUSH")
            {

                if (arg1 == "constant") { return $@"
@{arg2}
D=A
@SP
A=M
M=D
@SP
M=M+1"; }
                else if (arg1 == "static")
                {
                    return $@"
@{outputF}.{arg2}
D=M
@SP
A=M
M=D
@SP
M=M+1";
                }
                else if (arg1 == "pointer")
                {
                    return $@"
@{(arg2 == "0" ? "THIS" : "THAT")}
D=M
@SP
A=M
M=D
@SP
M=M+1";
                }
                else if (arg1 == "temp")
                {
                    return $@"
@{(int.Parse(AVARS["temp"])+ int.Parse(arg2))}
D=M
@SP
A=M
M=D
@SP
M=M+1";
                }
                else
                {
                    return $@"
@{arg2}
D=A
@{this.AVARS[arg1]}
A=D+M
D=M
@SP
A=M
M=D
@SP
M=M+1";
                }
            }
            else
            {
                if (arg1 == "constant") { return ""; }
                else if (arg1 == "static")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{outputF}.{arg2}
M=D";
                }
                else if (arg1 == "pointer")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{(arg2 == "0" ? "THIS":"THAT" )}
M=D";
                }
                else if (arg1 == "temp")
                {
                    return $@"
@SP
M=M-1
A=M
D=M
@{int.Parse(AVARS["temp"]) + int.Parse(arg2)}
M=D";
                }
                return $@"
@{AVARS[arg1]}
D=M
@addr
M=D
@{arg2}
D=A
@addr
M=D+M
@SP
M=M-1
A=M
D=M
@addr
A=M
M=D";
            }
        }
    }
}

using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace Catharsium.CodeGen
{
    public class CodeFileWriter
    {
        protected readonly string FileName;


        public CodeFileWriter(string fileName)
        {
            FileName = fileName;
        }


        public void GenerateCSharpCode(CodeCompileUnit compileUnit)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions
            {
                BracingStyle = "C"
            };
            using (var sourceWriter = new StreamWriter(this.FileName))
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, sourceWriter, options);
            }
        }
    }
}
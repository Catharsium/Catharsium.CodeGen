using System;
using System.CodeDom;
using System.Reflection;

namespace Catharsium.CodeGen
{
    public class Program
    {
        private readonly CodeTypeDeclaration targetClass;
        private readonly CodeCompileUnit targetUnit;


        public Program()
        {
            this.targetUnit = new CodeCompileUnit();
            var samples = new CodeNamespace("CodeDOMSample");
            samples.Imports.Add(new CodeNamespaceImport("System"));
            this.targetClass = new CodeTypeDeclaration("CodeDOMCreatedClass")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };
            samples.Types.Add(targetClass);
            this.targetUnit.Namespaces.Add(samples);
        }


        public void AddFields()
        {
            var widthValueField = new CodeMemberField
            {
                Attributes = MemberAttributes.Private,
                Name = "widthValue",
                Type = new CodeTypeReference(typeof(double))
            };
            widthValueField.Comments.Add(new CodeCommentStatement("The width of the object."));
            this.targetClass.Members.Add(widthValueField);

            var heightValueField = new CodeMemberField
            {
                Attributes = MemberAttributes.Private,
                Name = "heightValue",
                Type = new CodeTypeReference(typeof(double))
            };
            heightValueField.Comments.Add(new CodeCommentStatement(
                                              "The height of the object."));
            this.targetClass.Members.Add(heightValueField);
        }


        public void AddProperties()
        {
            var widthProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Width",
                HasGet = true,
                Type = new CodeTypeReference(typeof(double))
            };
            widthProperty.Comments.Add(new CodeCommentStatement("The Width property for the object."));
            widthProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "widthValue")));
            this.targetClass.Members.Add(widthProperty);

            var heightProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Height",
                HasGet = true,
                Type = new CodeTypeReference(typeof(double))
            };
            heightProperty.Comments.Add(new CodeCommentStatement("The Height property for the object."));
            heightProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "heightValue")));
            this.targetClass.Members.Add(heightProperty);

            var areaProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Area",
                HasGet = true,
                Type = new CodeTypeReference(typeof(double))
            };
            areaProperty.Comments.Add(new CodeCommentStatement("The Area property for the object."));

            var areaExpression = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "widthValue"), CodeBinaryOperatorType.Multiply, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "heightValue"));
            areaProperty.GetStatements.Add(new CodeMethodReturnStatement(areaExpression));
            this.targetClass.Members.Add(areaProperty);
        }


        public void AddMethod()
        {
            var toStringMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
                Name = "ToString",
                ReturnType = new CodeTypeReference(typeof(string))
            };

            var widthReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Width");
            var heightReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Height");
            var areaReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Area");

            var returnStatement = new CodeMethodReturnStatement();

            var formattedOutput = "The object:" + Environment.NewLine +
                                  " width = {0}," + Environment.NewLine +
                                  " height = {1}," + Environment.NewLine +
                                  " area = {2}";
            returnStatement.Expression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("System.String"), "Format", new CodePrimitiveExpression(formattedOutput),
                widthReference, heightReference, areaReference);
            toStringMethod.Statements.Add(returnStatement);
            this.targetClass.Members.Add(toStringMethod);
        }


        public void AddConstructor()
        {
            var constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };

            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "width"));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double), "height"));

            var widthReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "widthValue");
            constructor.Statements.Add(new CodeAssignStatement(widthReference, new CodeArgumentReferenceExpression("width")));
            var heightReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "heightValue");
            constructor.Statements.Add(new CodeAssignStatement(heightReference, new CodeArgumentReferenceExpression("height")));
            this.targetClass.Members.Add(constructor);
        }


        public void AddEntryPoint()
        {
            var start = new CodeEntryPointMethod();
            var objectCreate = new CodeObjectCreateExpression(
                    new CodeTypeReference("CodeDOMCreatedClass"),
                    new CodePrimitiveExpression(5.3),
                    new CodePrimitiveExpression(6.9));

            // Add the statement: 
            // "CodeDOMCreatedClass testClass =  
            //     new CodeDOMCreatedClass(5.3, 6.9);"
            start.Statements.Add(new CodeVariableDeclarationStatement(
                                     new CodeTypeReference("CodeDOMCreatedClass"), "testClass",
                                     objectCreate));

            // Creat the expression: 
            // "testClass.ToString()"
            var toStringInvoke =
                new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("testClass"), "ToString");

            // Add a System.Console.WriteLine statement with the previous  
            // expression as a parameter.
            start.Statements.Add(new CodeMethodInvokeExpression(
                                     new CodeTypeReferenceExpression("System.Console"),
                                     "WriteLine", toStringInvoke));
            this.targetClass.Members.Add(start);
        }


        private static void Main()
        {
            var sample = new Program();
            sample.AddFields();
            sample.AddProperties();
            sample.AddMethod();
            sample.AddConstructor();
            sample.AddEntryPoint();
            var filePath = @"D:\SampleCode.cs";
            Console.WriteLine(string.Format("Written to: {0}", filePath));
            new CodeFileWriter(@"D:\SampleCode.cs").GenerateCSharpCode(sample.targetUnit);
        }
    }
}
//
// Microsoft.CSharp.* Test Cases
//
// Authors:
// 	Erik LeBel (eriklebel@yahoo.ca)
//
// (c) 2003 Erik LeBel
//
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace MonoTests.Microsoft.CSharp
{
	/// <summary>
	/// Test ICodeGenerator's GenerateCodeFromNamespace, along with a 
	/// minimal set CodeDom components.
	/// </summary>
	[TestFixture]
	public class CodeGeneratorFromNamespaceTest : CodeGeneratorTestBase
	{
		CodeNamespace codeNamespace = null;

		[SetUp]
		public void Init ()
		{
			InitBase ();
			codeNamespace = new CodeNamespace ();
		}
		
		protected override string Generate (CodeGeneratorOptions options)
		{
			StringWriter writer = new StringWriter ();
			writer.NewLine = NewLine;

			generator.GenerateCodeFromNamespace (codeNamespace, writer, options);
			writer.Close ();
			return writer.ToString ();
		}
		
		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void NullNamespaceTest ()
		{
			codeNamespace = null;
			Generate ();
		}

		[Test]
		public void NullNamespaceNameTest ()
		{
			codeNamespace.Name = null;
			Assert.AreEqual ("\n", Generate ());
		}

		
		[Test]
		public void DefaultNamespaceTest ()
		{
			Assert.AreEqual ("\n", Generate ());
		}

		[Test]
		public void SimpleNamespaceTest ()
		{
			string code = null;

			codeNamespace.Name = "A";
			code = Generate ();
			Assert.AreEqual ("namespace A {\n    \n}\n", code, "#1");

			CodeGeneratorOptions options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";
			code = Generate (options);
			Assert.AreEqual ("namespace A\n{\n    \n}\n", code, "#2");
		}

		[Test]
		public void InvalidNamespaceTest ()
		{
			codeNamespace.Name = "A,B";
			Assert.AreEqual ("namespace A,B {\n    \n}\n", Generate ());
		}

		[Test]
		public void CommentOnlyNamespaceTest ()
		{
			CodeCommentStatement comment = new CodeCommentStatement ("a");
			codeNamespace.Comments.Add (comment);
			Assert.AreEqual ("// a\n\n", Generate ());
		}

		[Test]
		public void ImportsTest ()
		{
			codeNamespace.Imports.Add (new CodeNamespaceImport ("System"));
			codeNamespace.Imports.Add (new CodeNamespaceImport ("System.Collections"));

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"using System;{0}" +
				"using System.Collections;{0}" +
				"{0}", NewLine), Generate (), "#1");

			codeNamespace.Name = "A";

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"namespace A {{{0}" +
				"    using System;{0}" +
				"    using System.Collections;{0}" +
				"    {0}" +
				"}}{0}", NewLine), Generate (), "#2");

			codeNamespace.Name = null;
			codeNamespace.Comments.Add (new CodeCommentStatement ("a"));

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"// a{0}" +
				"using System;{0}" +
				"using System.Collections;{0}" +
				"{0}", NewLine), Generate (), "#3");

			codeNamespace.Name = "A";

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"// a{0}" +
				"namespace A {{{0}" +
				"    using System;{0}" +
				"    using System.Collections;{0}" +
				"    {0}" +
				"}}{0}", NewLine), Generate (), "#4");
		}

		[Test]
		public void TypeTest ()
		{
			codeNamespace.Types.Add (new CodeTypeDeclaration ("Person"));
			Assert.AreEqual (string.Format(CultureInfo.InvariantCulture,
				"{0}" +
				"{0}" +
				"public class Person {{{0}" +
				"}}{0}", NewLine), Generate (), "#A1");

			CodeGeneratorOptions options = new CodeGeneratorOptions ();
			options.BlankLinesBetweenMembers = false;
			Assert.AreEqual (string.Format(CultureInfo.InvariantCulture,
				"{0}" +
				"public class Person {{{0}" +
				"}}{0}", NewLine), Generate (options), "#A2");

			codeNamespace.Name = "A";
			Assert.AreEqual (string.Format(CultureInfo.InvariantCulture,
				"namespace A {{{0}" +
				"    {0}" +
				"    {0}" +
				"    public class Person {{{0}" +
				"    }}{0}" +
				"}}{0}", NewLine), Generate (), "#B1");

			Assert.AreEqual (string.Format (CultureInfo.InvariantCulture,
				"namespace A {{{0}" +
				"    {0}" +
				"    public class Person {{{0}" +
				"    }}{0}" +
				"}}{0}", NewLine), Generate (options), "#B2");
		}
	}
}

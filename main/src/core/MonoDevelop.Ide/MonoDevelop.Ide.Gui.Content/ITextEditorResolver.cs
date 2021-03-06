//
// ITextEditorResolver.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using Microsoft.CodeAnalysis;
using MonoDevelop.Ide.Editor;
using System;

namespace MonoDevelop.Ide.Gui.Content
{
	[Obsolete ("Use the Microsoft.VisualStudio.Text.Editor APIs")]
	public interface ITextEditorResolver
	{
		ISymbol GetLanguageItem (int offset);
		ISymbol GetLanguageItem (int offset, string expression);
	}
	
	[Obsolete ("Use the Microsoft.VisualStudio.Text.Editor APIs")]
	public interface ITextEditorResolverProvider
	{
		ISymbol GetLanguageItem (Document document, int offset, out DocumentRegion expressionRegion);
		ISymbol GetLanguageItem (Document document, int offset, string identifier);
	}

	[Obsolete ("Use the Microsoft.VisualStudio.Text.Editor APIs")]
	public static class TextEditorResolverService
	{
		static List<TextEditorResolverProviderCodon> providers = new List<TextEditorResolverProviderCodon> ();
		
		static TextEditorResolverService ()
		{
			AddinManager.AddExtensionNodeHandler ("/MonoDevelop/Ide/TextEditorResolver", delegate(object sender, ExtensionNodeEventArgs args) {
				switch (args.Change) {
				case ExtensionChange.Add:
					providers.Add ((TextEditorResolverProviderCodon) args.ExtensionNode);
					break;
				case ExtensionChange.Remove:
					providers.Remove ((TextEditorResolverProviderCodon) args.ExtensionNode);
					break;
				}
			});
		}
		
		public static ITextEditorResolverProvider GetProvider (string mimeType)
		{
			TextEditorResolverProviderCodon codon = providers.FirstOrDefault (p => p.MimeType == mimeType);
			if (codon == null)
				return null;
			return codon.CreateResolver ();
		}

		public static ISymbol GetLanguageItem (this Document document, int offset, out DocumentRegion expressionRegion)
		{
			if (document == null)
				throw new System.ArgumentNullException ("document");

			var textEditorResolver = TextEditorResolverService.GetProvider (document.Editor.MimeType);
			if (textEditorResolver != null) {
				return textEditorResolver.GetLanguageItem (document, offset, out expressionRegion);
			}
			expressionRegion = DocumentRegion.Empty;
			return null;
		}
	}
	
	[ExtensionNode (Description="A codon for text editor providers.")]
	[Obsolete ("Use the Microsoft.VisualStudio.Text.Editor APIs")]
	public class TextEditorResolverProviderCodon : ExtensionNode
	{
		[NodeAttribute("mimeType", "Mime type for this text editor provider")]
		string mimeType = null;
		
		[NodeAttribute("class", "Class name.")]
		string className = null;
		
		public string MimeType {
			get { return this.mimeType; }
		}
		
		public ITextEditorResolverProvider CreateResolver ()
		{
			return (ITextEditorResolverProvider) Addin.CreateInstance (className, true);
		}
	}
}

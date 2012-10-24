using D_Parser.Dom;
using ICSharpCode.NRefactory.Completion;
using MonoDevelop.D.Completion;
using MonoDevelop.D.Parser;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;

namespace MonoDevelop.D
{
	class DEditorCompletionExtension:CompletionTextEditorExtension
	{
		#region Properties / Init
		int lastTriggerOffset;
		private Mono.TextEditor.TextEditorData documentEditor;
		
		public override void Initialize()
		{
			base.Initialize();
			
			documentEditor = Document.Editor;	
		}
		#endregion

		#region Code completion
		public override ICompletionDataList CodeCompletionCommand(CodeCompletionContext completionContext)
		{
			int i = 0;
			return HandleCodeCompletion(completionContext,'\0',ref i);
		}

		public override ICompletionDataList HandleCodeCompletion(CodeCompletionContext completionContext, char triggerChar, ref int triggerWordLength)
		{
			if (!(triggerChar==' ' || 
				char.IsLetter(triggerChar) || 
				triggerChar == '@' ||
				triggerChar == '(' ||
				triggerChar == '_' || 
				triggerChar == '.' || 
				triggerChar == '\0'))
				return null;
							
			triggerWordLength = (char.IsLetter(triggerChar) || triggerChar=='_' || triggerChar=='@') ? 1 : 0;

			// Require a parsed D source
			
			var dom = base.Document.ParsedDocument as ParsedDModule;

			if (dom == null)
				return null;

			var l = new CompletionDataList();

			lock(dom)
			lock(dom.DDom)
				DCodeCompletionSupport.BuildCompletionData(
					Document,
					dom.DDom,
					completionContext,
					l,
					triggerChar);
			if (l.Count == 0)
				return null;
			return l;
		}
		#endregion

		#region Parameter completion
		public override int GetCurrentParameterIndex(int startOffset)
		{
			return 0; //TODO: Get actually typed parameter
		}

		public override void CursorPositionChanged()
		{
			if (CompletionWidget != null && Document.Editor.Caret.Offset < lastTriggerOffset)
			{
				ParameterInformationWindowManager.HideWindow(this,CompletionWidget);
				lastTriggerOffset = -1;
			}

			base.CursorPositionChanged();
		}

		public override bool KeyPress(Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			if (this.CompletionWidget != null && (keyChar == ')' || keyChar == ';'))
				ParameterInformationWindowManager.HideWindow(this, CompletionWidget);
			
			return base.KeyPress(key, keyChar, modifier);
		}

		public override IParameterDataProvider HandleParameterCompletion(CodeCompletionContext completionContext, char completionChar)
		{
			if (completionChar != ',' &&
				completionChar != '(' &&
				completionChar != '!')
			{
				return null;
			}
						
			// Require a parsed D source
			var dom = base.Document.ParsedDocument as ParsedDModule;

			if (dom == null)
				return null;

			lastTriggerOffset=completionContext.TriggerOffset;
			return DParameterDataProvider.Create(Document, dom.DDom, completionContext);
		}
		#endregion

		public override bool ExtendsEditor(Document doc, IEditableTextBuffer editor)
		{
			return doc.IsFile && DLanguageBinding.IsDFile(doc.FileName);
		}
	}
}

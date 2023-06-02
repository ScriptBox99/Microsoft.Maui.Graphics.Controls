﻿using Android.Content.Res;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Platform;
using static Android.Views.View;
using ATextAlignment = Android.Views.TextAlignment;

namespace Microsoft.Maui.Graphics.Controls
{
	public partial class EditorHandler : MixedGraphicsControlHandler<IEditorDrawable, IEditor, GraphicsEditor>
	{
		EditorFocusChangeListener FocusChangeListener { get; } = new EditorFocusChangeListener();

		protected override GraphicsEditor CreatePlatformView()
		{
			var platformView = new GraphicsEditor(Context!)
			{
				GraphicsControl = this,
				ImeOptions = ImeAction.Done
			};

			platformView.SetSingleLine(false);
			platformView.Gravity = GravityFlags.Top;
			platformView.TextAlignment = ATextAlignment.ViewStart;
			platformView.SetHorizontallyScrolling(false);
            
			var density = platformView.Resources?.DisplayMetrics?.Density ?? 1.0f;

            if (Drawable is MaterialEditorDrawable)
				platformView.SetPadding((int)(density * 12), (int)(density * 24), 0, 0);
			else if (Drawable is FluentEditorDrawable)
				platformView.SetPadding((int)(density * 12), (int)(density * 12), 0, 0);
			else if (Drawable is CupertinoEditorDrawable)
				platformView.SetPadding((int)(density * 12), (int)(density * 12), 0, 0);

			return platformView;
		}

		protected override void ConnectHandler(GraphicsEditor platformView)
		{
			FocusChangeListener.Handler = this;

            platformView.OnFocusChangeListener = FocusChangeListener;

            platformView.TextChanged += OnTextChanged;
		}

		protected override void DisconnectHandler(GraphicsEditor platformView)
		{
            platformView.OnFocusChangeListener = null;

			FocusChangeListener.Handler = null;

            platformView.TextChanged -= OnTextChanged;
		}

        public static void MapText(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateText(editor);
			(handler as IMixedGraphicsHandler)?.Invalidate();
		}

		public static void MapTextColor(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateTextColor(editor);
		}

		public static void MapCharacterSpacing(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateCharacterSpacing(editor);
		}

		public static void MapFont(EditorHandler handler, IEditor editor)
		{
			// TODO: Get require service FontManager
			//IFontManager? fontManager = null;
			//handler.PlatformView?.UpdateFont(editor, fontManager);
		}

		public static void MapIsReadOnly(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateIsReadOnly(editor);
		}

		public static void MapIsTextPredictionEnabled(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateIsTextPredictionEnabled(editor);
		}

		public static void MapMaxLength(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateMaxLength(editor);
		}

		public static void MapKeyboard(EditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateKeyboard(editor);
		}

		void OnFocusedChange(bool hasFocus)
		{
			AnimatePlaceholder();

			if (!hasFocus)
				VirtualView?.Completed();
		}

		void OnTextChanged(object? sender, TextChangedEventArgs e)
		{
			if (VirtualView is ITextInput textInput)
				textInput.UpdateText(e);
		}

		class EditorFocusChangeListener : Java.Lang.Object, IOnFocusChangeListener
		{
			public EditorHandler? Handler { get; set; }

			public void OnFocusChange(View? v, bool hasFocus)
			{
				if (Handler != null)
				{
					Handler.Drawable.HasFocus = hasFocus;

					Handler.OnFocusedChange(hasFocus);
				}
			}
		}
	}
}
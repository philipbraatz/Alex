﻿using System;
using Alex.Common.Gui.Elements;
using RocketUI;

namespace Alex.Gamestates.Common
{
    public class GuiConfirmState : GuiCallbackStateBase<bool>
    {
        public class GuiConfirmStateOptions
        {
            public string MessageText { get; set; }
            public string MessageTranslationKey { get; set; }

            public string ConfirmText { get; set; } = "Yes";
            public string ConfirmTranslationKey { get; set; } = "gui.yes";

            public string CancelText { get; set; } = "No";
            public string CancelTranslationKey { get; set; } = "gui.no";
        }

        public GuiConfirmState(string message, Action<bool> callbackAction) : this(new GuiConfirmStateOptions()
        {
            MessageText = message
        }, callbackAction)
        {

        }
        public GuiConfirmState(string message, string messageTranslationKey, Action<bool> callbackAction) : this(new GuiConfirmStateOptions()
        {
            MessageText = message,
            MessageTranslationKey = messageTranslationKey
        }, callbackAction)
        {

        }

        public GuiConfirmState(GuiConfirmStateOptions options, Action<bool> callbackAction) : base(callbackAction)
        {
            AddRocketElement(new TextElement()
            {
                Text = options.MessageText,
                TranslationKey = options.MessageTranslationKey
            });
            AddRocketElement(new AlexButton("Confirm", OnConfirmButtonPressed)
            {
                Text           = options.ConfirmText,
                TranslationKey = options.ConfirmTranslationKey
            });
            AddRocketElement(new AlexButton("Cancel", OnCancelButtonPressed)
            {
                Text           = options.CancelText,
                TranslationKey = options.CancelTranslationKey
            });
        }

        private void OnConfirmButtonPressed()
        {
            InvokeCallback(true);
        }

        private void OnCancelButtonPressed()
        {
            InvokeCallback(false);
        }
    }
}

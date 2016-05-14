using SharpDX.DirectWrite;
using SharpDX.UI;
using SharpDX.UI.Controls;
using System;
using System.Windows.Forms;
using Label = SharpDX.UI.Controls.Label;

namespace SharpDX.Scenes
{
    partial class MainMenuScene
    {
        private void LoadUi() {
            _panel = new PanelColored()
                .SetColor(new Color4(0.2f, 0.2f, 0.2f, 1f));

            lblTitle = new Label("Impact", 42f)
                .SetScreenPosition(0, 6)
                .SetHorizontalAlignment(TextAlignment.Center)
                .SetVerticalAlignment(ParagraphAlignment.Center)
                .SetColor(1f, 0f, 0f, 1f)
                .SetScreenSize(_bounds.Width, 40)
                .SetText("SharpDX");

            var btnX = (_bounds.Width - 128) / 2;

            btnNew = new TextButton()
                .SetScreenPosition(btnX, 64)
                .SetScreenSize(128, 32)
                .SetBackgroundColor(new Color4(0f, 0f, 0f, 1f))
                .SetTextColor(new Color4(1f, 1f, 1f, 1f))
                .SetColor(new Color4(0.1f, 0.1f, 0.2f, 1f))
                .SetHoverColor(new Color4(0.2f, 0.2f, 0.4f, 1f))
                .SetCursor(Cursors.Hand)
                .SetText("1) New")
                .OnClick(btnNew_OnClick);

            btnLoad = new TextButton()
                .SetScreenPosition(btnX, 112)
                .SetScreenSize(128, 32)
                .SetBackgroundColor(new Color4(0f, 0f, 0f, 1f))
                .SetTextColor(new Color4(1f, 1f, 1f, 1f))
                .SetColor(new Color4(0.1f, 0.1f, 0.2f, 1f))
                .SetHoverColor(new Color4(0.2f, 0.2f, 0.4f, 1f))
                .SetCursor(Cursors.Hand)
                .SetText("2) Load")
                .OnClick(btnLoad_OnClick);

            btnExit = new TextButton()
                .SetScreenPosition(btnX, 160)
                .SetScreenSize(128, 32)
                .SetBackgroundColor(new Color4(0f, 0f, 0f, 1f))
                .SetTextColor(new Color4(1f, 1f, 1f, 1f))
                .SetColor(new Color4(0.1f, 0.1f, 0.2f, 1f))
                .SetHoverColor(new Color4(0.2f, 0.2f, 0.4f, 1f))
                .SetCursor(Cursors.Hand)
                .SetText("3) Exit")
                .OnClick(btnExit_OnClick);

            _uiMgr.Add(_panel);

            _panel.Children.Add(lblTitle);
            _panel.Children.Add(btnNew);
            _panel.Children.Add(btnLoad);
            _panel.Children.Add(btnExit);

            Program.Form.KeyDown += Form_KeyDown;
        }

        private void UnloadUi() {
            Program.Form.KeyDown -= Form_KeyDown;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.D1:
                    ActionNew();
                    break;
                case Keys.D2:
                    ActionLoad();
                    break;
                case Keys.D3:
                case Keys.Escape:
                    ActionExit();
                    break;
            }
        }

        private void btnNew_OnClick(object sender, EventArgs e) {
            ActionNew();
        }

        private void btnLoad_OnClick(object sender, EventArgs e) {
            ActionLoad();
        }

        private void btnExit_OnClick(object sender, EventArgs e) {
            ActionExit();
        }
    }
}

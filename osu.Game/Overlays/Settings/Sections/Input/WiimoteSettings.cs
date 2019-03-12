// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;

namespace osu.Game.Overlays.Settings.Sections.Input
{
    public class WiimoteSettings : SettingsSubsection
    {
        protected override string Header => "Wiimote";

        public WiimoteSettings()
        {
            Children = new Drawable[]
            {
                new SettingsButton
                {
                    Text = "Connect Wiimote",
                    TooltipText = "Search for a connected wiimote, then enable input from it",
                    Action = connectWiimote
                },
                new SettingsButton
                {
                    Text = "Connect Balance Board",
                    TooltipText = "Search for a connected balance board, then enable input from it",
                    Action = connectBalanceBoard
                },
            };
        }

        private void connectBalanceBoard()
        {
            ((SettingsButton)Children[1]).Text = "Connecting";
        }

        private void connectWiimote()
        {
            ((SettingsButton)Children[0]).Text = "Connecting";
        }
    }
}

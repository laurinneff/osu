﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Graphics.UserInterface
{
    public class OsuTabControl<T> : TabControl<T>
    {
        protected override Dropdown<T> CreateDropdown() => new OsuTabDropdown();

        protected override TabItem<T> CreateTabItem(T value) => new OsuTabItem(value);

        private static bool isEnumType => typeof(T).IsEnum;

        public OsuTabControl()
        {
            TabContainer.Spacing = new Vector2(10f, 0f);

            if (isEnumType)
                foreach (var val in (T[])Enum.GetValues(typeof(T)))
                    AddItem(val);
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            if (accentColour == null)
                AccentColour = colours.Blue;
        }

        private Color4? accentColour;
        public Color4 AccentColour
        {
            get { return accentColour.GetValueOrDefault(); }
            set
            {
                accentColour = value;
                var dropdown = Dropdown as OsuTabDropdown;
                if (dropdown != null)
                    dropdown.AccentColour.Value = value;
                foreach (var item in TabContainer.Children.OfType<OsuTabItem>())
                    item.AccentColour = value;
            }
        }

        public class OsuTabItem : TabItem<T>
        {
            protected readonly SpriteText Text;
            private readonly Box box;

            private Color4? accentColour;
            public Color4 AccentColour
            {
                get { return accentColour.GetValueOrDefault(); }
                set
                {
                    accentColour = value;
                    if (!Active)
                        Text.Colour = value;
                }
            }

            private const float transition_length = 500;

            private void fadeActive()
            {
                box.FadeIn(transition_length, Easing.OutQuint);
                Text.FadeColour(Color4.White, transition_length, Easing.OutQuint);
            }

            private void fadeInactive()
            {
                box.FadeOut(transition_length, Easing.OutQuint);
                Text.FadeColour(AccentColour, transition_length, Easing.OutQuint);
            }

            protected override bool OnHover(InputState state)
            {
                if (!Active)
                    fadeActive();
                return true;
            }

            protected override void OnHoverLost(InputState state)
            {
                if (!Active)
                    fadeInactive();
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                if (accentColour == null)
                    AccentColour = colours.Blue;
            }

            public OsuTabItem(T value) : base(value)
            {
                AutoSizeAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;

                Children = new Drawable[]
                {
                    Text = new OsuSpriteText
                    {
                        Margin = new MarginPadding { Top = 5, Bottom = 5 },
                        Origin = Anchor.BottomLeft,
                        Anchor = Anchor.BottomLeft,
                        Text = (value as Enum)?.GetDescription() ?? value.ToString(),
                        TextSize = 14,
                        Font = @"Exo2.0-Bold", // Font should only turn bold when active?
                    },
                    box = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 1,
                        Alpha = 0,
                        Colour = Color4.White,
                        Origin = Anchor.BottomLeft,
                        Anchor = Anchor.BottomLeft,
                    }
                };
            }

            protected override void OnActivated() => fadeActive();

            protected override void OnDeactivated() => fadeInactive();
        }

        // todo: this needs to go
        private class OsuTabDropdown : OsuDropdown<T>
        {
            public OsuTabDropdown()
            {
                RelativeSizeAxes = Axes.X;
            }

            protected override DropdownMenu CreateMenu()
            {
                var menu = new OsuTabDropdownMenu();
                menu.AccentColour.BindTo(AccentColour);
                return menu;
            }

            protected override DropdownHeader CreateHeader()
            {
                var newHeader = new OsuTabDropdownHeader
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                };

                newHeader.AccentColour.BindTo(AccentColour);

                return newHeader;
            }

            private class OsuTabDropdownMenu : OsuDropdownMenu
            {
                public OsuTabDropdownMenu()
                {
                    Anchor = Anchor.TopRight;
                    Origin = Anchor.TopRight;

                    BackgroundColour = Color4.Black.Opacity(0.7f);
                    MaxHeight = 400;
                }

                protected override DrawableMenuItem CreateDrawableMenuItem(DropdownMenuItem<T> item)
                {
                    var poop = new DrawableOsuTabDropdownMenuItem(this, item);
                    poop.AccentColour.BindTo(AccentColour);
                    return poop;
                }

                private class DrawableOsuTabDropdownMenuItem : DrawableOsuDropdownMenuItem
                {
                    public DrawableOsuTabDropdownMenuItem(Menu<DropdownMenuItem<T>> menu, DropdownMenuItem<T> item)
                        : base(item)
                    {
                        ForegroundColourHover = Color4.Black;
                    }
                }
            }


            protected class OsuTabDropdownHeader : OsuDropdownHeader
            {
                public OsuTabDropdownHeader()
                {
                    RelativeSizeAxes = Axes.None;
                    AutoSizeAxes = Axes.X;

                    BackgroundColour = Color4.Black.Opacity(0.5f);

                    Background.Height = 0.5f;
                    Background.CornerRadius = 5;
                    Background.Masking = true;

                    Foreground.RelativeSizeAxes = Axes.None;
                    Foreground.AutoSizeAxes = Axes.X;
                    Foreground.RelativeSizeAxes = Axes.Y;
                    Foreground.Margin = new MarginPadding(5);

                    Foreground.Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Icon = FontAwesome.fa_ellipsis_h,
                            Size = new Vector2(14),
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                        }
                    };

                    Padding = new MarginPadding { Left = 5, Right = 5 };

                    AccentColour.ValueChanged += accentColourChanged;
                }

                private void accentColourChanged(Color4? newValue)
                {
                    Foreground.Colour = newValue ?? Color4.White;
                }

                protected override bool OnHover(InputState state)
                {
                    Foreground.Colour = BackgroundColour;
                    return base.OnHover(state);
                }

                protected override void OnHoverLost(InputState state)
                {
                    Foreground.Colour = BackgroundColourHover;
                    base.OnHoverLost(state);
                }
            }
        }
    }
}

﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;
using osuTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Colour;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;

namespace osu.Game.Overlays.Direct
{
    public class DirectListPanel : DirectPanel
    {
        private const float transition_duration = 120;
        private const float horizontal_padding = 10;
        private const float vertical_padding = 5;
        private const float height = 70;

        private FillFlowContainer statusContainer;
        private PlayButton playButton;
        private Box progressBar;

        protected override bool FadePlayButton => false;

        protected override PlayButton PlayButton => playButton;
        protected override Box PreviewBar => progressBar;

        public DirectListPanel(BeatmapSetInfo beatmap)
            : base(beatmap)
        {
            RelativeSizeAxes = Axes.X;
            Height = height;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Content.CornerRadius = 5;

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientHorizontal(Color4.Black.Opacity(0.25f), Color4.Black.Opacity(0.75f)),
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = vertical_padding, Bottom = vertical_padding, Left = horizontal_padding, Right = vertical_padding },
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            LayoutEasing = Easing.OutQuint,
                            LayoutDuration = transition_duration,
                            Spacing = new Vector2(10, 0),
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Horizontal,
                                            Children = new Drawable[]
                                            {
                                                playButton = new PlayButton(SetInfo)
                                                {
                                                    Origin = Anchor.CentreLeft,
                                                    Anchor = Anchor.CentreLeft,
                                                    Size = new Vector2(height / 2),
                                                    FillMode = FillMode.Fit,
                                                },
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Direction = FillDirection.Vertical,
                                                    Children = new Drawable[]
                                                    {
                                                        new OsuSpriteText
                                                        {
                                                            Text = new LocalisedString((SetInfo.Metadata.TitleUnicode, SetInfo.Metadata.Title)),
                                                            Font = OsuFont.GetFont(size: 18, weight: FontWeight.Bold, italics: true)
                                                        },
                                                        new OsuSpriteText
                                                        {
                                                            Text = new LocalisedString((SetInfo.Metadata.ArtistUnicode, SetInfo.Metadata.Artist)),
                                                            Font = OsuFont.GetFont(weight: FontWeight.Bold, italics: true)
                                                        },
                                                    }
                                                },
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Horizontal,
                                            Children = new Drawable[]
                                            {
                                                statusContainer = new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Margin = new MarginPadding { Vertical = vertical_padding, Horizontal = 5 },
                                                    Spacing = new Vector2(5),
                                                },
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.X,
                                                    Height = 20,
                                                    Margin = new MarginPadding { Top = vertical_padding, Bottom = vertical_padding },
                                                    Children = GetDifficultyIcons(),
                                                },
                                            },
                                        },
                                    },
                                },
                            }
                        },
                        new FillFlowContainer
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    AutoSizeAxes = Axes.Both,
                                    Child = new DownloadButton(SetInfo)
                                    {
                                        Size = new Vector2(height - vertical_padding * 3),
                                        Margin = new MarginPadding { Left = vertical_padding, Right = vertical_padding },
                                    },
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        new Statistic(FontAwesome.fa_play_circle, SetInfo.OnlineInfo?.PlayCount ?? 0),
                                        new Statistic(FontAwesome.fa_heart, SetInfo.OnlineInfo?.FavouriteCount ?? 0),
                                        new FillFlowContainer
                                        {
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight,
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Horizontal,
                                            Children = new[]
                                            {
                                                new OsuSpriteText
                                                {
                                                    Text = "mapped by ",
                                                    Font = OsuFont.GetFont(size: 14)
                                                },
                                                new OsuSpriteText
                                                {
                                                    Text = SetInfo.Metadata.Author.Username,
                                                    Font = OsuFont.GetFont(size: 14, weight: FontWeight.SemiBold, italics: true)
                                                },
                                            },
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = SetInfo.Metadata.Source,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight,
                                            Font = OsuFont.GetFont(size: 14),
                                            Alpha = string.IsNullOrEmpty(SetInfo.Metadata.Source) ? 0f : 1f,
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
                progressBar = new Box
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    RelativeSizeAxes = Axes.X,
                    BypassAutoSizeAxes = Axes.Y,
                    Size = new Vector2(0, 3),
                    Alpha = 0,
                    Colour = colours.Yellow,
                },
            });

            if (SetInfo.OnlineInfo?.HasVideo ?? false)
            {
                statusContainer.Add(new IconPill(FontAwesome.fa_film) { IconSize = new Vector2(20) });
            }

            if (SetInfo.OnlineInfo?.HasStoryboard ?? false)
            {
                statusContainer.Add(new IconPill(FontAwesome.fa_image) { IconSize = new Vector2(20) });
            }

            statusContainer.Add(new BeatmapSetOnlineStatusPill
            {
                TextSize = 12,
                TextPadding = new MarginPadding { Horizontal = 10, Vertical = 4 },
                Status = SetInfo.OnlineInfo?.Status ?? BeatmapSetOnlineStatus.None,
            });
        }
    }
}

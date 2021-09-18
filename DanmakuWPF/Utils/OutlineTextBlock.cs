using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace DanmakuWPF.Utils
{
    [ContentProperty("Text")]
    public class OutlineTextBlock : FrameworkElement
    {
        private FormattedText _formattedText;

        public OutlineTextBlock()
        {
            TextDecorations = new TextDecorationCollection();
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        // 拉伸
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }

        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public TextDecorationCollection TextDecorations
        {
            get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        // 渲染
        protected override void OnRender(DrawingContext drawingContext)
        {
            EnsureFormattedText();

            if (_formattedText == null)
                return;

            const int outlineWidth = 1;
            const int step = 1;

            _formattedText.SetForegroundBrush(Stroke);

            for (var x = -outlineWidth; x <= outlineWidth; x += step)
            {
                for (var y = -outlineWidth; y <= outlineWidth; y += step)
                {
                    if (x != 0 || y != 0)
                        drawingContext.DrawText(_formattedText, new Point(x, y));
                }
            }

            _formattedText.SetForegroundBrush(Fill);
            drawingContext.DrawText(_formattedText, new Point(0, 0));
        }

        /// <summary>
        /// 传入父容器分配的可用空间，返回该容器根据其子元素大小计算确定的在布局过程中所需的大小。
        /// </summary>
        /// <param name="availableSize">父容器分配的可用空间</param>
        /// <returns>该容器根据其子元素大小计算确定的在布局过程中所需的大小</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            EnsureFormattedText();

            if (_formattedText == null)
                return new Size(0, 0);
            // constrain the formatted text according to the available size
            // the Math.Min call is important - without this constraint (which seems arbitrary,
            // but is the maximum allowable text width),
            // things blow up when availableSize is infinite in both directions
            // the Math.Max call is to ensure we don't hit zero, which will cause MaxTextHeight to throw
            _formattedText.MaxTextWidth = Math.Min(3579139, Math.Max(0.0001d, availableSize.Width));
            _formattedText.MaxTextHeight = Math.Max(0.0001d, availableSize.Height);

            // return the desired size
            return new Size(_formattedText.Width, _formattedText.Height);
        }

        /// <summary>
        /// 传入父容器最终分配的控件大小，返回使用的实际大小
        /// </summary>
        /// <param name="finalSize">最终分配的控件大小</param>
        /// <returns>使用的实际大小</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            EnsureFormattedText();

            if (_formattedText == null)
                return new Size(0, 0);
            // update the formatted text with the final size
            _formattedText.MaxTextWidth = Math.Min(3579139, Math.Max(0.0001d, finalSize.Width));
            _formattedText.MaxTextHeight = Math.Max(0.0001d, finalSize.Height);

            return finalSize;
        }

        private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlineTextBlock)dependencyObject;
            outlinedTextBlock._formattedText = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private static void OnFormattedTextUpdated(DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlineTextBlock)dependencyObject;
            outlinedTextBlock.UpdateFormattedText();

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private void EnsureFormattedText()
        {
            if (_formattedText != null || Text == null)
                return;
            // 添加 DPI
            _formattedText = new FormattedText(Text, 
                CultureInfo.CurrentUICulture, FlowDirection,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal), FontSize, Brushes.Black,
                null, TextFormattingMode.Display, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            UpdateFormattedText();
        }

        private void UpdateFormattedText()
        {
            if (_formattedText == null)
                return;

            _formattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            _formattedText.TextAlignment = TextAlignment;
            _formattedText.Trimming = TextTrimming;

            _formattedText.SetFontSize(FontSize);
            _formattedText.SetFontStyle(FontStyle);
            _formattedText.SetFontWeight(FontWeight);
            _formattedText.SetFontFamily(FontFamily);
            _formattedText.SetFontStretch(FontStretch);
            _formattedText.SetTextDecorations(TextDecorations);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush),
                typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                typeof(OutlineTextBlock), new FrameworkPropertyMetadata(OnFormattedTextInvalidated));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment),
                typeof(OutlineTextBlock), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations",
                typeof(TextDecorationCollection), typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming),
                typeof(OutlineTextBlock),new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping),typeof(OutlineTextBlock),
                new FrameworkPropertyMetadata(TextWrapping.Wrap,OnFormattedTextUpdated));
    }
}

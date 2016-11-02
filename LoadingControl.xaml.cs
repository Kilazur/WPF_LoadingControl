using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;

namespace YourNamespace
{
	/// <summary>
	/// Interaction logic for Loading.xaml
	/// </summary>
	public partial class LoadingControl : UserControl, INotifyPropertyChanged
	{
		private const string COLOR_ANIM_TARGET_PROP = "Fill.(SolidColorBrush.Color)";
		private const string DOUBLE_ANIM_TARGET_PROP = "RenderTransform.ScaleY";
		private Storyboard storyboard;
		private BeginTimeConverter beginTimeConverter;
		private HalfDurationConverter halfDurationConverter;

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public LoadingControl()
		{
			storyboard = new Storyboard();
			halfDurationConverter = new HalfDurationConverter();
			InitializeComponent();
			this.Loaded += (s, e) => ResetRectangles();
		}

		public bool HideCenter
		{
			get { return (bool)GetValue(HideCenterProperty); }
			set
			{
				SetValue(HideCenterProperty, value);
				OnPropertyChanged("HideCenter");
				ResetRectangles();
			}
		}

		public int SideLength
		{
			get { return (int)GetValue(SideLengthProperty); }
			set
			{
				SetValue(SideLengthProperty, value);
				OnPropertyChanged("SideLength");
				ResetRectangles();
			}
		}

		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set
			{
				SetValue(MessageProperty, value);
				OnPropertyChanged("Message");
			}
		}

		public Color Color1
		{
			get { return (Color)GetValue(Color1Property); }
			set
			{
				SetValue(Color1Property, value);
				OnPropertyChanged("Color1");
			}
		}

		public Color Color2
		{
			get { return (Color)GetValue(Color2Property); }
			set
			{
				SetValue(Color2Property, value);
				OnPropertyChanged("Color2");
			}
		}

		public Duration SquareAnimationDuration
		{
			get { return (Duration)GetValue(SquareAnimationDurationProperty); }
			set
			{
				SetValue(SquareAnimationDurationProperty, value);
				OnPropertyChanged("SquareAnimationDuration");
			}
		}

		public IEasingFunction ColorEasing
		{
			get { return (IEasingFunction)GetValue(ColorEasingProperty); }
			set
			{
				SetValue(ColorEasingProperty, value);
				OnPropertyChanged("ColorEasing");
			}
		}

		public IEasingFunction SizeEasing
		{
			get { return (IEasingFunction)GetValue(SizeEasingProperty); }
			set
			{
				SetValue(SizeEasingProperty, value);
				OnPropertyChanged("SizeEasing");
			}
		}

		private void ResetRectangles()
		{
			rectangles.Children.Clear();
			storyboard.Children.Clear();

			int rectsCount = SideLength * SideLength;
			if (HideCenter && SideLength > 2)
				rectsCount -= (SideLength - 2) * (SideLength - 2);

			BeginTimeConverter beginTimeConverter = new BeginTimeConverter(ref rectsCount);

			int[,] indexes = Spiral(SideLength);

			for (int x = 0; x < SideLength; x++)
			{
				for (int y = 0; y < SideLength; y++)
				{
					if (HideCenter && !(y == 0 || y == SideLength - 1 || x == 0 || x == SideLength - 1))
					{
						rectangles.Children.Add(new UIElement());
						continue;
					}
					AddNewRectangle(indexes[x, y], ref beginTimeConverter);
				}
			}
			storyboard.Begin();
		}

		private void AddNewRectangle(int i, ref BeginTimeConverter btc)
		{
			var rectName = "rect" + i;
			var newRect = new Rectangle { Name = rectName };
			rectangles.Children.Add(newRect);
			storyboard.Children.Add(GenerateColorAnimation(ref i, ref newRect, ref btc));
			storyboard.Children.Add(GenerateDoubleAnimation(ref i, ref newRect, ref btc));
		}

		private ColorAnimation GenerateColorAnimation(ref int i, ref Rectangle rect, ref BeginTimeConverter beginTimeConverter)
		{
			var colorAnim = new ColorAnimation
			{
				RepeatBehavior = RepeatBehavior.Forever,
				AutoReverse = true
			};

			BindingOperations.SetBinding(colorAnim, ColorAnimation.BeginTimeProperty,
				new Binding
				{
					Path = new PropertyPath("SquareAnimationDuration"),
					Source = this,
					Converter = beginTimeConverter,
					ConverterParameter = i
				});
			BindingOperations.SetBinding(colorAnim, ColorAnimation.DurationProperty,
				new Binding
				{
					Path = new PropertyPath("SquareAnimationDuration"),
					Source = this,
					Converter = halfDurationConverter,
					ConverterParameter = colorAnim.AutoReverse
				});
			BindingOperations.SetBinding(colorAnim, ColorAnimation.FromProperty,
				new Binding { Path = new PropertyPath("Color1"), Source = this });
			BindingOperations.SetBinding(colorAnim, ColorAnimation.ToProperty,
				new Binding { Path = new PropertyPath("Color2"), Source = this });
			BindingOperations.SetBinding(colorAnim, ColorAnimation.EasingFunctionProperty,
				new Binding { Path = new PropertyPath("ColorEasing"), Source = this });

			Storyboard.SetTarget(colorAnim, rect);
			Storyboard.SetTargetProperty(colorAnim, new PropertyPath(COLOR_ANIM_TARGET_PROP));

			return colorAnim;
		}

		private DoubleAnimation GenerateDoubleAnimation(ref int i, ref Rectangle rect, ref BeginTimeConverter beginTimeConverter)
		{
			var doubleAnim = new DoubleAnimation
			{
				RepeatBehavior = RepeatBehavior.Forever,
				AutoReverse = true,
				From = 1,
				To = 0.15
			};

			BindingOperations.SetBinding(doubleAnim, DoubleAnimation.BeginTimeProperty,
				new Binding
				{
					Path = new PropertyPath("SquareAnimationDuration"),
					Source = this,
					Converter = beginTimeConverter,
					ConverterParameter = i
				});
			BindingOperations.SetBinding(doubleAnim, DoubleAnimation.DurationProperty,
				new Binding
				{
					Path = new PropertyPath("SquareAnimationDuration"),
					Source = this,
					Converter = halfDurationConverter,
					ConverterParameter = doubleAnim.AutoReverse
				});
			BindingOperations.SetBinding(doubleAnim, DoubleAnimation.EasingFunctionProperty,
				new Binding { Path = new PropertyPath("SizeEasing"), Source = this });

			Storyboard.SetTarget(doubleAnim, rect);
			Storyboard.SetTargetProperty(doubleAnim, new PropertyPath(DOUBLE_ANIM_TARGET_PROP));

			return doubleAnim;
		}

		//https://rosettacode.org/wiki/Spiral_matrix#C.23
		public int[,] Spiral(int n)
		{
			int[,] result = new int[n, n];

			int pos = 0;
			int count = n;
			int value = -n;
			int sum = -1;

			do
			{
				value = -1 * value / n;
				for (int i = 0; i < count; i++)
				{
					sum += value;
					result[sum / n, sum % n] = pos++;
				}
				value *= n;
				count--;
				for (int i = 0; i < count; i++)
				{
					sum += value;
					result[sum / n, sum % n] = pos++;
				}
			} while (count > 0);

			return result;
		}

		public static readonly DependencyProperty HideCenterProperty =
			DependencyProperty.Register("HideCenter", typeof(bool),
				typeof(LoadingControl), new PropertyMetadata(true));
		public static readonly DependencyProperty SideLengthProperty =
			DependencyProperty.Register("SideLength", typeof(int),
				typeof(LoadingControl), new PropertyMetadata(3));
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string),
				typeof(LoadingControl), new PropertyMetadata(null));
		public static readonly DependencyProperty Color1Property =
			DependencyProperty.Register("Color1", typeof(Color),
				typeof(LoadingControl), new PropertyMetadata(Colors.Green));
		public static readonly DependencyProperty Color2Property =
			DependencyProperty.Register("Color2", typeof(Color),
				typeof(LoadingControl), new PropertyMetadata(Colors.Yellow));
		public static readonly DependencyProperty SquareAnimationDurationProperty =
			DependencyProperty.Register("SquareAnimationDuration", typeof(Duration),
				typeof(LoadingControl), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.5))));
		public static readonly DependencyProperty ColorEasingProperty =
			DependencyProperty.Register("ColorEasing", typeof(IEasingFunction),
				typeof(LoadingControl), new PropertyMetadata(null));
		public static readonly DependencyProperty SizeEasingProperty =
			DependencyProperty.Register("SizeEasing", typeof(IEasingFunction),
				typeof(LoadingControl), new PropertyMetadata(null));

		private class BeginTimeConverter : IValueConverter
		{
			private readonly int rectsCount;

			public BeginTimeConverter(ref int rectsCount)
			{
				this.rectsCount = rectsCount;
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return TimeSpan.FromSeconds((((Duration)value).TimeSpan.TotalSeconds / rectsCount) * (int)parameter);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private class HalfDurationConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				bool isAutoReversed = parameter == null ? false : (bool)parameter;
				return isAutoReversed ? new Duration(TimeSpan.FromSeconds(((Duration)value).TimeSpan.TotalSeconds / 2.0)) : value;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}

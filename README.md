# WPF_LoadingControl

Just a custom UserControl that displays a message and an animation, best used as a loading overlay screen.
All the custom properties support binding.

### Exemple usage

	<res:LoadingControl VerticalAlignment="Stretch"
						HorizontalAlignment="Stretch"
						Color1="Blue"
						Color2="Orange"
						SquareAnimationDuration="0:0:1"
						Message="Please wait">
		<res:LoadingControl.ColorEasing>
			<SineEase EasingMode="EaseInOut" />
		</res:LoadingControl.ColorEasing>
		<res:LoadingControl.SizeEasing>
			<QuadraticEase EasingMode="EaseIn" />
		</res:LoadingControl.SizeEasing>
	</res:LoadingControl>

#### TODOs

1. Add a bindable property to manage the maximum size (hard-coded 300 right now)

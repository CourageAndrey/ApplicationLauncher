using System;
using System.Globalization;
using System.Windows.Data;

namespace ApplicationLauncher
{
	internal class UpdateTypeRepresenter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var enumValue = (UpdateType) value;
			switch (enumValue)
			{
				case UpdateType.HighlyRecommended:
					return "Важное обновление, которое желательно не пропускать.";
				case UpdateType.Required:
					return "Обязательное обновление, установка которого не может быть пропущена.";
				case UpdateType.Regular:
				default:
					return string.Empty;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
